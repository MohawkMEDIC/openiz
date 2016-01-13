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
        public const string OpenIzDeviceIdentifierClaim = "http://openiz.org/claims/device-id";

        /// <summary>
        /// Identifier of the application
        /// </summary>
        public const string OpenIzApplicationIdentifierClaim = "http://openiz.org/claims/application-id";

        /// <summary>
        /// Patient identifier claim
        /// </summary>
        public const string XUAPatientIdentifierClaim = "urn:oasis:names:tc:xacml:2.0:resource:resource-id";
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
        public const string XspaOrganizationNameClaim = "urn:oasis:names:tc:xspa:1.0: subject:organization-id";
        /// <summary>
        /// User identifier claim
        /// </summary>
        public const string XspaUserIdentifierClaim = "urn:oasis:names:tc:xacml:1.0: subject:subject-id";
    }
}
