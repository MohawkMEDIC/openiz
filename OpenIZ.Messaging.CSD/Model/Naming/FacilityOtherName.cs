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

using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Model.Naming
{
	/// <summary>
	/// Represents a facility other name.
	/// </summary>
	[XmlType("facilityOtherName", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	public class FacilityOtherName
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FacilityOtherName"/> class.
		/// </summary>
		public FacilityOtherName()
		{
			
		}

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		[XmlAttribute("lang", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string Language { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlText]
		public string Value { get; set; }
	}
}