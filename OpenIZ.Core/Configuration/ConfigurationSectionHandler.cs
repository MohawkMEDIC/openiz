using System;
using System.Collections.Generic;
using System.Configuration;
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

                XmlElement basicSecurityNode = securityNode.SelectSingleNode("./basic") as XmlElement;
                if(basicSecurityNode != null)
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
