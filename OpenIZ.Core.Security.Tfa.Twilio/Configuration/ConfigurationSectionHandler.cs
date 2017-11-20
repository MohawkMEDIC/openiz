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
using System.Configuration;
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