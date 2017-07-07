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
 * Date: 2017-1-6
 */

using Newtonsoft.Json;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents an auto-complete source definition which is that of a query.
	/// </summary>
	[XmlType(nameof(QueryAutoCompleteSourceDefinition), Namespace = "http://openiz.org/risi")]
	[JsonObject(nameof(QueryAutoCompleteSourceDefinition))]
	public class QueryAutoCompleteSourceDefinition : AutoCompleteSourceDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryAutoCompleteSourceDefinition"/> class.
		/// </summary>
		public QueryAutoCompleteSourceDefinition()
		{
		}

		/// <summary>
		/// Gets or sets the query itself.
		/// </summary>
		[XmlElement("query")]
		public string Query { get; set; }

		/// <summary>
		/// Gets or sets the source of the auto-complete source.
		/// </summary>
		[XmlElement("source"), JsonProperty("source")]
		public string Source { get; set; }
	}
}