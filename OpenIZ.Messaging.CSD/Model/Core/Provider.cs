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

using System.Collections.Generic;
using System.Xml.Serialization;
using OpenIZ.Messaging.CSD.Model.Association;
using OpenIZ.Messaging.CSD.Model.Entity;
using OpenIZ.Messaging.CSD.Model.Identifier;

namespace OpenIZ.Messaging.CSD.Model.Core
{
	/// <summary>
	/// Represents a provider.
	/// </summary>
	/// <seealso cref="UniqueId" />
	[XmlType("provider", Namespace = "urn:ihe:iti:csd:2013")]
	public class Provider : UniqueId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Provider"/> class.
		/// </summary>
		public Provider()
		{
			
		}

		/// <summary>
		/// Gets or sets the other ids.
		/// </summary>
		/// <value>The other ids.</value>
		[XmlElement("otherID")]
		public List<OtherId> OtherIds { get; set; }

		/// <summary>
		/// Gets or sets the coded types.
		/// </summary>
		/// <value>The coded types.</value>
		[XmlElement("codedType")]
		public List<CodedType> CodedTypes { get; set; }

		/// <summary>
		/// Gets or sets the person.
		/// </summary>
		/// <value>The person.</value>
		[XmlElement("demographic")]
		public Person Person { get; set; }

		/// <summary>
		/// Gets or sets the languages.
		/// </summary>
		/// <value>The languages.</value>
		[XmlElement("language")]
		public List<CodedType> Languages { get; set; }

		/// <summary>
		/// Gets or sets the provider organizations.
		/// </summary>
		/// <value>The provider organizations.</value>
		[XmlArrayItem("organization", IsNullable = false)]
		public List<ProviderOrganization> ProviderOrganizations { get; set; }

		/// <summary>
		/// Gets or sets the provider facilities.
		/// </summary>
		/// <value>The provider facilities.</value>
		[XmlArrayItem("facility", IsNullable = false)]
		public List<ProviderFacility> ProviderFacilities { get; set; }

		/// <summary>
		/// Gets or sets the credentials.
		/// </summary>
		/// <value>The credentials.</value>
		[XmlElement("credential")]
		public List<Credential> Credentials { get; set; }

		/// <summary>
		/// Gets or sets the specialty.
		/// </summary>
		/// <value>The specialty.</value>
		[XmlElement("specialty")]
		public List<CodedType> Specialty { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets or sets the record.
		/// </summary>
		/// <value>The record.</value>
		[XmlElement("record")]
		public Record Record { get; set; }
	}
}
