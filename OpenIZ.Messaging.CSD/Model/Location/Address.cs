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

namespace OpenIZ.Messaging.CSD.Model.Location
{
	/// <summary>
	/// Represents an address
	/// </summary>
	[XmlType("address", Namespace = "urn:ihe:iti:csd:2013")]
	public class Address
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Address"/> class.
		/// </summary>
		public Address()
		{
			
		}

		/// <summary>
		/// Gets or sets the address lines.
		/// </summary>
		/// <value>The address lines.</value>
		[XmlElement("addressLine")]
		public List<AddressLine> AddressLines { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[XmlAttribute("type")]
		public string Type { get; set; }
	}
}
