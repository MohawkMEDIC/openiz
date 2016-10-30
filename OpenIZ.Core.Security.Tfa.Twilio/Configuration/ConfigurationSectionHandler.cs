using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Core.Security.Tfa.Twilio.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the specified configuration object
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlElement sms = section.SelectSingleNode("./*[local-name() = 'sms']") as XmlElement;

            MechanismConfiguration configuration = new MechanismConfiguration();

            if (sms == null)
                throw new ConfigurationErrorsException("Missing SMS configuration", section);
            else
            {
                configuration.Auth = sms.Attributes["auth"]?.Value;
                configuration.From = sms.Attributes["from"]?.Value;
                configuration.Sid = sms.Attributes["sid"]?.Value;
            }

            return configuration;
        }
    }
}
