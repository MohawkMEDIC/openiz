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
 * Date: 2016-1-25
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Attribute
{
    /// <summary>
    /// Represents a security attribute which requires that a user be in the possession of a 
    /// particular claim
    /// </summary>
    public class PolicyPermissionAttribute : CodeAccessSecurityAttribute
    {

        // Security
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Core.Security");

        /// <summary>
        /// Creates a policy permission attribute
        /// </summary>
        public PolicyPermissionAttribute(SecurityAction action, String policyId) : base(action)
        {
            this.PolicyId = policyId;
        }

        /// <summary>
        /// The claim type which the user must 
        /// </summary>
        public String PolicyId { get; set; }

        /// <summary>
        /// Permission 
        /// </summary>
        public override IPermission CreatePermission()
        {
            var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();
            var currentPrincipal = System.Threading.Thread.CurrentPrincipal;

            var action = pdp.GetPolicyOutcome(currentPrincipal, this.PolicyId);

            this.m_traceSource.TraceInformation("Policy Enforce: {0}({1}) = {2}", currentPrincipal.Identity.Name, this.PolicyId, action);

            return action == PolicyDecisionOutcomeType.Grant ?
                new PrincipalPermission(PermissionState.Unrestricted) :
                new PrincipalPermission(PermissionState.None);
        }
    }
}
