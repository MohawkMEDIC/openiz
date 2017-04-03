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

namespace OpenIZ.Messaging.CSD.Model.Association
{
	/// <summary>
	/// Represents a provider facility association.
	/// </summary>
	/// <seealso cref="UniqueId" />
	[XmlType("providerFacility", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	public class ProviderFacility : UniqueId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProviderFacility"/> class.
		/// </summary>
		public ProviderFacility()
		{
			
		}

		/// <summary>
		/// Gets or sets the provider facility services.
		/// </summary>
		/// <value>The provider facility services.</value>
		[XmlElement("service")]
		public List<ProviderFacilityService> ProviderFacilityServices { get; set; }

		/// <summary>
		/// Gets or sets the operating hours.
		/// </summary>
		/// <value>The operating hours.</value>
		[XmlElement("operatingHours")]
		public List<OperatingHours> OperatingHours { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }
	}
}