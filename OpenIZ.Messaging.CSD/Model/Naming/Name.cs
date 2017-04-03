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

namespace OpenIZ.Messaging.CSD.Model.Naming
{
	/// <summary>
	/// Represents a name.
	/// </summary>
	[XmlType("name", Namespace = "urn:ihe:iti:csd:2013")]
	public class Name
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Name"/> class.
		/// </summary>
		public Name()
		{
			
		}

		/// <summary>
		/// Gets or sets the common names.
		/// </summary>
		/// <value>The common names.</value>
		[XmlElement("commonName")]
		public List<CommonName> CommonNames { get; set; }

		/// <summary>
		/// Gets or sets the honorific.
		/// </summary>
		/// <value>The honorific.</value>
		[XmlElement("honorific")]
		public string Honorific { get; set; }

		/// <summary>
		/// Gets or sets the forename.
		/// </summary>
		/// <value>The forename.</value>
		[XmlElement("forename")]
		public string Forename { get; set; }

		/// <summary>
		/// Gets or sets the other names.
		/// </summary>
		/// <value>The other names.</value>
		[XmlElement("otherName")]
		public List<CodedType> OtherNames { get; set; }

		/// <summary>
		/// Gets or sets the surname.
		/// </summary>
		/// <value>The surname.</value>
		[XmlElement("surname")]
		public string Surname { get; set; }

		/// <summary>
		/// Gets or sets the suffix.
		/// </summary>
		/// <value>The suffix.</value>
		[XmlElement("suffix")]
		public string Suffix { get; set; }
	}
}
