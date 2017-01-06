/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-12-4
 */

using System;
using OpenIZ.Core.Model.Security;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a stored query to be performed against the RISI.
	/// </summary>
	[XmlType(nameof(Report), Namespace = "http://openiz.org/risi")]
	[XmlRoot(nameof(Report), Namespace = "http://openiz.org/risi")]
	public class Report : BaseEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Report"/> class.
		/// </summary>
		public Report()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Report"/> class
		/// with a specific name.
		/// </summary>
		/// <param name="name">The name of the report.</param>
		public Report(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the correlation id of the report to the report engine.
		/// </summary>
		[XmlElement("correlationId")]
		public Guid CorrelationId { get; set; }

		/// <summary>
		/// Gets or sets the description of the report.
		/// </summary>
		[XmlElement("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the name of the stored query.
		/// </summary>
		[XmlElement("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a list of parameters which is supported for the specified query.
		/// </summary>
		[XmlElement("parameters")]
		public List<ReportParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets security policy instances related to the query definition.
		/// </summary>
		[XmlElement("policy")]
		public List<SecurityPolicyInstance> Policies { get; set; }
	}
}