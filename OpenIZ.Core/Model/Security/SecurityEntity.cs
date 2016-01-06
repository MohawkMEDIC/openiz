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
    public class SecurityEntity : VersionedBaseData
    {

        /// <summary>
        /// Gets the list of security policies associated with the user
        /// </summary>
        public SecurityEntity()
        {
            this.Policies = new List<SecurityPolicyInstance>();
        }

        /// <summary>
        /// Security policies associated with the security entity
        /// </summary>
        public List<SecurityPolicyInstance> Policies { get; private set; }

    }
}
