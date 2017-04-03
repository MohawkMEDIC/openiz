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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenIZ.Messaging.CSD.Model;
using System.Xml;
using OpenIZ.Messaging.CSD.Model.Identifier;

namespace OpenIZ.Messaging.CSD.Message
{
	/// <summary>
	/// Represents a care services function.
	/// </summary>
	[XmlType("careServicesFunction", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	[XmlRoot("careServicesFunction", Namespace = "urn:ihe:iti:csd:2013", IsNullable = false)]
	public class CareServicesFunction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CareServicesFunction"/> class.
		/// </summary>
		public CareServicesFunction()
		{
			
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[XmlElement("description")]
		public object Description { get; set; }

		/// <summary>
		/// Gets or sets the definition.
		/// </summary>
		/// <value>The definition.</value>
		[XmlElement("definition")]
		public string Definition { get; set; }

		/// <summary>
		/// Gets or sets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		[XmlElement("extension")]
		public List<Extension> Extensions { get; set; }

		/// <summary>
		/// Gets or sets the urn.
		/// </summary>
		/// <value>The urn.</value>
		[XmlAttribute("urn")]
		public string Urn { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		[XmlAttribute("content-type")]
		public string ContentType { get; set; }

		/// <summary>
		/// Gets or sets any attribute.
		/// </summary>
		/// <value>Any attribute.</value>
		[XmlAnyAttribute]
		public List<XmlAttribute> AnyAttr { get; set; }
	}
}
