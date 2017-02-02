/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Services;
using System.Threading;
using OpenIZ.Core.Model.Constants;
using System.Globalization;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a care plan service that can bundle protocol acts together 
    /// based on their start/stop times
    /// </summary>
    public class SimpleCarePlanService : ICarePlanService
    {

        // Protocols 
        private List<IClinicalProtocol> m_protocols = new List<IClinicalProtocol>();

        /// <summary>
        /// Constructs the aggregate care planner
        /// </summary>
        public SimpleCarePlanService()
        {
        }

        /// <summary>
        /// Gets the protocols
        /// </summary>
        public List<IClinicalProtocol> Protocols
        {
            get
            {
                int c;
                var repo = ApplicationServiceContext.Current.GetService(typeof(IClinicalProtocolRepositoryService)) as IClinicalProtocolRepositoryService;
                var protos = repo.FindProtocol(o => !o.ObsoletionTime.HasValue, 0, null, out c).ToArray();
                foreach (var proto in protos)
                {
                    // First , do we already have this?
                    if (!this.m_protocols.Any(p => p.Id == proto.Key))
                    {
                        var protocolClass = Activator.CreateInstance(proto.HandlerClass) as IClinicalProtocol;
                        protocolClass.Load(proto);
                        this.m_protocols.Add(protocolClass);
                    }
                }
                return this.m_protocols;
            }
        }

        /// <summary>
        /// Create a care plan for the specified patient
        /// </summary>
        public IEnumerable<Act> CreateCarePlan(Patient p)
        {
            return this.CreateCarePlan(p, false);
        }


        /// <summary>
        /// Create a care plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<Act> CreateCarePlan(Patient p, bool asEncounters)
        {
            return this.CreateCarePlan(p, asEncounters, this.Protocols.Select(o=>o.Id).ToArray());
        }

        /// <summary>
        /// Create a care plan with the specified protocols only
        /// </summary>
        public IEnumerable<Act> CreateCarePlan(Patient p, bool asEncounters, params Guid[] protocols)
        {
            // We want to flatten the patient's encounters 
            p = p.Clone() as Patient;
            p.Participations = new List<ActParticipation>(p.Participations);
            // The record target here is also a record target for any relationships
            p.Participations = p.Participations.Union(p.Participations.SelectMany(pt => pt.Act.Relationships.Select(r => new ActParticipation(ActParticipationKey.RecordTarget, p) { Act = r.TargetAct, ParticipationRole = new Model.DataTypes.Concept() { Mnemonic = "RecordTarget", Key = ActParticipationKey.RecordTarget } }))).ToList();
            List<Act> protocolActs = this.Protocols.Where(o=>protocols.Contains(o.Id)).OrderBy(o => o.Name).AsParallel().SelectMany(o => o.Calculate(p)).OrderBy(o => o.StopTime - o.StartTime).ToList();

            if (asEncounters)
            {
                List<PatientEncounter> encounters = new List<PatientEncounter>();
                foreach (var act in new List<Act>(protocolActs).Where(o => o.StartTime.HasValue && o.StopTime.HasValue).OrderBy(o => o.StartTime))
                {
                    // Is there a candidate encounter which is bound by start/end
                    var candidate = encounters.FirstOrDefault(e => (act.StartTime ?? DateTimeOffset.MinValue) <= (e.StopTime ?? DateTimeOffset.MaxValue) && (act.StopTime ?? DateTimeOffset.MaxValue) >= (e.StartTime ?? DateTimeOffset.MinValue));

                    // Create candidate
                    if (candidate == null)
                    {
                        candidate = this.CreateEncounter(act, p);
                        encounters.Add(candidate);
                        protocolActs.Add(candidate);
                    }
                    else
                    {
                        TimeSpan[] overlap = {
                            (candidate.StopTime ?? DateTimeOffset.MaxValue) - (candidate.StartTime ?? DateTimeOffset.MinValue),
                            (candidate.StopTime ?? DateTimeOffset.MaxValue) - (act.StartTime ?? DateTimeOffset.MinValue),
                            (act.StopTime ?? DateTimeOffset.MaxValue) - (candidate.StartTime ?? DateTimeOffset.MinValue),
                            (act.StopTime ?? DateTimeOffset.MaxValue) - (act.StartTime ?? DateTimeOffset.MinValue)
                        };
                        // find the minimum overlap
                        var minOverlap = overlap.Min();
                        var overlapMin = Array.IndexOf(overlap, minOverlap);
                        // Adjust the dates based on the start / stop time
                        if (overlapMin % 2 == 1)
                            candidate.StartTime = act.StartTime;
                        if (overlapMin > 1)
                            candidate.StopTime = act.StopTime;
                        candidate.ActTime = candidate.StartTime ?? candidate.ActTime;
                    }

                    // Add the protocol act
                    candidate.Relationships.Add(new ActRelationship(ActRelationshipTypeKeys.HasComponent, act));

                    // Remove so we don't have duplicates
                    protocolActs.Remove(act);
                    p.Participations.RemoveAll(o => o.Act == act);

                }

                // for those acts which do not have a stop time, schedule them in the first appointment available
                foreach (var act in new List<Act>(protocolActs).Where(o => !o.StopTime.HasValue))
                {
                    var candidate = encounters.OrderBy(o => o.StartTime).FirstOrDefault(e => e.StartTime >= act.StartTime);
                    if (candidate == null)
                    {
                        candidate = this.CreateEncounter(act, p);
                        encounters.Add(candidate);
                        protocolActs.Add(candidate);
                    }
                    // Add the protocol act
                    candidate.Relationships.Add(new ActRelationship(ActRelationshipTypeKeys.HasComponent, act));

                    // Remove so we don't have duplicates
                    protocolActs.Remove(act);
                    p.Participations.RemoveAll(o => o.Act == act);
                }

            }

            // TODO: Configure for days of week
            foreach (var itm in protocolActs)
                while (itm.ActTime.DayOfWeek == DayOfWeek.Sunday || itm.ActTime.DayOfWeek == DayOfWeek.Saturday)
                    itm.ActTime = itm.ActTime.AddDays(1);

            return protocolActs.ToList();
        }

        /// <summary>
        /// Create an encounter
        /// </summary>
        private PatientEncounter CreateEncounter(Act act, Patient recordTarget)
        {
            var retVal = new PatientEncounter()
            {
                Participations = new List<ActParticipation>()
                        {
                            new ActParticipation(ActParticipationKey.RecordTarget, recordTarget.Key)
                        },
                ActTime = act.ActTime,
                StartTime = act.StartTime,
                StopTime = act.StopTime,
                MoodConceptKey = ActMoodKeys.Propose,
                Key = Guid.NewGuid()
            };
            recordTarget.Participations.Add(new ActParticipation()
            {
                ParticipationRoleKey = ActParticipationKey.RecordTarget,
                Act = retVal
            });
            return retVal;
        }
    }
}
