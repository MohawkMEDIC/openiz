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
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
	/// <summary>
	/// Application version information
	/// </summary>
	[JsonObject(nameof(DiagnosticVersionInfo)), XmlType(nameof(DiagnosticVersionInfo), Namespace = "http://openiz.org/ami/diagnostics")]
	public class DiagnosticVersionInfo
	{
		/// <summary>
		/// Diagnostic version information
		/// </summary>
		public DiagnosticVersionInfo()
		{
		}

		/// <summary>
		/// Version information
		/// </summary>
		public DiagnosticVersionInfo(Assembly asm)
		{
			if (asm == null) return;
			this.Version = asm.GetName().Version.ToString();
			this.InformationalVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
			this.Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
			this.Product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
			this.Name = asm.GetName().Name;
			this.Info = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
		}

		[JsonProperty("version"), XmlAttribute("version")]
		public String Version { get; set; }

		[JsonProperty("infoVersion"), XmlAttribute("infoVersion")]
		public String InformationalVersion { get; set; }

		[JsonProperty("copyright"), XmlAttribute("copyright")]
		public String Copyright { get; set; }

		[JsonProperty("product"), XmlAttribute("product")]
		public String Product { get; set; }

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		[JsonProperty("name"), XmlAttribute("name")]
		public String Name { get; set; }

		/// <summary>
		/// Gets or sets the informational value
		/// </summary>
		[JsonProperty("info"), XmlElement("description")]
		public String Info { get; set; }
	}
}