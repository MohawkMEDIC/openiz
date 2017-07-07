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
 * Date: 2017-4-4
 */
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.DataSource
{
	/// <summary>
	/// Represents a JNDI data source.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("jndiDataSource")]
	[XmlRoot("jndiDataSource")]
	public class JndiDataSource : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JndiDataSource"/> class.
		/// </summary>
		public JndiDataSource()
		{
			
		}

		/// <summary>
		/// Gets or sets the name of the JNDI.
		/// </summary>
		/// <value>The name of the JNDI.</value>
		[XmlElement("jndiName")]
		public string JndiName { get; set; }

		/// <summary>
		/// Gets or sets the time zone.
		/// </summary>
		/// <value>The time zone.</value>
		[XmlElement("timezone")]
		public string TimeZone { get; set; }
	}
}
