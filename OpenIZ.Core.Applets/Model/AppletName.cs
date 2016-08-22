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
 * User: justi
 * Date: 2016-6-14
 */
using System;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenIZ.Core.Applets.Model
{
	/// <summary>
	/// Applet reference
	/// </summary>
	[XmlType(nameof(AppletName), Namespace = "http://openiz.org/applet"), JsonObject]
	public class AppletName
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Applets.Model.AppletName"/> class.
		/// </summary>
		public AppletName ()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Core.Applets.Model.AppletName"/> class.
		/// </summary>
		public AppletName (String id, String version, String publicKeyToken)
		{
			this.Id = id;
			this.Version = version;
			this.PublicKeyToken = publicKeyToken;
		}

		/// <summary>
		/// The identifier of the applet
		/// </summary>
		[XmlAttribute("id")]
        [JsonProperty("id")]
		public String Id {
			get;
			set;
		}

		/// <summary>
		/// The version of the applet
		/// </summary>
		[XmlAttribute("version")]
        [JsonProperty("version")]
		public String Version {
			get;
			set;
		}

		/// <summary>
		/// The signature of the applet (not used for verification, rather lookup)
		/// </summary>
		/// <value>The signature.</value>
		[XmlAttribute("publicKeyToken")]
		public String PublicKeyToken {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the signature which can be used to validate the file
		/// </summary>
		[XmlElement("signature")]
		public byte[] Signature {
			get;
			set;
		}

		/// <summary>
		/// A hash which validates the file has not changed on the disk. Different from signature which 
		/// indicates the manifest has not changed since installed.
		/// </summary>
		/// <value><c>true</c> if this instance hash; otherwise, <c>false</c>.</value>
		[XmlElement("hash")]
		public byte[] Hash {
			get;
			set;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString ()
		{
			return string.Format ("Id={0}, Version={1}, PublicKeyToken={2}", Id, Version, PublicKeyToken);
		}
	}


}

