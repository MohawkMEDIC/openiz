using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Local policy enforcement service
    /// </summary>
    public class LocalPolicyEnforcementService : IDaemonService
    {

        // Policy tracer
        private TraceSource m_tracer = new TraceSource("OpenIZ.Core.Security.Policy");
        
        // Attached and receiving events?
        private bool m_attached = false;

        /// <summary>
        /// Gets whether the daemon is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_attached;
            }
        }

        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            var conceptPersister = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            conceptPersister.Queried += ConceptPersister_Queried;

            this.Started?.Invoke(this, EventArgs.Empty);

            return true;
        }

        /// <summary>
        /// PEP listener for disclosure of concepts
        /// </summary>
        private void ConceptPersister_Queried(object sender, MARC.HI.EHRS.SVC.Core.Event.PostQueryEventArgs<Concept> e)
        {
            var pip = ApplicationContext.Current.GetService<IPolicyInformationService>();
            var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            // First, ask the pdp to get the policy decision with the current principal
            var denyOrElevate = e.Results.Select(o => pdp.GetPolicyDecision(e.Principal, o)).Where(
                d=>d.Outcome != PolicyDecisionOutcomeType.Grant
            );

            // Deny or elevate decision... ugh.. we have to do work
            if (denyOrElevate.Count() > 0)
            {
                // Exceptions
                List<Concept> exceptions = new List<Concept>();
                foreach(var dp in denyOrElevate)
                {
                    switch(dp.Outcome)
                    {
                        case PolicyDecisionOutcomeType.Deny:
                            exceptions.Add(dp.Securable as Concept); // TODO: Should I audit this?
                            break;
                        case PolicyDecisionOutcomeType.Elevate:
                            var elevateDecisions = dp.Details.Where(o => o.Outcome == PolicyDecisionOutcomeType.Elevate);
                            if (!elevateDecisions.Any(o => pip.GetPolicy(o.PolicyId).CanOverride))
                                exceptions.Add(dp.Securable as Concept);
                            else
                                (dp.Securable as Concept).Mask();// Just let them know it is masked
                            break;
                    }
                }
                // Remove or "except" those in the exception list
                e.Except(exceptions);
            }
            else // Nothing to do
            {
                // 🎶 Now it's just my luck to have the watch with nothing left to do...
                // Than watch the deadly waters glide as we roll north to the Sault....
                // And wonder when they'll turn again and pitch us to the rail...
                // And whirl off one more youngster in the gail.. 🎶
                this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "PEP: No policy enforcement done on query {0}", e.Query);
                // 🎶 And I told that kid a 100 time not to take the lakes for granted
                // They go from calm to 100 knots so fast they seem enchated... 🎶
            }

        }
        

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
