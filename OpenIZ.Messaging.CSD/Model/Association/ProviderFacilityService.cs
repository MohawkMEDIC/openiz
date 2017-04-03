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
using OpenIZ.Messaging.CSD.Model.Naming;

namespace OpenIZ.Messaging.CSD.Model.Association
{
	/// <summary>
	/// Represents a provider facility service association.
	/// </summary>
	[XmlType("providerFacilityService", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	public class ProviderFacilityService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProviderFacilityService"/> class.
		/// </summary>
		public ProviderFacilityService()
		{
			
		}

		/// <summary>
		/// Gets or sets the names.
		/// </summary>
		/// <value>The names.</value>
		[XmlElement("name")]
		public List<Name> Names { get; set; }

		/// <summary>
		/// Gets or sets the organization.
		/// </summary>
		/// <value>The organization.</value>
		[XmlElement("organization")]
		public UniqueId Organization { get; set; }

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		[XmlElement("language")]
		public List<CodedType> Language { get; set; }

		/// <summary>
		/// Gets or sets the operating hours.
		/// </summary>
		/// <value>The operating hours.</value>
		[XmlElement("operationHours")]
		public List<OperatingHours> OperatingHours { get; set; }

		/// <summary>
		/// Gets or sets the free busy URI.
		/// </summary>
		/// <value>The free busy URI.</value>
		[XmlElement("freeBusyURI", DataType = "anyURI")]
		public string FreeBusyUri { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }
	}
}