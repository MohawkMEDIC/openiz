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
		/// The parameter type.
		/// </summary>
		private ParameterType parameterType;

		/// <summary>
		/// The parameter type key.
		/// </summary>
		private Guid parameterTypeKey;

		/// <summary>
		/// The report definition.
		/// </summary>
		private ReportDefinition reportDefinition;

		/// <summary>
		/// The report definition key.
		/// </summary>
		private Guid reportDefinitionKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		public ReportParameter()
		{
			this.CreationTime = DateTimeOffset.Now;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public ReportParameter(Guid key) : this()
		{
			this.Key = key;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter" /> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public ReportParameter(Guid key, byte[] value) : this(key)
		{
			this.Key = key;
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
		/// Gets or sets the parameter type associated with the report parameter.
		/// </summary>
		[XmlElement("parameterType"), JsonProperty("parameterType")]
		public ParameterType ParameterType { get; set; }

		/// <summary>
		/// Gets or sets the order of the parameter.
		/// </summary>
		[XmlAttribute("position"), JsonProperty("position")]
		public int Position { get; set; }

		/// <summary>
		/// Gets or sets the report definition associated with the report parameter.
		/// </summary>
		[XmlIgnore, JsonIgnore]
		public ReportDefinition ReportDefinition
		{
			get
			{
				return this.reportDefinition;
			}
			set
			{
				this.reportDefinition = value;
				this.reportDefinitionKey = value.Key ?? Guid.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the report definition key.
		/// </summary>
		/// <value>The report definition key.</value>
		[XmlElement("reportDefinition"), JsonProperty("reportDefinition")]
		public Guid ReportDefinitionKey
		{
			get
			{
				return this.reportDefinitionKey;
			}
			set
			{
				this.reportDefinitionKey = value;
				this.reportDefinition = null;
			}
		}

		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		[XmlElement("value"), JsonProperty("value")]
		public byte[] Value { get; set; }
	}
}