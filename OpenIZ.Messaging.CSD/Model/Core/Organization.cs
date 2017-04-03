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
using OpenIZ.Messaging.CSD.Model.Contact;
using OpenIZ.Messaging.CSD.Model.Identifier;
using OpenIZ.Messaging.CSD.Model.Location;
using OpenIZ.Messaging.CSD.Model.Naming;

namespace OpenIZ.Messaging.CSD.Model.Core
{
	/// <summary>
	/// Represnts an organization.
	/// </summary>
	/// <seealso cref="UniqueId" />
	[XmlType("organization", Namespace = "urn:ihe:iti:csd:2013")]
	public class Organization : UniqueId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Organization"/> class.
		/// </summary>
		public Organization()
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
		/// Gets or sets the primary name.
		/// </summary>
		/// <value>The primary name.</value>
		[XmlElement("primaryName")]
		public string PrimaryName { get; set; }

		/// <summary>
		/// Gets or sets the organization other names.
		/// </summary>
		/// <value>The organization other names.</value>
		[XmlElement("otherName")]
		public List<OrganizationOtherName> OrganizationOtherNames { get; set; }

		/// <summary>
		/// Gets or sets the addresses.
		/// </summary>
		/// <value>The addresses.</value>
		[XmlElement("address")]
		public List<Address> Addresses { get; set; }

		/// <summary>
		/// Gets or sets the organization contacts.
		/// </summary>
		/// <value>The organization contacts.</value>
		[XmlElement("contact")]
		public List<OrganizationContact> OrganizationContacts { get; set; }

		/// <summary>
		/// Gets or sets the credentials.
		/// </summary>
		/// <value>The credentials.</value>
		[XmlElement("credential")]
		public List<Credential> Credentials { get; set; }

		/// <summary>
		/// Gets or sets the languages.
		/// </summary>
		/// <value>The languages.</value>
		[XmlElement("language")]
		public List<CodedType> Languages { get; set; }

		/// <summary>
		/// Gets or sets the specializations.
		/// </summary>
		/// <value>The specializations.</value>
		[XmlElement("specialization")]
		public List<CodedType> Specializations { get; set; }

		/// <summary>
		/// Gets or sets the contact points.
		/// </summary>
		/// <value>The contact points.</value>
		[XmlElement("contactPoint")]
		public List<ContactPoint> ContactPoints { get; set; }

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[XmlElement("parent")]
		public UniqueId Parent { get; set; }

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