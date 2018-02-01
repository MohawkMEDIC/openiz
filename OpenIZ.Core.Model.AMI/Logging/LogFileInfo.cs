/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */

using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Logging
{
	/// <summary>
	/// Log file information
	/// </summary>
	[XmlRoot(nameof(LogFileInfo), Namespace = "http://openiz.org/ami")]
	[JsonObject(nameof(LogFileInfo))]
	public class LogFileInfo
	{
		/// <summary>
		/// Gets or sets the content
		/// </summary>
		[XmlElement("text"), JsonProperty("text")]
		public byte[] Contents { get; set; }

		/// <summary>
		/// Gets or sets the last write time
		/// </summary>
		[XmlElement("modified"), JsonProperty("modified")]
		public DateTime LastWrite { get; set; }

		/// <summary>
		/// The key of the logfile
		/// </summary>
		[XmlElement("name"), JsonProperty("name")]
		public String Name { get; set; }

		/// <summary>
		/// Gets or sets the size of the file
		/// </summary>
		[XmlElement("size"), JsonProperty("size")]
		public long Size { get; set; }
	}
}