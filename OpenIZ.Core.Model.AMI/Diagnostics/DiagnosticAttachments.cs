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
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
	/// <summary>
	/// Represents meta-data about a particular log
	/// </summary>
	[JsonObject(nameof(DiagnosticTextAttachment)), XmlType(nameof(DiagnosticTextAttachment), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticTextAttachment : DiagnosticAttachmentInfo
	{
		/// <summary>
		/// The content of the log file
		/// </summary>
		[XmlText, JsonProperty("text")]
		public String Content { get; set; }
	}

	/// <summary>
	/// Represents meta-data about a particular log
	/// </summary>
	[JsonObject(nameof(DiagnosticBinaryAttachment)), XmlType(nameof(DiagnosticBinaryAttachment), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticBinaryAttachment : DiagnosticAttachmentInfo
	{
		/// <summary>
		/// The content of the log file
		/// </summary>
		[XmlElement("data"), JsonProperty("data")]
		public byte[] Content { get; set; }
	}
}