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
    public static class PermissionPolicyIdentifiers
    {
        /// <summary>
        /// Access administrative function
        /// </summary>
        public const string AccessAdministrativeFunction = "1.3.6.1.4.1.33349.3.5.9.2.0";

        /// <summary>
        /// Policy identifier for allowance of changing passwords
        /// </summary>
        /// TODO: Affix the mohawk college OID for this
        public const string ChangePassword = "1.3.6.1.4.1.33349.3.5.9.2.0.1";

        /// <summary>
        /// Whether the user can create roles
        /// </summary>
        public const string CreateRoles = "1.3.6.1.4.1.33349.3.5.9.2.0.2";

        /// <summary>
        /// Policy identifier for allowance of altering passwords
        /// </summary>
        public const string AlterRoles = "1.3.6.1.4.1.33349.3.5.9.2.0.3";

        /// <summary>
        /// Policy identifier for allowing of creating new identities
        /// </summary>
        public const string CreateIdentity = "1.3.6.1.4.1.33349.3.5.9.2.0.4";

        /// <summary>
        /// Policy identifier for allowance of login
        /// </summary>
        public const string Login = "1.3.6.1.4.1.33349.3.5.9.2.1";

        /// <summary>
        /// Access clinical data permission 
        /// </summary>
        public const string UnrestrictedClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.2";

        /// <summary>
        /// Query clinical data
        /// </summary>
        public const string QueryClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.2.0";

        /// <summary>
        /// Write clinical data
        /// </summary>
        public const string WriteClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.2.1";

        /// <summary>
        /// Delete clinical data
        /// </summary>
        public const string DeleteClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.2.2";

        /// <summary>
        /// Read clinical data
        /// </summary>
        public const string ReadClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.2.3";

        /// <summary>
        /// Indicates the user can elevate themselves (Break the glass)
        /// </summary>
        public const string ElevateClinicalData = "1.3.6.1.4.1.33349.3.5.9.2.3";
    }
}
