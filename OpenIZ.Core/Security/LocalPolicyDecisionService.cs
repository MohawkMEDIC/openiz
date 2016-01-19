using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Security.Claims;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Local policy decision service
    /// </summary>
    public class LocalPolicyDecisionService : IPolicyDecisionService
    {
        /// <summary>
        /// Get a policy decision 
        /// </summary>
        public PolicyDecision GetPolicyDecision(IPrincipal principal, object securable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a policy outcome
        /// </summary>
        public PolicyDecisionOutcomeType GetPolicyOutcome(IPrincipal principal, string policyId)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            else if (String.IsNullOrEmpty(policyId))
                throw new ArgumentNullException(nameof(policyId));

            // Can we make this decision based on the claims? 
            if (principal is ClaimsPrincipal && (principal as ClaimsPrincipal).HasClaim(c => c.Type == OpenIzClaimTypes.OpenIzGrantedPolicyClaim && c.Value == policyId))
                return PolicyDecisionOutcomeType.Grant;
            
            // Get the user object from the principal
            var userService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            var user = userService.Query(o => o.UserName == principal.Identity.Name, principal).FirstOrDefault();

            // Deny
            if (user == null)
                return PolicyDecisionOutcomeType.Deny;

            // Policies
            var policyInstance = user.Policies.Find(o => o.Policy.Oid == policyId);
            if (!policyInstance.Policy.CanOverride && policyInstance.GrantType == PolicyDecisionOutcomeType.Elevate)
                return PolicyDecisionOutcomeType.Deny;
            else if (policyInstance.Policy.ObsoletionTime != null)
                return PolicyDecisionOutcomeType.Deny;

            return policyInstance.GrantType;
            
        }
    }
}
