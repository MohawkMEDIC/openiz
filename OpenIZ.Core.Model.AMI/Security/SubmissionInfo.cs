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
 * Date: 2016-8-2
 */
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// Enrollment information
	/// </summary>
	[XmlType(nameof(SubmissionInfo), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SubmissionInfo), Namespace = "http://openiz.org/ami")]
	public class SubmissionInfo
	{
		/// <summary>
		/// Administration contact
		/// </summary>
		[XmlElement("adminContact")]
		public string AdminContact { get; set; }

		/// <summary>
		/// Administration
		/// </summary>
		public String AdminSiteAddress { get; set; }

		/// <summary>
		/// Disposition message
		/// </summary>
		[XmlElement("message")]
		public string DispositionMessage { get; set; }

		/// <summary>
		/// DN
		/// </summary>
		[XmlElement("dn")]
		public string DistinguishedName { get; set; }

		/// <summary>
		/// Email address of user
		/// </summary>
		[XmlElement("email")]
		public string EMail { get; set; }

		/// <summary>
		/// Expiry
		/// </summary>
		[XmlElement("notAfter")]
		public string NotAfter { get; set; }

		/// <summary>
		/// Before date
		/// </summary>
		[XmlElement("notBefore")]
		public string NotBefore { get; set; }

		/// <summary>
		/// RequestId
		/// </summary>
		[XmlAttribute("id")]
		public string RequestID { get; set; }

		/// <summary>
		/// Resolved on
		/// </summary>
		[XmlElement("resolved")]
		public string ResolvedWhen { get; set; }

		/// <summary>
		/// Revoke reason from Keystore
		/// </summary>
		[XmlIgnore]
		public String RevokedReason
		{
			get { return ((int)this.XmlRevokeReason).ToString(); }
			set
			{
				if (value != null)
					this.XmlRevokeReason = (RevokeReason)Int32.Parse(value);
			}
		}

		/// <summary>
		/// Revoked on
		/// </summary>
		[XmlElement("revoked")]
		public string RevokedWhen { get; set; }

		/// <summary>
		/// Submitted on
		/// </summary>
		[XmlElement("submitted")]
		public string SubmittedWhen { get; set; }

		/// <summary>
		/// Revokation reason
		/// </summary>
		[XmlElement("revokationReason")]
		public RevokeReason XmlRevokeReason { get; set; }

		/// <summary>
		/// Status code
		/// </summary>
		[XmlAttribute("status")]
		public SubmissionStatus XmlStatusCode { get; set; }
	}
}