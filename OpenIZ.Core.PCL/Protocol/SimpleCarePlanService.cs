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

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a care plan service that can bundle protocol acts together 
    /// based on their start/stop times
    /// </summary>
    public class SimpleCarePlanService : ICarePlanService
    {

        // Group as appointments
        private bool m_groupAsAppointments = true;

        // Protocols 
        private List<IClinicalProtocol> m_protocols = new List<IClinicalProtocol>();

        /// <summary>
        /// Constructs the aggregate care planner
        /// </summary>
        public SimpleCarePlanService()
        {
        }

        /// <summary>
        /// Simple care plan services
        /// </summary>
        public SimpleCarePlanService(bool groupAppointments)
        {
            this.m_groupAsAppointments = groupAppointments;
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
                foreach (var proto in repo.FindProtocol(o => !o.ObsoletionTime.HasValue, 0, null, out c))
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
        /// Create a care plan
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public IEnumerable<Act> CreateCarePlan(Patient p)
        {
            List<Act> protocolActs = this.Protocols.OrderBy(o => o.Name).AsParallel().SelectMany(o => o.Calculate(p)).OrderBy(o=>o.StopTime - o.StartTime).ToList();

            if (this.m_groupAsAppointments)
            {
                List<PatientEncounter> encounters = new List<PatientEncounter>();
                foreach (var act in new List<Act>(protocolActs))
                {
                    // Is there a candidate encounter which is bound by start/end
                    var candidate = encounters.FirstOrDefault(e => e.ActTime >= act.StartTime && e.ActTime <= act.StopTime);

                    // Create candidate
                    if (candidate == null)
                    {
                        candidate = new PatientEncounter()
                        {
                            Participations = new List<ActParticipation>()
                        {
                            new ActParticipation(ActParticipationKey.RecordTarget, p.Key)
                        },
                            ActTime = act.ActTime,
                            StartTime = act.StartTime,
                            StopTime = act.StopTime,
                            MoodConceptKey = ActMoodKeys.Propose,
                            Key = Guid.NewGuid()
                        };
                        p.Participations.Add(new ActParticipation()
                        {
                            ParticipationRoleKey = ActParticipationKey.RecordTarget,
                            Act = candidate
                        });
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
            return protocolActs.ToList();
        }
    }
}
