using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
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
    ///         <basic requireClientAuth="true">
    ///             <!-- Claims allowed to be made by clients on basic auth -->
    ///             <allowedClaims>
    ///                 <add claimType=""/>
    ///             </allowedClaims>
    ///         </basic>
    ///         <token>
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
            XmlElement securityNode = section.SelectSingleNode("./security") as XmlElement;

            // Security?
            if (securityNode != null)
            {
                retVal.Security = new OpenIzSecurityConfiguration();

                XmlElement basicSecurityNode = securityNode.SelectSingleNode("./basic") as XmlElement,
                    tokenSecurityNode = securityNode.SelectSingleNode("./token") as XmlElement;

                if(tokenSecurityNode != null)
                {
                    retVal.Security.ClaimsAuth = new OpenIzClaimsAuthorization();

                    foreach (XmlNode aud in tokenSecurityNode.SelectNodes("./audience/add/@name"))
                        retVal.Security.ClaimsAuth.Audiences.Add(aud.Value);
                    foreach (XmlNode iss in tokenSecurityNode.SelectNodes("./issuer/add"))
                    {
                        String name = iss.Attributes["name"]?.Value,
                            thumbprint = iss.Attributes["findValue"]?.Value,
                            storeLocation = iss.Attributes["storeLocation"]?.Value,
                            storeName = iss.Attributes["storeName"]?.Value,
                            findType = iss.Attributes["x509FindType"]?.Value,
                            symmetricKey = iss.Attributes["symmetricKey"]?.Value;

                        if (String.IsNullOrEmpty(name))
                            throw new ConfigurationException("Issuer must have name");

                        if (!String.IsNullOrEmpty(symmetricKey))
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

                    // Allowed claims
                    XmlNodeList allowedClaims = basicSecurityNode.SelectNodes("./allowedClaims/add/@claimType");
                    retVal.Security.BasicAuth.AllowedClientClaims = new List<string>();
                    foreach(XmlNode all in allowedClaims)
                        retVal.Security.BasicAuth.AllowedClientClaims.Add(all.Value);
                }
            }

            return retVal;
        }
    }
}
