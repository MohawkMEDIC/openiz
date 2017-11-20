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
 * User: justi
 * Date: 2017-4-10
 */
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Core.Configuration
{
	/// <summary>
	/// Represents a certificate credential.
	/// </summary>
	[XmlType(nameof(CertificateCredential), Namespace = "http://openiz.org/reporting")]
	public class CertificateCredential : CredentialBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateCredential"/> class.
		/// </summary>
		public CertificateCredential()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CertificateCredential"/> class.
		/// </summary>
		/// <param name="storeLocation">The store location.</param>
		/// <param name="thumbprint">The thumbprint.</param>
		public CertificateCredential(StoreLocation storeLocation, string thumbprint)
		{
			this.StoreLocation = storeLocation;
			this.Thumbprint = thumbprint;
		}

		/// <summary>
		/// Gets or sets the store location.
		/// </summary>
		/// <value>The store location.</value>
		[XmlAttribute("storeLocation")]
		public StoreLocation StoreLocation { get; set; }

		/// <summary>
		/// Gets or sets the thumbprint.
		/// </summary>
		/// <value>The thumbprint.</value>
		[XmlAttribute("thumbprint")]
		public string Thumbprint { get; set; }
	}
}