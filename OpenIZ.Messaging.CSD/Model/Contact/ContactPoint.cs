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

using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Model.Contact
{
	/// <summary>
	/// Represents a contact point.
	/// </summary>
	[XmlType("contactPoint", Namespace = "urn:ihe:iti:csd:2013")]
	public class ContactPoint
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContactPoint"/> class.
		/// </summary>
		public ContactPoint()
		{
			
		}

		/// <summary>
		/// Gets or sets the type of the coded.
		/// </summary>
		/// <value>The type of the coded.</value>
		[XmlElement("codedType")]
		public CodedType CodedType { get; set; }

		/// <summary>
		/// Gets or sets the equipment.
		/// </summary>
		/// <value>The equipment.</value>
		[XmlElement("equipment")]
		public string Equipment { get; set; }

		/// <summary>
		/// Gets or sets the purpose.
		/// </summary>
		/// <value>The purpose.</value>
		[XmlElement("purpose")]
		public string Purpose { get; set; }

		/// <summary>
		/// Gets or sets the certificate.
		/// </summary>
		/// <value>The certificate.</value>
		[XmlElement("certificate")]
		public string Certificate { get; set; }
	}
}