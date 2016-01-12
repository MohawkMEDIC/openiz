using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Security Entity base class
    /// </summary>
    public abstract class SecurityEntity : VersionedBaseData
    {

        // Policies
        private List<SecurityPolicyInstance> m_policies = new List<SecurityPolicyInstance>();

        /// <summary>
        /// Policies associated with the entity
        /// </summary>
        public virtual List<SecurityPolicyInstance> Policies { get { return this.m_policies; } }
    }
}
