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
 * Date: 2017-1-16
 */

using OpenIZ.OrmLite.Providers;
using System.Xml.Serialization;

namespace OpenIZ.Persistence.Reporting.PSQL.Configuration
{
	/// <summary>
	/// Represents reporting configuration.
	/// </summary>
	public class ReportingConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportingConfiguration"/> class.
		/// </summary>
		public ReportingConfiguration()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether to automatically update existing records.
		/// </summary>
		/// <value><c>true</c> if records should be automatically updated; otherwise, <c>false</c>.</value>
		[XmlAttribute("autoUpdateExisting")]
		public bool AutoUpdateExisting { get; set; }

		/// <summary>
		/// Gets or sets the provider.
		/// </summary>
		/// <value>The provider.</value>
		[XmlAttribute("provider")]
		public IDbProvider Provider { get; set; }

		/// <summary>
		/// Gets or sets the readonly connection string.
		/// </summary>
		/// <value>The readonly connection string.</value>
		[XmlAttribute("readonlyConnection")]
		public string ReadonlyConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the connection string of the configuration.
		/// </summary>
		[XmlAttribute("readWriteConnection")]
		public string ReadWriteConnectionString { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the application should trace SQL statements.
		/// </summary>
		/// <value><c>true</c> If the application should trace SQL statements; otherwise, <c>false</c>.</value>
		[XmlAttribute("traceSql")]
		public bool TraceSql { get; set; }
	}
}