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
	/// Environment information
	/// </summary>
	[JsonObject(nameof(DiagnosticEnvironmentInfo)), XmlType(nameof(DiagnosticEnvironmentInfo), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticEnvironmentInfo
	{
		/// <summary>
		/// Is platform 64 bit
		/// </summary>
		[JsonProperty("is64bit"), XmlAttribute("is64Bit")]
		public bool Is64Bit { get; set; }

		/// <summary>
		/// OS Version
		/// </summary>
		[JsonProperty("osVersion"), XmlAttribute("osVersion")]
		public String OSVersion { get; set; }

		/// <summary>
		/// CPU count
		/// </summary>
		[JsonProperty("processorCount"), XmlAttribute("processorCount")]
		public int ProcessorCount { get; set; }

		/// <summary>
		/// Used memory
		/// </summary>
		[JsonProperty("usedMem"), XmlElement("mem")]
		public long UsedMemory { get; set; }

		/// <summary>
		/// Version
		/// </summary>
		[JsonProperty("version"), XmlElement("version")]
		public String Version { get; set; }
	}
}