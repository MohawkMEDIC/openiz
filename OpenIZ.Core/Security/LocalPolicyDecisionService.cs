/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-19
 */
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
            var pip = ApplicationContext.Current.GetService<IPolicyInformationService>();

            // Policies
            var activePolicies = pip.GetActivePolicies(principal).Where(o => policyId.StartsWith(o.Policy.Oid));
            // Most restrictive
            IPolicyInstance policyInstance = null;
            foreach (var pol in activePolicies)
                if (policyInstance == null)
                    policyInstance = pol;
                else if (pol.Rule < policyInstance.Rule)
                    policyInstance = pol;

            if(policyInstance == null)
            {
                // TODO: Configure OptIn or OptOut
                return PolicyDecisionOutcomeType.Deny;
            }
            else if (!policyInstance.Policy.CanOverride && policyInstance.Rule == PolicyDecisionOutcomeType.Elevate)
                return PolicyDecisionOutcomeType.Deny;
            else if (!policyInstance.Policy.IsActive)
                return PolicyDecisionOutcomeType.Grant;
            else if ((policyInstance.Policy as ILocalPolicy)?.Handler != null)
            {
                IPolicyHandler handlerInstance = null;
                var policy = policyInstance.Policy as ILocalPolicy;
                if(policy != null)
                    return policy.Handler.GetPolicyDecision(principal, policy, null).Outcome;

            }
            return policyInstance.Rule;
            
        }
    }
}
