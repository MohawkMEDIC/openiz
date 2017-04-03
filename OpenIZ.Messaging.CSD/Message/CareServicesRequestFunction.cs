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
using System.Xml;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Message
{
	/// <summary>
	/// Represents a care services request function.
	/// </summary>
	[XmlType("careServicesRequestFunction",AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	public class CareServicesRequestFunction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CareServicesRequestFunction"/> class.
		/// </summary>
		public CareServicesRequestFunction()
		{
			
		}

		/// <summary>
		/// Gets or sets any.
		/// </summary>
		/// <value>Any.</value>
		[XmlAnyElement]
		public XmlElement Any { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CareServicesRequestExpression"/> is encapsulated.
		/// </summary>
		/// <value><c>true</c> if encapsulated; otherwise, <c>false</c>.</value>
		[XmlAttribute("encapsulated")]
		public bool Encapsulated { get; set; }

		/// <summary>
		/// Gets or sets the urn.
		/// </summary>
		/// <value>The urn.</value>
		[XmlAttribute("urn")]
		public string Urn { get; set; }

		/// <summary>
		/// Gets or sets any attribute.
		/// </summary>
		/// <value>Any attribute.</value>
		[XmlAnyAttribute]
		public List<XmlAttribute> AnyAttr { get; set; }
	}
}