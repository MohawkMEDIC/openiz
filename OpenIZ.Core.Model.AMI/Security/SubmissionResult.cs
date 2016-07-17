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

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// Represents the submission result
	/// </summary>
	[XmlType(nameof(SubmissionResult), Namespace = "http://openiz.org/ami")]
	[XmlRoot(nameof(SubmissionResult), Namespace = "http://openiz.org/ami")]
	public class SubmissionResult
	{
		public SubmissionResult()
		{
		}

		/// <summary>
		/// Creates a new client certificate request result based on the internal request response
		/// </summary>
		/// <param name="certificateRequestResponse"></param>
		public SubmissionResult(MARC.Util.CertificateTools.CertificateRequestResponse certificateRequestResponse)
		{
			this.Message = certificateRequestResponse.Message;
			this.RequestId = certificateRequestResponse.RequestId;
			this.Status = (SubmissionStatus)certificateRequestResponse.Outcome;
			this.Certificate = certificateRequestResponse.AuthorityResponse;
		}

		/// <summary>
		/// Gets or sets the message from the server
		/// </summary>
		[XmlElement("message")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the certificate content
		/// </summary>
		[XmlElement("pkcs")]
		public string Certificate { get; set; }

		/// <summary>
		/// Gets or sets the status
		/// </summary>
		[XmlAttribute("status")]
		public SubmissionStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the request id
		/// </summary>
		[XmlAttribute("id")]
		public int RequestId { get; set; }
	}
}