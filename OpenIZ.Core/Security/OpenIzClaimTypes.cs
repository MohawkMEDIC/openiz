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
    /// OpenIZ Claim Types
    /// </summary>
    public static class OpenIzClaimTypes
    {

        /// <summary>
        /// Granted policy claim
        /// </summary>
        public const string OpenIzGrantedPolicyClaim = "http://openiz.org/claims/grant";

        /// <summary>
        /// Device identifier claim
        /// </summary>
        public const string OpenIzDeviceIdentifierClaim = "http://openiz.org/claims/device-Ref";

        /// <summary>
        /// Identifier of the application
        /// </summary>
        public const string OpenIzApplicationIdentifierClaim = "http://openiz.org/claims/application-Ref";

        /// <summary>
        /// Patient identifier claim
        /// </summary>
        public const string XUAPatientIdentifierClaim = "urn:oasis:names:tc:xacml:2.0:resource:resource-Ref";
        /// <summary>
        /// Purpose of use claim
        /// </summary>
        public const string XspaPurposeOfUseClaim = "urn:oasis:names:tc:xacml:2.0:action:purpose";
        /// <summary>
        /// Purpose of use claim
        /// </summary>
        public const string XspaUserRoleClaim = "urn:oasis:names:tc:xacml:2.0:subject:role";
        /// <summary>
        /// Facility id claim
        /// </summary>
        public const string XspaFacilityUrlClaim = "urn:oasis:names:tc:xspa:1.0: subject:facility";
        /// <summary>
        /// Organization name claim
        /// </summary>
        public const string XspaOrganizationNameClaim = "urn:oasis:names:tc:xspa:1.0: subject:organization-Ref";
        /// <summary>
        /// User identifier claim
        /// </summary>
        public const string XspaUserIdentifierClaim = "urn:oasis:names:tc:xacml:1.0: subject:subject-Ref";
    }
}
