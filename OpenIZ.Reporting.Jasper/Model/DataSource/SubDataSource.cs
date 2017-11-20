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

using System.Xml;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.DataSource
{
	/// <summary>
	/// Represents a sub data source.
	/// </summary>
	[XmlType("subDataSource")]
	public class SubDataSource
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SubDataSource"/> class.
		/// </summary>
		public SubDataSource()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SubDataSource" /> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="url">The URL.</param>
		public SubDataSource(string id, string url)
		{
			this.Id = id;
			this.Url = url;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		[XmlElement("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("url")]
		public string Url { get; set; }
	}
}