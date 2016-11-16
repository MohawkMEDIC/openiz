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
 * User: Nityan
 * Date: 2016-11-15
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.BusinessRules.Core.Configuration
{
	/// <summary>
	/// Represents a configuration section handler for a business rules configuration.
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
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
			var configuration = new BusinessRulesCoreConfiguration();
			var directoryElement = section.SelectSingleNode("./*[local-name() = 'directory']") as XmlElement;

			var path = directoryElement?.Attributes["path"]?.Value;

			if (path == null)
			{
				throw new ConfigurationErrorsException("Missing directory", section);
			}

			if (!Directory.Exists(path))
			{
				throw new ConfigurationErrorsException($"Path {path} does not exist");
			}

			return new BusinessRulesCoreConfiguration(path);
		}
	}
}
