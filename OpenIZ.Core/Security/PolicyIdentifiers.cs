/**
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
