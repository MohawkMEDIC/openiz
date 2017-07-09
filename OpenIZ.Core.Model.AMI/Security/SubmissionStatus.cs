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
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// Represents a submission status.
	/// </summary>
	[XmlType(nameof(SubmissionStatus), Namespace = "http://openiz.org/ami")]
	public enum SubmissionStatus
	{
		/// <summary>
		/// The submission status is not yet complete.
		/// </summary>
		[XmlEnum("NOT COMPLETE")]
		NotComplete = 0x0,

		/// <summary>
		/// The submission status failed.
		/// </summary>
		[XmlEnum("ERROR")]
		Failed = 0x1,

		/// <summary>
		/// The submission status is denied.
		/// </summary>
		[XmlEnum("DENIED")]
		Denied = 0x2,

		/// <summary>
		/// The submission status is issued.
		/// </summary>
		[XmlEnum("ISSUED")]
		Issued = 0x3,

		/// <summary>
		/// The submission status is issued separately.
		/// </summary>
		[XmlEnum("ISSUED SEPERATELY")]
		IssuedSeparately = 0x4,

		/// <summary>
		/// The submission status is submitted.
		/// </summary>
		[XmlEnum("SUBMITTED")]
		Submission = 0x5,

		/// <summary>
		/// The submission status is revoked.
		/// </summary>
		[XmlEnum("REVOKED")]
		Revoked = 0x6
	}
}