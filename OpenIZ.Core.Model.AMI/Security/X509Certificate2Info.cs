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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// Certificate information
	/// </summary>
	[XmlRoot(nameof(X509Certificate2Info), Namespace = "http://openiz.org/ami")]
	[XmlType(nameof(X509Certificate2Info), Namespace = "http://openiz.org/ami")]
	public class X509Certificate2Info
	{
		public X509Certificate2Info()
		{
		}

		/// <summary>
		/// Constructs a certificate info
		/// </summary>
		/// <param name="cert"></param>
		public X509Certificate2Info(String issuer, DateTime? nbf, DateTime? naf, String sub, String ser)
		{
			this.Issuer = issuer;
			this.NotBefore = nbf;
			this.NotAfter = naf;
			this.Subject = sub;
			this.Thumbprint = ser;
		}

		/// <summary>
		/// Create from a CA attribute set
		/// </summary>
		/// <param name="attributes"></param>
		public X509Certificate2Info(List<KeyValuePair<String, String>> attributes)
		{
			this.Id = Int32.Parse(attributes.First(o => o.Key == "RequestID").Value);
			this.Thumbprint = attributes.First(o => o.Key == "SerialNumber").Value;
			this.Subject = attributes.First(o => o.Key == "DistinguishedName").Value;
			this.NotAfter = DateTime.Parse(attributes.First(o => o.Key == "NotAfter").Value);
			this.NotBefore = DateTime.Parse(attributes.First(o => o.Key == "NotBefore").Value);
			this.Issuer = attributes.First(o => o.Key == "ccm").Value;
		}

		/// <summary>
		/// The identifier of the certificate
		/// </summary>
		[XmlAttribute("id")]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the issuers
		/// </summary>
		[XmlElement("iss")]
		public String Issuer { get; set; }

		/// <summary>
		/// Gets or sets the expiry date
		/// </summary>
		[XmlElement("exp")]
		public DateTime? NotAfter { get; set; }
        public bool ShouldSerializeNotAfter() => this.NotAfter.HasValue;

        /// <summary>
        /// Gets or sets the issue date
        /// </summary>
        [XmlElement("nbf")]
		public DateTime? NotBefore { get; set; }
        public bool ShouldSerializeNotBefore() => this.NotBefore.HasValue;

		/// <summary>
		/// Distinguished name
		/// </summary>
		[XmlElement("sub")]
		public String Subject { get; set; }

		/// <summary>
		/// Gets or sets the thumbprint
		/// </summary>
		[XmlElement("thumbprint")]
		public String Thumbprint { get; set; }
	}
}