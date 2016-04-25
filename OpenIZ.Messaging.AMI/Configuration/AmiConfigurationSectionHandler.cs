using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Messaging.AMI.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class AmiConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration object
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {

            XmlElement caConfigurationElement = section.SelectSingleNode("./*[local-name() = 'ca']") as XmlElement;
            CertificationAuthorityConfiguration caConfiguration = new CertificationAuthorityConfiguration();

            if(caConfigurationElement != null)
            {
                caConfiguration.AutoApprove = caConfigurationElement?.Attributes["autoApprove"]?.Value == "true";
                caConfiguration.Name = caConfigurationElement?.Attributes["cn"]?.Value;
                caConfiguration.ServerName = caConfigurationElement?.Attributes["serverName"]?.Value;
            }

            // Configuration
            return new AmiConfiguration()
            {
                CaConfiguration = caConfiguration
            };
        }
    }
}
