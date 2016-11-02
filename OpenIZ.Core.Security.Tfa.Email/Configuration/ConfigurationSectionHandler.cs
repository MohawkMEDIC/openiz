using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Core.Security.Tfa.Email.Configuration
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
            XmlElement smtp = section.SelectSingleNode("./*[local-name() = 'smtp']") as XmlElement;
            XmlNodeList templates = section.SelectNodes("./*[local-name() = 'template']/*[local-name() = 'add']");

            MechanismConfiguration configuration = new MechanismConfiguration();

            if (smtp == null)
                throw new ConfigurationErrorsException("Missing SMTP configuration", section);
            else
                configuration.Smtp = new SmtpConfiguration(new Uri(smtp.Attributes["server"]?.Value ?? "smtp://localhost:25"), smtp.Attributes["username"]?.Value, smtp.Attributes["password"]?.Value, Boolean.Parse(smtp.Attributes["ssl"]?.Value ?? "false"));

            // templates
            configuration.Templates = templates.OfType<XmlElement>().Select(o => new TemplateConfiguration(o.Attributes["lang"]?.Value, o.Attributes["file"]?.Value)).ToList();

            return configuration;
        }
    }
}
