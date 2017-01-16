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

using System.Xml.Serialization;

namespace OpenIZ.Persistence.Reporting.MSSQL.Configuration
{
	/// <summary>
	/// Represents reporting configuration.
	/// </summary>
	[XmlType(nameof(ReportingConfiguration), Namespace = "http://openiz.org/risi")]
	public class ReportingConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportingConfiguration"/> class.
		/// </summary>
		public ReportingConfiguration()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportingConfiguration"/> class.
		/// </summary>
		/// <param name="connectionStringName">The connection string name.</param>
		public ReportingConfiguration(string connectionStringName)
		{
			this.ConnectionStringName = connectionStringName;
		}

		/// <summary>
		/// Gets or sets the connection string of the configuration.
		/// </summary>
		[XmlAttribute("name")]
		public string ConnectionStringName { get; set; }
	}
}