using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Messaging.IMSI.Configuration
{
    /// <summary>
    /// Configuration handler for the IMSI section
    /// </summary>
    public class ImsiConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            
            // Section
            XmlElement serviceElement = section.SelectSingleNode("./*[local-name() = 'service']") as XmlElement;

            string wcfServiceName = serviceElement?.Attributes["wcfServiceName"]?.Value;

            if(wcfServiceName == null)
                throw new ConfigurationErrorsException("Missing serviceElement", section);

            return new ImsiConfiguration(wcfServiceName);
        }
    }
}
