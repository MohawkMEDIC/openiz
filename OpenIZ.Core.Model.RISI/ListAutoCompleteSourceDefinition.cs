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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents an auto complete source which is fed from a static list of members.
	/// </summary>
	[XmlType(nameof(ListAutoCompleteSourceDefinition), Namespace = "http://openiz.org/risi")]
	[JsonObject(nameof(ListAutoCompleteSourceDefinition))]
	public class ListAutoCompleteSourceDefinition : AutoCompleteSourceDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListAutoCompleteSourceDefinition"/> class.
		/// </summary>
		public ListAutoCompleteSourceDefinition()
		{
			this.Items = new List<IdentifiedData>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListAutoCompleteSourceDefinition"/> class
		/// with a specific list of items.
		/// </summary>
		/// <param name="items">The list of items.</param>
		public ListAutoCompleteSourceDefinition(IEnumerable<IdentifiedData> items)
		{
			this.Items = items.ToList();
		}

		/// <summary>
		/// Gets or sets the static list of auto-complete items.
		/// </summary>
		[XmlElement("items"), JsonProperty("items")]
		public List<IdentifiedData> Items { get; set; }
	}
}