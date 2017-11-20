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
 * Date: 2016-11-30
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
	/// <summary>
	/// Diagnostics report
	/// </summary>
	[JsonObject(nameof(DiagnosticReport)), XmlType(nameof(DiagnosticReport), Namespace = "http://openiz.org/ami/diagnostics")]
	[XmlRoot(nameof(DiagnosticReport), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticReport : BaseEntityData
	{
		/// <summary>
		/// Gets or sets the note
		/// </summary>
		[XmlElement("note"), JsonProperty("note")]
		public String Note { get; set; }

		/// <summary>
		/// Represents the submitter
		/// </summary>
		[XmlElement("submitter"), JsonProperty("submitter")]
		public UserEntity Submitter { get; set; }

		/// <summary>
		/// Represents the most recent logs for the bug report
		/// </summary>
		[XmlElement("attachText", typeof(DiagnosticTextAttachment)), JsonProperty("attach")]
		[XmlElement("attachBin", typeof(DiagnosticBinaryAttachment))]
		public List<DiagnosticAttachmentInfo> Attachments { get; set; }

		/// <summary>
		/// Application configuration information
		/// </summary>
		[XmlElement("appInfo"), JsonProperty("appInfo")]
		public DiagnosticApplicationInfo ApplicationInfo { get; set; }

		/// <summary>
		/// Gets or sets any ticket related information
		/// </summary>
		[XmlElement("ticketId"), JsonProperty("ticketId")]
		public string CorrelationId { get; set; }
	}
}