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
using OpenIZ.Reporting.Jasper.Model.Collection;
using OpenIZ.Reporting.Jasper.Model.Reference;

namespace OpenIZ.Reporting.Jasper.Model.Core
{
	/// <summary>
	/// Represents an input control.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("inputControl")]
	[XmlRoot("inputControl")]
	public class InputControl : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputControl"/> class.
		/// </summary>
		public InputControl()
		{
			
		}

		/// <summary>
		/// Gets or sets the type of the data.
		/// </summary>
		/// <value>The type of the data.</value>
		[XmlElement("dataTypeReference")]
		public DataType DataType { get; set; }

		/// <summary>
		/// Gets or sets the list of values.
		/// </summary>
		/// <value>The list of values.</value>
		[XmlElement("listOfValues")]
		public ListOfValues ListOfValues { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="InputControl"/> is mandatory.
		/// </summary>
		/// <value><c>true</c> if mandatory; otherwise, <c>false</c>.</value>
		[XmlElement("mandatory")]
		public bool Mandatory { get; set; }

		/// <summary>
		/// Gets or sets the query.
		/// </summary>
		/// <value>The query.</value>
		[XmlElement("queryReference")]
		public Query Query { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [read only].
		/// </summary>
		/// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
		[XmlElement("readOnly")]
		public bool ReadOnly { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[XmlElement("type")]
		public int Type { get; set; }

		/// <summary>
		/// Gets or sets the used fields.
		/// </summary>
		/// <value>The used fields.</value>
		[XmlElement("usedFields")]
		public string UsedFields { get; set; }

		/// <summary>
		/// Gets or sets the value column.
		/// </summary>
		/// <value>The value column.</value>
		[XmlElement("valueColumn")]
		public string ValueColumn { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="InputControl"/> is visible.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		[XmlElement("visible")]
		public bool Visible { get; set; }
		/// <summary>
		/// Gets or sets the visible columns.
		/// </summary>
		/// <value>The visible columns.</value>
		[XmlElement("visibleColumns")]
		public List<string> VisibleColumns { get; set; }
	}
}
