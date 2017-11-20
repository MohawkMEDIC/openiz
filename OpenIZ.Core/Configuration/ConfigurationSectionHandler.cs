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
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Core.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// <openiz.core>
    ///     <security>
    ///         <basic requireClientAuth="true" realm="">
    ///             <!-- Claims allowed to be made by clients on basic auth -->
    ///             <allowedClaims>
    ///                 <add claimType=""/>
    ///             </allowedClaims>
    ///         </basic>
    ///         <token realm="">
    ///             <audience>
    ///                 <add name=""/>
    ///             </audience>
    ///             <issuers customCertificateValidator="">
    ///                 <add name="issuerName" findValue="" storeLocation="" storeName="" x509FindType=""/>
    ///                 <add name="issuerName" symmetricKey=""/>
    ///             </issuers>
    ///         </token>
    ///     </security>
    /// </openiz.core>
    /// ]]>
    /// </remarks>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            OpenIzConfiguration retVal = new OpenIzConfiguration();

            // Nodes
            XmlElement securityNode = section.SelectSingleNode("./security") as XmlElement,
                threadingNode = section.SelectSingleNode("./threading") as XmlElement;

            retVal.ThreadPoolSize = Int32.Parse(threadingNode?.Attributes["poolSize"].Value ?? Environment.ProcessorCount.ToString());
            // Security?
            if (securityNode != null)
            {
                retVal.Security = new OpenIzSecurityConfiguration();

                XmlElement basicSecurityNode = securityNode.SelectSingleNode("./basic") as XmlElement,
                    tokenSecurityNode = securityNode.SelectSingleNode("./token") as XmlElement,
                    appletSecurityNode = securityNode.SelectSingleNode("./applet") as XmlElement;

                retVal.Security.AllowUnsignedApplets = Boolean.Parse(appletSecurityNode?.Attributes["allowUnsignedApplets"]?.Value ?? "false");
                retVal.Security.TrustedPublishers = new System.Collections.ObjectModel.ObservableCollection<string>(appletSecurityNode?.SelectNodes("./trustedPublishers/add")?.OfType<XmlElement>().Select(o => o.InnerText).ToArray());

                if (tokenSecurityNode != null)
                {
                    retVal.Security.ClaimsAuth = new OpenIzClaimsAuthorization();
                    retVal.Security.ClaimsAuth.Realm = tokenSecurityNode.Attributes["realm"]?.Value;

                    foreach (XmlNode aud in tokenSecurityNode.SelectNodes("./audience/add/@name"))
                        retVal.Security.ClaimsAuth.Audiences.Add(aud.Value);
                    foreach (XmlNode iss in tokenSecurityNode.SelectNodes("./issuer/add"))
                    {
                        String name = iss.Attributes["name"]?.Value,
                            thumbprint = iss.Attributes["findValue"]?.Value,
                            storeLocation = iss.Attributes["storeLocation"]?.Value,
                            storeName = iss.Attributes["storeName"]?.Value,
                            findType = iss.Attributes["x509FindType"]?.Value,
                            symmetricKey = iss.Attributes["symmetricKey"]?.Value,
                            secret = iss.Attributes["symmetricSecret"]?.Value;

                        if (String.IsNullOrEmpty(name))
                            throw new ConfigurationException("Issuer must have name");

                        if(!String.IsNullOrEmpty(secret))
                            retVal.Security.ClaimsAuth.IssuerKeys.Add(name, new InMemorySymmetricSecurityKey(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(secret))));
                        else if (!String.IsNullOrEmpty(symmetricKey))
                            retVal.Security.ClaimsAuth.IssuerKeys.Add(name, new InMemorySymmetricSecurityKey(Convert.FromBase64String(symmetricKey)));
                        else 
                            retVal.Security.ClaimsAuth.IssuerKeys.Add(name, new X509SecurityKey(
                                SecurityUtils.FindCertificate(
                                    storeLocation,
                                    storeName,
                                    findType,
                                    thumbprint
                                    ))
                            );
                        
                    }
                }
                else if(basicSecurityNode != null)
                {
                    retVal.Security.BasicAuth = new OpenIzBasicAuthorization();
                    retVal.Security.BasicAuth.RequireClientAuth = basicSecurityNode.Attributes["requireClientAuth"]?.Value == "true";
                    retVal.Security.BasicAuth.Realm = basicSecurityNode.Attributes["realm"]?.Value;
                    // Allowed claims
                    XmlNodeList allowedClaims = basicSecurityNode.SelectNodes("./allowedClaims/add/@claimType");
                    retVal.Security.BasicAuth.AllowedClientClaims = new System.Collections.ObjectModel.ObservableCollection<string>();
                    foreach(XmlNode all in allowedClaims)
                        retVal.Security.BasicAuth.AllowedClientClaims.Add(all.Value);
                }
            }

            return retVal;
        }
    }
}
