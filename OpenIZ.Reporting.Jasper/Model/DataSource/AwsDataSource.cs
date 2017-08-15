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
 * Date: 2017-4-4
 */

using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.DataSource
{
	/// <summary>
	/// Represents an Amazon Web Services data source.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("awsDataSource")]
	[XmlRoot("awsDataSource")]
	public class AwsDataSource : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AwsDataSource"/> class.
		/// </summary>
		public AwsDataSource()
		{
		}

		/// <summary>
		/// Gets or sets the access key.
		/// </summary>
		/// <value>The access key.</value>
		[XmlElement("accessKey")]
		public string AccessKey { get; set; }

		/// <summary>
		/// Gets or sets the connection URL.
		/// </summary>
		/// <value>The connection URL.</value>
		[XmlElement("connectionUrl")]
		public string ConnectionUrl { get; set; }

		/// <summary>
		/// Gets or sets the database instance identifier.
		/// </summary>
		/// <value>The database instance identifier.</value>
		[XmlElement("dbInstanceIdentifier")]
		public string DatabaseInstanceIdentifier { get; set; }

		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		/// <value>The name of the database.</value>
		[XmlElement("dbName")]
		public string DatabaseName { get; set; }

		/// <summary>
		/// Gets or sets the database service.
		/// </summary>
		/// <value>The database service.</value>
		[XmlElement("dbService")]
		public string DatabaseService { get; set; }

		/// <summary>
		/// Gets or sets the driver class.
		/// </summary>
		/// <value>The driver class.</value>
		[XmlElement("driverClass")]
		public string DriverClass { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[XmlElement("password")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the region.
		/// </summary>
		/// <value>The region.</value>
		[XmlElement("region")]
		public string Region { get; set; }

		/// <summary>
		/// Gets or sets the role arn.
		/// </summary>
		/// <value>The role arn.</value>
		[XmlElement("roleArn")]
		public string RoleArn { get; set; }

		/// <summary>
		/// Gets or sets the secret key.
		/// </summary>
		/// <value>The secret key.</value>
		[XmlElement("secretKey")]
		public string SecretKey { get; set; }

		/// <summary>
		/// Gets or sets the time zone.
		/// </summary>
		/// <value>The time zone.</value>
		[XmlElement("timezone")]
		public string TimeZone { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		[XmlElement("username")]
		public string Username { get; set; }
	}
}