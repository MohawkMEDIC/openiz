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

namespace OpenIZ.Core.Model.AMI.Auth
{
	/// <summary>
	/// Security ticket information
	/// </summary>
	[XmlType(nameof(TfaRequestInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(TfaRequestInfo), Namespace = "http://openiz.org/ami")]
	[JsonObject(nameof(TfaRequestInfo))]
	public class TfaRequestInfo
	{
		/// <summary>
		/// Gets or sets the user key
		/// </summary>
		[XmlElement("mechanism"), JsonProperty("mechanism")]
		public Guid ResetMechanism { get; set; }

		/// <summary>
		/// The verification (usually the phone number or e-mail address of the user)
		/// </summary>
		[XmlElement("verification"), JsonProperty("verification")]
		public String Verification { get; set; }

		/// <summary>
		/// The verification (usually the phone number or e-mail address of the user)
		/// </summary>
		[XmlElement("username"), JsonProperty("username")]
		public String UserName { get; set; }

		/// <summary>
		/// The scope or purpose of the TFA secret
		/// </summary>
		[XmlElement("purpose"), JsonProperty("purpose")]
		public String Purpose { get; set; }
	}
}