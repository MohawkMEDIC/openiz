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
 * Date: 2017-1-12
 */

using Newtonsoft.Json;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a stored query to be performed against the RISI.
	/// </summary>
	[XmlType(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
	[XmlRoot(nameof(ReportDefinition), Namespace = "http://openiz.org/risi")]
	[JsonObject(nameof(ReportDefinition))]
	public class ReportDefinition : BaseEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinition"/> class.
		/// </summary>
		public ReportDefinition() : this(Guid.NewGuid())
		{
			this.CreationTime = DateTimeOffset.Now;
			this.Parameters = new List<ReportParameter>();
			this.Policies = new List<SecurityPolicyInstance>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinition"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public ReportDefinition(Guid key)
		{
			this.Key = key;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinition"/> class
		/// with a specific name.
		/// </summary>
		/// <param name="name">The name of the report.</param>
		public ReportDefinition(string name) : this()
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the correlation id of the report to the report engine.
		/// </summary>
		[XmlAttribute("correlationId"), JsonProperty("correlationId")]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Gets or sets the description of the report.
		/// </summary>
		[XmlElement("description"), JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the report formats.
		/// </summary>
		/// <value>The report formats.</value>
		[XmlElement("formats"), JsonProperty("formats")]
		public List<ReportFormat> Formats { get; set; }

		/// <summary>
		/// Gets or sets the name of the stored query.
		/// </summary>
		[XmlElement("name"), JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a list of parameters which are supported for the specified query.
		/// </summary>
		[XmlElement("parameters"), JsonProperty("parameters")]
		public List<ReportParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets security policy instances related to the query definition.
		/// </summary>
		[XmlElement("policy"), JsonProperty("policy")]
		public List<SecurityPolicyInstance> Policies { get; set; }
	}
}