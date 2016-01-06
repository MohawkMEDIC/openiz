using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a simply security policy
    /// </summary>
    public class SecurityPolicy : BaseData
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

    }

    /// <summary>
    /// Represents a security policy instance
    /// </summary>
    public class SecurityPolicyInstance : SecurityPolicy
    {

        /// <summary>
        /// Gets or sets whether the policy is a Deny
        /// </summary>
        public bool IsDeny { get; set; }

    }
}
