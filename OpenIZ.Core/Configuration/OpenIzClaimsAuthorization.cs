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
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;

namespace OpenIZ.Core.Configuration
{
    /// <summary>
    /// Represents claims authorization configuration
    /// </summary>
    public class OpenIzClaimsAuthorization
    {

        /// <summary>
        /// Creates a new claims
        /// </summary>
        public OpenIzClaimsAuthorization()
        {
            this.Audiences = new ObservableCollection<string>();
            this.IssuerKeys = new Dictionary<string, SecurityKey>();
        }

        /// <summary>
        /// Custom validator type
        /// </summary>
        public Type CustomValidator { get; set; }

        /// <summary>
        /// Issuer keys
        /// </summary>
        public Dictionary<String, SecurityKey> IssuerKeys { get; set; }
        
        /// <summary>
        /// Gets or sets the allowed audiences 
        /// </summary>
        public ObservableCollection<String> Audiences { get; set; }

        /// <summary>
        /// Gets or sets the realm
        /// </summary>
        public string Realm { get; internal set; }


        /// <summary>
        /// Convert this to a STS handler config
        /// </summary>
        public TokenValidationParameters ToConfigurationObject()
        {

            TokenValidationParameters retVal = new TokenValidationParameters();

            retVal.ValidIssuers = this.IssuerKeys.Select(o => o.Key);
            retVal.RequireExpirationTime = true;
            retVal.RequireSignedTokens = true;
            retVal.ValidAudiences = this.Audiences;
            retVal.ValidateLifetime = true;
            retVal.ValidateIssuerSigningKey = true;
            retVal.ValidateIssuer = true;
            retVal.ValidateAudience = true;
            retVal.IssuerSigningTokens = this.IssuerKeys.Where(o => o.Value is X509SecurityKey).Select(o => new X509SecurityToken((o.Value as X509SecurityKey).Certificate));
            retVal.IssuerSigningKeys = this.IssuerKeys.Select(o => o.Value);
            retVal.IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
            {

                if (identifier.Count > 0)
                    return identifier.Select(o =>
                    {
                        // Lookup by thumbprint
                        SecurityKey candidateKey = null;

                        if (o is X509ThumbprintKeyIdentifierClause)
                                candidateKey = this.IssuerKeys.SingleOrDefault(ik => (ik.Value as X509SecurityKey).Certificate.Thumbprint == BitConverter.ToString((o as X509ThumbprintKeyIdentifierClause).GetX509Thumbprint()).Replace("-","")).Value;

                        return candidateKey;
                    }).First(o => o != null);
                else
                {
                    SecurityKey candidateKey = null;
                    this.IssuerKeys.TryGetValue((securityToken as JwtSecurityToken).Issuer, out candidateKey);
                    return candidateKey;
                }
            };
           
            // Custom validator
            if (this.CustomValidator != null)
            {
                ConstructorInfo ci = this.CustomValidator.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                    throw new ConfigurationException("No constructor found for custom validator");
                retVal.CertificateValidator = ci.Invoke(null) as X509CertificateValidator;
            }

            
            return retVal;
        }
    }
}