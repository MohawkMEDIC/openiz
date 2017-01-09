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
 * Date: 2017-1-5
 */

using OpenIZ.Core.Model.RISI.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a parameter.
	/// </summary>
	[XmlType(nameof(ReportParameter), Namespace = "http://openiz.org/risi")]
	public class ReportParameter : BaseEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		public ReportParameter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class
		/// with a specific name, order, and value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="order">The order of the parameter.</param>
		/// <param name="value">The value of the parameter.</param>
		public ReportParameter(string name, int order, object value)
		{
			this.Name = name;
			this.Order = order;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the report data types of the report parameter.
		/// </summary>
		[XmlElement("dataType")]
		public ReportDataType DataType { get; set; }

		/// <summary>
		/// Gets or sets whether the report parameter is nullable.
		/// </summary>
		[XmlAttribute("isNullable")]
		public bool IsNullable { get; set; }

		/// <summary>
		/// Gets or sets the name of the parameter.
		/// </summary>
		[XmlElement("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the order of the parameter.
		/// </summary>
		[XmlAttribute("order")]
		public int Order { get; set; }

		/// <summary>
		/// Gets or sets the default provider of the report parameter.
		/// </summary>
		[XmlElement("provider")]
		public string Provider { get; set; }
		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		[XmlElement("value")]
		public object Value { get; set; }
	}
}