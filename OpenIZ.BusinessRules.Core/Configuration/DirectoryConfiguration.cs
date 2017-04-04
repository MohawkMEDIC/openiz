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
 * User: Nityan
 * Date: 2016-11-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.BusinessRules.Core.Configuration
{
	/// <summary>
	/// Represents a directory configuration.
	/// </summary>
	public class DirectoryConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryConfiguration"/> class.
		/// </summary>
		public DirectoryConfiguration()
		{
			this.SupportedExtensions = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryConfiguration"/> class
		/// with a specific path and supported extensions.
		/// </summary>
		/// <param name="path">The path of the directory.</param>
		/// <param name="supportedExtensions">The supported extensions of the directory configuration.</param>
		public DirectoryConfiguration(string path, List<string> supportedExtensions)
		{
			this.Path = path;
			this.SupportedExtensions = supportedExtensions;
		}

		/// <summary>
		/// Gets or sets a list of supported extensions of the directory.
		/// </summary>
		[XmlElement]
		public List<string> SupportedExtensions { get; set; }

		/// <summary>
		/// Gets or sets the path of the directory.
		/// </summary>
		[XmlAttribute("path")]
		public string Path { get; set; }
	}
}
