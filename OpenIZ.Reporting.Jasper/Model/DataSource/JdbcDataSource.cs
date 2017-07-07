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

namespace OpenIZ.Reporting.Jasper.Model.DataSource
{
	/// <summary>
	/// Represents a JDBC data source.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("jdbcDataSource")]
	[XmlRoot("jdbcDataSource")]
	public class JdbcDataSource : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JdbcDataSource"/> class.
		/// </summary>
		public JdbcDataSource()
		{
			
		}

		/// <summary>
		/// Gets or sets the driver class.
		/// </summary>
		/// <value>The driver class.</value>
		[XmlElement("driverClass")]
		public string DriverClass { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[XmlElement("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[XmlElement("password")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the connection URL.
		/// </summary>
		/// <value>The connection URL.</value>
		[XmlElement("connectionUrl")]
		public string ConnectionUrl { get; set; }

		/// <summary>
		/// Gets or sets the time zone.
		/// </summary>
		/// <value>The time zone.</value>
		[XmlElement("timezone")]
		public string TimeZone { get; set; }

	}
}
