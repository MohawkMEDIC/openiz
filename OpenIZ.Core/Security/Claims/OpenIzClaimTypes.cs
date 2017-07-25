/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Claims
{
    /// <summary>
    /// OpenIZ Claim Types
    /// </summary>
    public static class OpenIzClaimTypes
    {

        /// <summary>
        /// Static ctor
        /// </summary>
        static OpenIzClaimTypes()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    foreach (var t in asm.GetTypes().Where(o => typeof(IClaimTypeHandler).IsAssignableFrom(o) && o.IsClass))
                    {
                        IClaimTypeHandler handler = t.GetConstructor(Type.EmptyTypes).Invoke(null) as IClaimTypeHandler;
                        s_claimHandlers.Add(handler.ClaimType, handler);
                    }
                }
                catch { }
        }

        /// <summary>
        /// A list of claim handlers
        /// </summary>
        private static Dictionary<String, IClaimTypeHandler> s_claimHandlers = new Dictionary<string, IClaimTypeHandler>();

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
        /// Secret claim
        /// </summary>
        public const string OpenIZTfaSecretClaim = "http://openiz.org/claims/tfa-secret";

        /// <summary>
        /// TFA secret expiry
        /// </summary>
        public const string OpenIZTfaSecretExpiry = "http://openiz.org/claims/tfa-secret-expiry";

        /// <summary>
        /// TFA secret expiry
        /// </summary>
        public const string OpenIzScopeClaim = "http://openiz.org/claims/scope";


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
        public const string XspaFacilityUrlClaim = "urn:oasis:names:tc:xspa:1.0:subject:facility";
        /// <summary>
        /// Organization name claim
        /// </summary>
        public const string XspaOrganizationNameClaim = "urn:oasis:names:tc:xspa:1.0:subject:organization-id";
        /// <summary>
        /// User identifier claim
        /// </summary>
        public const string XspaUserIdentifierClaim = "urn:oasis:names:tc:xacml:1.0:subject:subject-id";

        /// <summary>
        /// Passwordless authentication (allow authentication without password)
        /// </summary>
        public const string OpenIZPasswordlessAuth = "http://openiz.org/claims/auth-nopassword";

        /// <summary>
        /// Gets the specified claim type handler
        /// </summary>
        public static IClaimTypeHandler GetHandler(String claimType)
        {
            IClaimTypeHandler handler = null;
            s_claimHandlers.TryGetValue(claimType, out handler);
            return handler;
        }

        /// <summary>
        /// Extract claims
        /// </summary>
        public static List<Claim> ExtractClaims(NameValueCollection headers)
        {
            var claimsHeaders = headers[OpenIzConstants.BasicHttpClientClaimHeaderName];
            if (claimsHeaders == null)
                return new List<Claim>();
            else
                return claimsHeaders.Split(',').Select(o => Encoding.UTF8.GetString(Convert.FromBase64String(o)).Split('=')).Select(c => new Claim(c[0], c[1])).ToList();
        } 
    }
}
