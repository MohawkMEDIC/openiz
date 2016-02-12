using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.OAuth2.Wcf
{
    /// <summary>
    /// Authorization policy
    /// </summary>
    public class ClientAuthorizationPolicy : IAuthorizationPolicy
    {
        // ID
        private readonly Guid m_id = Guid.NewGuid();

        // Trace source
        private TraceSource m_traceSource = new TraceSource(OAuthConstants.TraceSourceName);

        /// <summary>
        /// Return ID
        /// </summary>
        public string Id
        {
            get
            {
                return this.m_id.ToString();
            }
        }
        
        /// <summary>
        /// Issuer of the claim
        /// </summary>
        public ClaimSet Issuer
        {
            get
            {
                return ClaimSet.System;
            }
        }

        /// <summary>
        /// Evaluate the context
        /// </summary>
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {

            try
            {
                this.m_traceSource.TraceInformation("Entering OAuth2.Wcf.ClientAuthorizationPolicy");

                object obj;
                if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
                    throw new Exception("No Identity found");
                IList<IIdentity> identities = obj as IList<IIdentity>;
                if (identities == null || identities.Count <= 0)
                    throw new Exception("No Identity found");

                evaluationContext.Properties["Principal"] = new GenericPrincipal(identities[0], null);
                return true;
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                throw;
            }
        }
    }
}
