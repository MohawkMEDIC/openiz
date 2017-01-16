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
 * User: khannan
 * Date: 2017-1-7
 */

using System.Configuration;
using System.Xml;

namespace OpenIZ.Persistence.Reporting.MSSQL.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for the reporting configuration.
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section">Section XML node.</param>
		/// <returns>The created section handler object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			var providerElement = section.SelectSingleNode("./*[local-name() = 'connectionString']") as XmlElement;

			var connectionString = providerElement?.Attributes["name"]?.Value;

			if (connectionString == null)
			{
				throw new ConfigurationErrorsException("The 'connectionString' element must have a 'name' attribute");
			}

			return new ReportingConfiguration(connectionString);
		}
	}
}