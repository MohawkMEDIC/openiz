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
 * Date: 2017-9-6
 */

using System.Configuration;
using System.Xml;

namespace OpenIZ.Reporting.Jasper.Configuration
{
	/// <summary>
	/// Represents a jasper configuration section handler.
	/// </summary>
	public class JasperConfigurationSectionHandler : IConfigurationSectionHandler
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
			var reportEngineElement = section.SelectSingleNode("./*[local-name() = 'engine']") as XmlElement;

			return new JasperConfiguration(reportEngineElement?.Attributes["reportPath"]?.Value);
		}
	}
}