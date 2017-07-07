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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.Connection
{
	/// <summary>
	/// Represents an XML/A connection.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("xmlaConnection")]
	[XmlRoot("xmlaConnection")]
	public class XmlAConnection : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlAConnection"/> class.
		/// </summary>
		public XmlAConnection()
		{
			
		}

		/// <summary>
		/// Gets or sets the catalog.
		/// </summary>
		/// <value>The catalog.</value>
		[XmlElement("catalog")]
		public string Catalog { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[XmlElement("password")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[XmlElement("url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[XmlElement("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the XML a data source.
		/// </summary>
		/// <value>The XML a data source.</value>
		[XmlElement("xmlaDataSource")]
		public string XmlADataSource { get; set; }
	}
}
