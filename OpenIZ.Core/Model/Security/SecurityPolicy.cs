using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core;
using System.ComponentModel;
using MARC.HI.EHRS.SVC.Core.Services.Policy;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a simply security policy
    /// </summary>
    public class SecurityPolicy : BaseData, IPolicy
    {
        
        /// <summary>
        /// Gets or sets the handler which may handle this policy
        /// </summary>
        public Type Handler { get; set; }

        /// <summary>
        /// Gets or sets the name of the policy
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the universal ID
        /// </summary>
        public String Oid { get; set; }

        /// <summary>
        /// Whether the property is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the policy can be elevated over
        /// </summary>
        public bool CanElevate { get; set; }
    }

    /// <summary>
    /// Represents a security policy instance
    /// </summary>
    public class SecurityPolicyInstance 
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public SecurityPolicyInstance()
        {

        }

        /// <summary>
        /// Creates a new policy instance with the specified policy and grant
        /// </summary>
        public SecurityPolicyInstance(SecurityPolicy policy, PolicyDecisionOutcomeType grantType)
        {
            this.Policy = policy;
            this.GrantType = grantType;
        }

        /// <summary>
        /// The policy
        /// </summary>
        public SecurityPolicy Policy { get; set; }

        /// <summary>
        /// Gets or sets whether the policy is a Deny
        /// </summary>
        public PolicyDecisionOutcomeType GrantType { get; set; }

    }
}
