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
using OpenIZ.Messaging.CSD.Model.Core;
using OpenIZ.Messaging.CSD.Model.Identifier;
using OpenIZ.Messaging.CSD.Model.Location;

namespace OpenIZ.Messaging.CSD.Model.Association
{
	/// <summary>
	/// Represents a provider organization association.
	/// </summary>
	/// <seealso cref="UniqueId" />
	[XmlType("providerOrganization", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	public class ProviderOrganization : UniqueId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProviderOrganization" /> class.
		/// </summary>
		public ProviderOrganization()
		{
			
		}

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets or sets the addresses.
		/// </summary>
		/// <value>The addresses.</value>
		[XmlElement("address")]
		public List<Address> Addresses { get; set; }

		/// <summary>
		/// Gets or sets the contact points.
		/// </summary>
		/// <value>The contact points.</value>
		[XmlElement("contactPoint")]
		public List<ContactPoint> ContactPoints { get; set; }
	}
}