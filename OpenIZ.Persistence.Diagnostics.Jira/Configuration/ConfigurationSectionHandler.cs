using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
    /// <summary>
    /// A configuration section handler for jira configuration
    /// </summary>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration 
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {

            // URL
            XmlElement urlConfig = section.SelectSingleNode("./*[local-name() = 'jira']") as XmlElement,
                authConfig = section.SelectSingleNode("./*[local-name() = 'auth']") as XmlElement;

            String url = urlConfig?.Attributes["url"]?.Value,
                project = urlConfig?.Attributes["project"]?.Value,
                username = authConfig?.Attributes["username"]?.Value,
                password = authConfig?.Attributes["password"]?.Value;

            if (url == null)
                throw new ConfigurationErrorsException("JIRA URL is required", section);
            else if (project == null)
                throw new ConfigurationErrorsException("JIRA project key is required", section);
            else if (username == null || password == null)
                throw new ConfigurationErrorsException("JIRA username and password required", section);

            return new JiraServiceConfiguration(url, project, username, password);
        }
    }
}
