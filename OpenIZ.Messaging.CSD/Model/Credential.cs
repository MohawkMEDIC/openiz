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
 * User: Nityan
 * Date: 2017-4-2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenIZ.Messaging.CSD.Model.Identifier;

namespace OpenIZ.Messaging.CSD.Model
{
	/// <summary>
	/// Represents a credential.
	/// </summary>
	[XmlType("credential", Namespace = "urn:ihe:iti:csd:2013")]
	public class Credential
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Credential"/> class.
		/// </summary>
		public Credential()
		{
			
		}

		/// <summary>
		/// Gets or sets the type of the coded.
		/// </summary>
		/// <value>The type of the coded.</value>
		[XmlElement("codedType")]
		public CodedType CodedType { get; set; }

		/// <summary>
		/// Gets or sets the number.
		/// </summary>
		/// <value>The number.</value>
		[XmlElement("number")]
		public string Number { get; set; }

		/// <summary>
		/// Gets or sets the issuing authority.
		/// </summary>
		/// <value>The issuing authority.</value>
		[XmlElement("issuingAuthority")]
		public string IssuingAuthority { get; set; }

		/// <summary>
		/// Gets or sets the credential issue date.
		/// </summary>
		/// <value>The credential issue date.</value>
		[XmlElement("credentialIssueDate", DataType = "date")]
		public DateTime CredentialIssueDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [credential issue date specified].
		/// </summary>
		/// <value><c>true</c> if [credential issue date specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool CredentialIssueDateSpecified { get; set; }

		/// <summary>
		/// Gets or sets the credential renewal date.
		/// </summary>
		/// <value>The credential renewal date.</value>
		[XmlElement("credentialRenewalDate", DataType = "date")]
		public DateTime CredentialRenewalDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [credential renewal date specified].
		/// </summary>
		/// <value><c>true</c> if [credential renewal date specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool CredentialRenewalDateSpecified { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }
	}
}
