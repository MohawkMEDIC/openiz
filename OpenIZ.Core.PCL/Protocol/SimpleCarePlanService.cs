/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a care plan service that can bundle protocol acts together 
    /// based on their start/stop times
    /// </summary>
    public class SimpleCarePlanService : ICarePlanService
    {

        /// <summary>
        /// True if the view model initializer for the care plans should be ignored
        /// </summary>
        public bool IgnoreViewModelInitializer { get; set; }

        /// <summary>
        /// Represents a parameter dictionary
        /// </summary>
        public class ParameterDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class
        {

            /// <summary>
            /// Add new item key
            /// </summary>
            public new void Add(TKey key, TValue value)
            {
                if (value == null)
                {
                    // adding null value is pointless...
                    return;
                }
                base.Add(key, value);
            }

            /// <summary>
            /// Remove key
            /// </summary>
            /// <param name="key"></param>
            public new void Remove(TKey key)
            {
                if (!ContainsKey(key))
                {
                    // nothing to do
                    return;
                }
                base.Remove(key);
            }

            /// <summary>
            /// Indexer
            /// </summary>
            public new TValue this[TKey key]
            {
                get
                {
                    TValue value;
                    return TryGetValue(key, out value) ? value : null;
                }
                set
                {
                    if (value == null)
                    {
                        // setting value null is same as removing it
                        Remove(key);
                    }
                    else
                    {
                        base[key] = value;
                    }
                }
            }
        }

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SimpleCarePlanService));
        // Protocols 
        private List<IClinicalProtocol> m_protocols = new List<IClinicalProtocol>();
        // Care plan loading promise dictionary (prevents double-loading of patients)
        private Dictionary<Guid, Patient> m_patientPromise = new Dictionary<Guid, Patient>();

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
        public CarePlan CreateCarePlan(Patient p)
        {
            return this.CreateCarePlan(p, false, null);
        }

        /// <summary>
        /// Create a care plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public CarePlan CreateCarePlan(Patient p, bool asEncounters)
        {
            return this.CreateCarePlan(p, asEncounters, null, this.Protocols.Select(o => o.Id).ToArray());
        }

        /// <summary>
        /// Create a care plan
        /// </summary>
        public CarePlan CreateCarePlan(Patient p, bool asEncounters, IDictionary<String, Object> parameters)
        {
            return this.CreateCarePlan(p, asEncounters, parameters, this.Protocols.Select(o => o.Id).ToArray());
        }
        /// <summary>
        /// Create a care plan with the specified protocols only
        /// </summary>
        public CarePlan CreateCarePlan(Patient p, bool asEncounters, IDictionary<String, Object> parameters, params Guid[] protocols)
        {
            if (p == null) return null;

            try
            {
                var parmDict = new ParameterDictionary<String, Object>();
                if(parameters != null)
                    foreach (var itm in parameters)
                        parmDict.Add(itm.Key, itm.Value);

                // Allow each protocol to initialize itself
                var execProtocols = this.Protocols.Where(o => protocols.Contains(o.Id)).OrderBy(o => o.Name).Distinct().ToList();

                Patient currentProcessing = null;
                bool isCurrentProcessing = false;
                if(p.Key.HasValue)
                    isCurrentProcessing = this.m_patientPromise.TryGetValue(p.Key.Value, out currentProcessing);
                if (p.Key.HasValue && !isCurrentProcessing)
                {
                    lock (this.m_patientPromise)
                        if (!this.m_patientPromise.TryGetValue(p.Key.Value, out currentProcessing))
                        {
                            currentProcessing = p.Copy() as Patient;

                            // Are the participations of the patient null?
                            if (p.Participations.Count == 0 && p.VersionKey.HasValue)
                            {
                                p.Participations = EntitySource.Current.Provider.Query<Act>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key) &&
                                    o.StatusConceptKey != StatusKeys.Nullified && o.StatusConceptKey != StatusKeys.Obsolete && o.StatusConceptKey != StatusKeys.Cancelled).OfType<Act>()
                                    .Select(a =>
                                    new ActParticipation()
                                    {
                                        Act = a,
                                        ParticipationRole = new Concept() { Mnemonic = "RecordTarget" },
                                        PlayerEntity = currentProcessing
                                    }).ToList();

                                //EntitySource.Current.Provider.Query<SubstanceAdministration>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key)).OfType<Act>()
                                //    .Union(EntitySource.Current.Provider.Query<QuantityObservation>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key))).OfType<Act>()
                                //    .Union(EntitySource.Current.Provider.Query<CodedObservation>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key))).OfType<Act>()
                                //    .Union(EntitySource.Current.Provider.Query<TextObservation>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key))).OfType<Act>()
                                //    .Union(EntitySource.Current.Provider.Query<PatientEncounter>(o => o.Participations.Where(g => g.ParticipationRole.Mnemonic == "RecordTarget").Any(g => g.PlayerEntityKey == currentProcessing.Key))).OfType<Act>()
                                    
                                (ApplicationServiceContext.Current.GetService(typeof(IDataCachingService)) as IDataCachingService)?.Add(p);
                            }
                            currentProcessing.Participations = new List<ActParticipation>(p.Participations);

                            // The record target here is also a record target for any /relationships
                            // TODO: I think this can be removed no?
                            //currentProcessing.Participations = currentProcessing.Participations.Union(currentProcessing.Participations.SelectMany(pt =>
                            //{
                            //    if (pt.Act == null)
                            //        pt.Act = EntitySource.Current.Get<Act>(pt.ActKey);
                            //    return pt.Act?.Relationships?.Select(r =>
                            //    {
                            //        var retVal = new ActParticipation(ActParticipationKey.RecordTarget, currentProcessing)
                            //        {
                            //            ActKey = r.TargetActKey,
                            //            ParticipationRole = new Model.DataTypes.Concept() { Mnemonic = "RecordTarget", Key = ActParticipationKey.RecordTarget }
                            //        };
                            //        if (r.TargetAct != null)
                            //            retVal.Act = r.TargetAct;
                            //        else
                            //        {
                            //            retVal.Act = currentProcessing.Participations.FirstOrDefault(o=>o.ActKey == r.TargetActKey)?.Act ?? EntitySource.Current.Get<Act>(r.TargetActKey);
                            //        }
                            //        return retVal;
                            //    }
                            //    );
                            //})).ToList();

                            // Add to the promised patient
                            this.m_patientPromise.Add(p.Key.Value, currentProcessing);

                        }
                }
                else if (!p.Key.HasValue) // Not persisted
                    currentProcessing = p.Clone() as Patient;

                // Initialize for protocol execution
                parmDict.Add("runProtocols", execProtocols.Distinct());
                if(!this.IgnoreViewModelInitializer)
                    foreach (var o in this.Protocols.Distinct()) o.Initialize(currentProcessing, parmDict);

                parmDict.Remove("runProtocols");

                List<Act> protocolActs = new List<Act>();
                lock (currentProcessing)
                {
                    var thdPatient = currentProcessing.Copy() as Patient;
                    thdPatient.Participations = new List<ActParticipation>(currentProcessing.Participations.ToList().Where(o=>o.Act?.MoodConceptKey != ActMoodKeys.Propose && o.Act?.StatusConceptKey != StatusKeys.Nullified && o.Act?.StatusConceptKey != StatusKeys.Obsolete && o.Act?.StatusConceptKey != StatusKeys.Cancelled));

                    // Let's ensure that there are some properties loaded eh?
                    if(this.IgnoreViewModelInitializer)
                        foreach(var itm in thdPatient.LoadCollection<ActParticipation>("Participations"))
                        {
                            itm.LoadProperty<Act>("TargetAct").LoadProperty<Concept>("TypeConcept");
                            foreach (var itmPtcpt in itm.LoadProperty<Act>("TargetAct").LoadCollection<ActParticipation>("Participations"))
                            {
                                itmPtcpt.LoadProperty<Concept>("ParticipationRole");
                                itmPtcpt.LoadProperty<Entity>("PlayerEntity").LoadProperty<Concept>("TypeConcept");
                                itmPtcpt.LoadProperty<Entity>("PlayerEntity").LoadProperty<Concept>("MoodConcept");
                            };
                        }
                    protocolActs = execProtocols.AsParallel().SelectMany(o => o.Calculate(thdPatient, parmDict)).OrderBy(o => o.StopTime - o.StartTime).ToList();
                }

                // Current processing 
                if (asEncounters)
                {
                    List<PatientEncounter> encounters = new List<PatientEncounter>();
                    foreach (var act in new List<Act>(protocolActs).Where(o => o.StartTime.HasValue && o.StopTime.HasValue).OrderBy(o => o.StartTime).OrderBy(o=>(o.StopTime ?? o.ActTime.AddDays(7)) - o.StartTime))
                    {

                        act.StopTime = act.StopTime ?? act.ActTime;
                        // Is there a candidate encounter which is bound by start/end
                        var candidate = encounters.FirstOrDefault(e => (act.StartTime ?? DateTimeOffset.MinValue) <= (e.StopTime ?? DateTimeOffset.MaxValue) 
                            && (act.StopTime ?? DateTimeOffset.MaxValue) >= (e.StartTime ?? DateTimeOffset.MinValue)
                            && !e.Relationships.Any(r=>r.TargetAct?.Protocols.Intersect(act.Protocols, new ProtocolComparer()).Count() == r.TargetAct?.Protocols.Count())
                        );

                        // Create candidate
                        if (candidate == null)
                        {
                            candidate = this.CreateEncounter(act, currentProcessing);
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
                        currentProcessing.Participations.RemoveAll(o => o.Act == act);

                    }

                    // for those acts which do not have a stop time, schedule them in the first appointment available
                    foreach (var act in new List<Act>(protocolActs).Where(o => !o.StopTime.HasValue))
                    {
                        var candidate = encounters.OrderBy(o => o.StartTime).FirstOrDefault(e => e.StartTime >= act.StartTime);
                        if (candidate == null)
                        {
                            candidate = this.CreateEncounter(act, currentProcessing);
                            encounters.Add(candidate);
                            protocolActs.Add(candidate);
                        }
                        // Add the protocol act
                        candidate.Relationships.Add(new ActRelationship(ActRelationshipTypeKeys.HasComponent, act));

                        // Remove so we don't have duplicates
                        protocolActs.Remove(act);
                        currentProcessing.Participations.RemoveAll(o => o.Act == act);
                    }

                }

                // TODO: Configure for days of week
                foreach (var itm in protocolActs)
                    while (itm.ActTime.DayOfWeek == DayOfWeek.Sunday || itm.ActTime.DayOfWeek == DayOfWeek.Saturday)
                        itm.ActTime = itm.ActTime.AddDays(1);

                return new CarePlan(p, protocolActs.ToList())
                {
                    CreatedByKey = Guid.Parse("fadca076-3690-4a6e-af9e-f1cd68e8c7e8")
                };
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error creating care plan: {0}", e);
                throw;
            }
            finally
            {
                lock (m_patientPromise)
                    if(p.Key.HasValue && this.m_patientPromise.ContainsKey(p.Key.Value))
                        m_patientPromise.Remove(p.Key.Value);
            }
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
