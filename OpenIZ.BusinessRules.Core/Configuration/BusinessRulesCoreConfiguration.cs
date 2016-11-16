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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.BusinessRules.Core.Configuration
{
	/// <summary>
	/// Represents a business rules core configuration.
	/// </summary>
	public class BusinessRulesCoreConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessRulesCoreConfiguration"/> class.
		/// </summary>
		public BusinessRulesCoreConfiguration()
		{
			this.DirectoryConfiguration = new DirectoryConfiguration();
		}

		/// <summary>
		/// Gets or sets the path of the business rules core configuration.
		/// </summary>
		[XmlElement("directory")]
		public DirectoryConfiguration DirectoryConfiguration { get; set; }
	}
}
