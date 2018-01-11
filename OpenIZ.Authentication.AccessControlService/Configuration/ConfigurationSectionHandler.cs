/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using OpenIZ.Core.Security;

namespace OpenIZ.Authentication.OAuth2.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// <openiz.authentication.oauth2>
    ///     <token expiry="" issuerName=""/>
    ///     <signature>
    ///         <!-- If using X509 RSA certificates -->
    ///         <certificate storeLocation="" storeName="" x509FindType="" findValue=""/>
    ///         <!-- If using symmetric key -->
    ///         <symmetric secret=""/>
    ///     </signature>
    ///     <claims>
    ///         <add claimType="claimName"/>
    ///     </claims>
    ///     <scopes>
    ///         <add name="scopeName"/>
    ///     </scopes>
    /// </openiz.authentication.oauth2>
    /// ]]>
    /// </remarks>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration handler
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            OAuthConfiguration retVal = new OAuthConfiguration();

            // Nodes
            XmlElement tokenElement = section.SelectSingleNode("./token") as XmlElement,
                x509Signature = section.SelectSingleNode("./signature/certificate") as XmlElement,
                symmSignature = section.SelectSingleNode("./signature/symmetric") as XmlElement;
            XmlNodeList claims = section.SelectNodes("./claims/add/@claimType"),
                scopes = section.SelectNodes("./scopes/add/@name");

            retVal.ValidityTime = TimeSpan.Parse(tokenElement?.Attributes["expiry"]?.Value ?? "00:00:10:00");
            retVal.IssuerName = tokenElement?.Attributes["issuerName"]?.Value ?? "http://localhost/oauth2_token";

            if (x509Signature != null)
            {
                retVal.Certificate = SecurityUtils.FindCertificate(x509Signature.Attributes["storeLocation"]?.Value,
                    x509Signature.Attributes["storeName"]?.Value,
                    x509Signature.Attributes["x509FindType"]?.Value,
                    x509Signature.Attributes["findValue"]?.Value);
            }
            else if (symmSignature != null)
            {
                retVal.ServerSecret = symmSignature.Attributes["secret"]?.Value;
                if(symmSignature.Attributes["key"] != null)
                    retVal.ServerKey = Convert.FromBase64String(symmSignature.Attributes["key"].Value);
                
            }
            else
                throw new ConfigurationErrorsException("One of certificate or symmetric key must be selected", null, section);

            // Claims
            if (claims != null)
                foreach (XmlNode itm in claims)
                    retVal.AllowedClientClaims.Add(itm.Value);
            if (scopes != null)
                foreach (XmlNode itm in scopes)
                    retVal.AllowedScopes.Add(itm.Value);

            // Validate the configuration
            retVal.Validate();

            return retVal;

        }
    }
}
