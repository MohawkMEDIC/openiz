using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Claim types
    /// </summary>
    public static class PolicyIdentifiers
    {

        /// <summary>
        /// Policy identifier for allowance of changing passwords
        /// </summary>
        /// TODO: Affix the mohawk college OID for this
        public const string OpenIzChangePasswordPolicy = "1.1.1.1.1.1.1.1.1.1";

        /// <summary>
        /// Policy identifier for allowance of login
        /// </summary>
        public const string OpenIzLoginPolicy = "1.1.1.1.1.1.1.1.2";

        /// <summary>
        /// Whether the user can create roles
        /// </summary>
        public const string OpenIzCreateRolesPolicy = "1.1.1.1.1.1.1.1.3";

        /// <summary>
        /// Policy identifier for allowance of altering passwords
        /// </summary>
        public const string OpenIzAlterRolePolicy = "1.1.1.1.1.1.1.1.1.4";

        /// <summary>
        /// Policy identifier for allowing of creating new identities
        /// </summary>
        public const string OpenIzCreateIdentityPolicy = "1.1.1.1.1.1.1.1.1.5";
    }
}
