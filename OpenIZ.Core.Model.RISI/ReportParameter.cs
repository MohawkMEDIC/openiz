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

using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a parameter.
	/// </summary>
	[XmlType(nameof(ReportParameter), Namespace = "http://openiz.org/risi")]
    [JsonObject(nameof(ReportParameter))]
	public class ReportParameter : BaseEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		public ReportParameter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="value">The value.</param>
		public ReportParameter(Guid id, byte[] value)
		{
			this.Key = id;
			this.Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter" /> class
		/// with a specific name, order, and value.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="position">The position.</param>
		/// <param name="value">The value of the parameter.</param>
		public ReportParameter(string name, int position, byte[] value)
		{
			this.Name = name;
			this.Position = position;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the correlation identifier.
		/// </summary>
		/// <value>The correlation identifier.</value>
		[XmlElement("correlationId"), JsonProperty("correlationId")]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Gets or sets the description of the report parameter.
		/// </summary>
		[XmlElement("description"), JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets whether the report parameter is nullable.
		/// </summary>
		[XmlAttribute("isNullable"), JsonProperty("isNullable")]
		public bool IsNullable { get; set; }

		/// <summary>
		/// Gets or sets the name of the parameter.
		/// </summary>
		[XmlElement("name"), JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the order of the parameter.
		/// </summary>
		[XmlAttribute("position"), JsonProperty("position")]
		public int Position { get; set; }

		/// <summary>
		/// Gets or sets the parameter type associated with the report parameter.
		/// </summary>
		[XmlElement("type"), JsonProperty("type")]
		public ParameterType ParameterType { get; set; }

		/// <summary>
		/// Gets or sets the report definition associated with the report parameter.
		/// </summary>
		[XmlElement("reportDefinition"), JsonProperty("reportDefinition")]
		public ReportDefinition ReportDefinition { get; set; }

		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		[XmlElement("value"), JsonProperty("value")]
		public byte[] Value { get; set; }
	}
}