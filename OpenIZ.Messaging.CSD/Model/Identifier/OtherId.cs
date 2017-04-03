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

namespace OpenIZ.Messaging.CSD.Model.Identifier
{
	/// <summary>
	/// Represents an other id.
	/// </summary>
	[XmlType("otherID", Namespace = "urn:ihe:iti:csd:2013")]
	public class OtherId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OtherId"/> class.
		/// </summary>
		public OtherId()
		{
			
		}

		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		[XmlAttribute("code")]
		public string Code { get; set; }

		/// <summary>
		/// Gets or sets the name of the assigning authority.
		/// </summary>
		/// <value>The name of the assigning authority.</value>
		[XmlAttribute("assigningAuthorityName")]
		public string AssigningAuthorityName { get; set; }
	}
}