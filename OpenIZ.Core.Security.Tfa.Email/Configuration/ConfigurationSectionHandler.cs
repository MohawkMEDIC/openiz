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
 * Date: 2016-11-30
 */
using System;
using System.Configuration;
using System.Linq;
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
                configuration.Smtp = new SmtpConfiguration(new Uri(smtp.Attributes["server"]?.Value ?? "smtp://localhost:25"), smtp.Attributes["username"]?.Value ?? string.Empty, smtp.Attributes["password"]?.Value ?? string.Empty, Boolean.Parse(smtp.Attributes["ssl"]?.Value ?? "false"), smtp.Attributes["from"]?.Value);

			// templates
			configuration.Templates = templates.OfType<XmlElement>().Select(o => new TemplateConfiguration(o.Attributes["lang"]?.Value, o.Attributes["file"]?.Value)).ToList();

			return configuration;
		}
	}
}