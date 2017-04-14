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
 * Date: 2017-1-13
 */

using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a report format.
	/// </summary>
	[XmlRoot(nameof(ReportFormat), Namespace = "http://openiz.org/risi")]
	[XmlType(nameof(ReportFormat), Namespace = "http://openiz.org/risi")]
	[JsonObject(nameof(ReportFormat))]
    public class ReportFormat : BaseEntityData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportFormat"/> class.
		/// </summary>
		public ReportFormat()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportFormat"/> class
		/// with a specific id and format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <param name="format">The format of the report format.</param>
		public ReportFormat(Guid id, string format)
		{
			this.Key = id;
			this.Format = format;
		}

		/// <summary>
		/// Gets or sets the format of the report format.
		/// </summary>
		[XmlAttribute("format"), JsonProperty("format")]
		public string Format { get; set; }
	}
}