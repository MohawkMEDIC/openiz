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

using System;
using System.Configuration;
using System.Linq;
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
			var reportEngineElement = section.SelectSingleNode("./*[local-name() = 'reportEngine']") as XmlElement;

			var type = reportEngineElement?.Attributes["type"]?.Value;

			if (type == null)
			{
				throw new ConfigurationErrorsException("The 'reportEngine' element must have a type attribute");
			}

			if (!Enum.GetNames(typeof(ReportEngineType)).Contains(type))
			{
				throw new ConfigurationErrorsException($"The 'type' attribute must be a valid { nameof(ReportEngineType) } value. Valid values: { string.Join(", ", Enum.GetNames(typeof(ReportEngineType))) }");
			}

			ReportEngineType reportEngineType;

			Enum.TryParse<ReportEngineType>(type, out reportEngineType);

			switch (reportEngineType)
			{
				case ReportEngineType.Jasper:
					break;

				case ReportEngineType.MsSql:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var address = reportEngineElement.Attributes["address"]?.Value;

			if (address == null)
			{
				throw new ConfigurationErrorsException("The 'address' attribute cannot be null");
			}

			if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
			{
				throw new ConfigurationErrorsException("The 'address' attribute must be a well formed URI");
			}

			return new RisiConfiguration(new Uri(address), reportEngineType);
		}
	}
}