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
using MARC.HI.EHRS.SVC.Core.Exceptions;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
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

        /// <summary>
        /// Creates a policy permission attribute
        /// </summary>
        public PolicyPermissionAttribute(SecurityAction action) : base(action)
        {
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
            
            // TODO: Configure this 
            if (ApplicationContext.Current.GetService(typeof(IPolicyDecisionService)) == null)
                return new PolicyPermission(PermissionState.Unrestricted, this.PolicyId);
            else
                return new PolicyPermission(PermissionState.None, this.PolicyId);

        }
    }

    /// <summary>
    /// A policy permission
    /// </summary>
    [Serializable]
    public class PolicyPermission : IPermission, IUnrestrictedPermission
    {

        // True if unrestricted
        private bool m_isUnrestricted;
        private String m_policyId;
        private IPrincipal m_principal;

        // Security
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Core.Security");

        /// <summary>
        /// Policy permission
        /// </summary>
        public PolicyPermission(PermissionState state, String policyId, IPrincipal principal)
        {
            this.m_isUnrestricted = state == PermissionState.Unrestricted;
            this.m_policyId = policyId;
            this.m_principal = principal;
        }

        /// <summary>
        /// Creates a new policy permission
        /// </summary>
        public PolicyPermission(PermissionState state, String policyId) : base()
        {
            this.m_isUnrestricted = state == PermissionState.Unrestricted;
            this.m_policyId = policyId;
            this.m_principal = AuthenticationContext.Current.Principal;
        }

        /// <summary>
        /// Copy the permission
        /// </summary>
        public IPermission Copy()
        {
            return new PolicyPermission(this.m_isUnrestricted ? PermissionState.Unrestricted : PermissionState.None, this.m_policyId, this.m_principal);
        }

        /// <summary>
        /// Demand the permission
        /// </summary>
        public void Demand()
        {
            var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();

            // Non system principals must be authenticated
            if(!this.m_principal.Identity.IsAuthenticated &&
                this.m_principal != AuthenticationContext.SystemPrincipal)
                throw new PolicyViolationException(this.m_policyId, PolicyDecisionOutcomeType.Deny);

            PolicyDecisionOutcomeType action = PolicyDecisionOutcomeType.Deny;
            if (pdp == null) // No way to verify 
                action = PolicyDecisionOutcomeType.Grant;
            else if (pdp != null)
                action = pdp.GetPolicyOutcome(this.m_principal, this.m_policyId);

            this.m_traceSource.TraceInformation("Policy Enforce: {0}({1}) = {2}", this.m_principal?.Identity?.Name, this.m_policyId, action);

            if (action != PolicyDecisionOutcomeType.Grant)
                throw new PolicyViolationException(this.m_policyId, action);
        }

        /// <summary>
        /// From XML
        /// </summary>
        public void FromXml(SecurityElement elem)
        {
            string element = elem.Attribute("Unrestricted");
            if (element != null)
                this.m_isUnrestricted = Convert.ToBoolean(element);
            element = elem.Attribute("PolicyId");
            if (element != null)
                this.m_policyId = element;
            element = elem.Attribute("principal");
            if(element != null)
                this.m_principal = new GenericPrincipal(ApplicationContext.Current.GetService<IIdentityProviderService>().GetIdentity(element), null);
            else
                throw new InvalidOperationException("Must have policyid");
        }

        /// <summary>
        /// Intersect the permission
        /// </summary>
        public IPermission Intersect(IPermission target)
        {
            if (target == null)
                return null;
            if ((target as IUnrestrictedPermission)?.IsUnrestricted() == false)
                return target;
            else
                return this.Copy();
        }

        /// <summary>
        /// If the two operations allow the exact set of operations
        /// </summary>
        public bool IsSubsetOf(IPermission target)
        {
            if (target == null)
                return !this.m_isUnrestricted;
            else
            {
                var permission = target as PolicyPermission;
                return permission.m_isUnrestricted == this.m_isUnrestricted &&
                    this.m_policyId.StartsWith(permission.m_policyId);
            }
        }

        /// <summary>
        /// True if the permission is unrestricted
        /// </summary>
        public bool IsUnrestricted()
        {
            return this.m_isUnrestricted;
        }

        /// <summary>
        /// Represent the element as XML
        /// </summary>
        public SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("IPermission");
            Type type = this.GetType();
            StringBuilder AssemblyName = new StringBuilder(type.Assembly.ToString());
            AssemblyName.Replace('\"', '\'');
            element.AddAttribute("class", type.FullName + ", " + AssemblyName);
            element.AddAttribute("version", "1");
            element.AddAttribute("Unrestricted", this.m_isUnrestricted.ToString());
            element.AddAttribute("Policy", this.m_policyId);
            element.AddAttribute("Principal", this.m_principal.Identity.Name);
            return element;

        }

        public IPermission Union(IPermission target)
        {
            throw new NotImplementedException();
        }
    }
}
