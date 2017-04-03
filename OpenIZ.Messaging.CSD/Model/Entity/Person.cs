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
using System.Xml.Serialization;
using OpenIZ.Messaging.CSD.Model.Contact;
using OpenIZ.Messaging.CSD.Model.Location;
using OpenIZ.Messaging.CSD.Model.Naming;

namespace OpenIZ.Messaging.CSD.Model.Entity
{
	/// <summary>
	/// Represents a person.
	/// </summary>
	[XmlType("person", Namespace = "urn:ihe:iti:csd:2013")]
	public class Person
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Person"/> class.
		/// </summary>
		public Person()
		{
			
		}

		/// <summary>
		/// Gets or sets the names.
		/// </summary>
		/// <value>The names.</value>
		[XmlElement("name")]
		public List<PersonName> Names { get; set; }

		/// <summary>
		/// Gets or sets the contact points.
		/// </summary>
		/// <value>The contact points.</value>
		[XmlElement("contactPoint")]
		public List<ContactPoint> ContactPoints { get; set; }

		/// <summary>
		/// Gets or sets the addresses.
		/// </summary>
		/// <value>The addresses.</value>
		[XmlElement("address")]
		public List<Address> Addresses { get; set; }

		/// <summary>
		/// Gets or sets the gender.
		/// </summary>
		/// <value>The gender.</value>
		[XmlElement("gender")]
		public string Gender { get; set; }

		/// <summary>
		/// Gets or sets the date of birth.
		/// </summary>
		/// <value>The date of birth.</value>
		[XmlElement("dateOfBirth", DataType = "date")]
		public DateTime DateOfBirth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [date of birth specified].
		/// </summary>
		/// <value><c>true</c> if [date of birth specified]; otherwise, <c>false</c>.</value>
		[XmlIgnore]
		public bool DateOfBirthSpecified { get; set; }
	}
}
