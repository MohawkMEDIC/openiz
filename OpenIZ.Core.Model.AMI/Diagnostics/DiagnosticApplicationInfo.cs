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
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
	/// <summary>
	/// Application information
	/// </summary>
	[JsonObject(nameof(DiagnosticApplicationInfo)), XmlType(nameof(DiagnosticApplicationInfo), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticApplicationInfo : DiagnosticVersionInfo
	{
		private Tracer m_tracer = Tracer.GetTracer(typeof(DiagnosticApplicationInfo));

		/// <summary>
		/// Diagnostic application information
		/// </summary>
		public DiagnosticApplicationInfo()
		{
		}

		/// <summary>
		/// Creates new diagnostic application information
		/// </summary>
		/// <param name="versionInfo"></param>
		public DiagnosticApplicationInfo(Assembly versionInfo) : base(versionInfo)
		{
		}

		/// <summary>
		/// Environment information
		/// </summary>
		[JsonProperty("environment"), XmlElement("environment")]
		public DiagnosticEnvironmentInfo EnvironmentInfo { get; set; }

		/// <summary>
		/// Open IZ information
		/// </summary>
		[JsonProperty("openiz"), XmlElement("openiz")]
		public DiagnosticVersionInfo OpenIZ { get; set; }

		/// <summary>
		/// Gets or sets the assemblies
		/// </summary>
		[JsonProperty("assembly"), XmlElement("assembly")]
		public List<DiagnosticVersionInfo> Assemblies { get; set; }

		/// <summary>
		/// Gets or sets the applets
		/// </summary>
		[JsonProperty("applet"), XmlElement("applet")]
		public List<AppletInfo> Applets { get; set; }

        /// <summary>
        /// Gets or sets the applets
        /// </summary>
        [JsonProperty("service"), XmlElement("service")]
        public List<DiagnosticServiceInfo> ServiceInfo { get; set; }


        /// <summary>
        /// Gets or sets file info
        /// </summary>
        [JsonProperty("fileInfo"), XmlElement("fileInfo")]
		public List<DiagnosticAttachmentInfo> FileInfo { get; set; }

		/// <summary>
		/// Gets the sync info
		/// </summary>
		[JsonProperty("syncInfo"), XmlElement("syncInfo")]
		public List<DiagnosticSyncInfo> SyncInfo { get; set; }
	}
}