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
		public SubmissionResult(string msg, int id, SubmissionStatus outcome, string cert)
		{
			this.Message = msg;
			this.RequestId = id;
			this.Status = outcome;
			this.Certificate = cert;
		}

		/// <summary>
		/// Gets or sets the certificate content
		/// </summary>
		[XmlElement("pkcs")]
		public string Certificate { get; set; }

		/// <summary>
		/// Gets or sets the message from the server
		/// </summary>
		[XmlElement("message")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the request id
		/// </summary>
		[XmlAttribute("id")]
		public int RequestId { get; set; }

		/// <summary>
		/// Gets or sets the status
		/// </summary>
		[XmlAttribute("status")]
		public SubmissionStatus Status { get; set; }
	}
}