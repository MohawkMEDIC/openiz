/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-28
 */
using System.Configuration;
using System.Xml;

namespace OpenIZ.Messaging.RISI.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for the RISI configuration.
	/// </summary>
	public class RisiConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates the configuration section.
		/// </summary>
		/// <param name="parent">The parent configuration section.</param>
		/// <param name="configContext">The configuration context.</param>
		/// <param name="section">The configuration section.</param>
		/// <returns>Returns an instance of the configuration section.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlElement serviceElement = section.SelectSingleNode("./*[local-name() = 'service']") as XmlElement;

			string wcfServiceName = serviceElement?.Attributes["wcfServiceName"]?.Value;

			if (wcfServiceName == null)
			{
				throw new ConfigurationErrorsException("Missing serviceElement", section);
			}

			return new RisiConfiguration(wcfServiceName);
		}
	}
}