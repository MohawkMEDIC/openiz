/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: Nityan
 * Date: 2016-6-17
 */

using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
	/// <summary>
	/// Resubmission outcome
	/// </summary>
	[XmlType(nameof(SubmissionStatus), Namespace = "http://openiz.org/ami")]
	public enum SubmissionStatus
	{
		[XmlEnum("NOT COMPLETE")]
		NotComplete = 0x0,

		[XmlEnum("ERROR")]
		Failed = 0x1,

		[XmlEnum("DENIED")]
		Denied = 0x2,

		[XmlEnum("ISSUED")]
		Issued = 0x3,

		[XmlEnum("ISSUED SEPERATELY")]
		IssuedSeperately = 0x4,

		[XmlEnum("SUBMITTED")]
		Submission = 0x5,

		[XmlEnum("REVOKED")]
		Revoked = 0x6
	}
}