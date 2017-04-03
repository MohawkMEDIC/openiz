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

namespace OpenIZ.Messaging.CSD.Message
{
	/// <summary>
	/// Represents a care services request.
	/// </summary>
	[XmlType("careServicesRequest", AnonymousType = true, Namespace = "urn:ihe:iti:csd:2013")]
	[XmlRoot("careServicesRequest", IsNullable = false, Namespace = "urn:ihe:iti:csd:2013")]
	public class CareServicesRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CareServicesRequest"/> class.
		/// </summary>
		public CareServicesRequest()
		{
			
		}

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		[XmlElement("expression", typeof(CareServicesRequestExpression))]
		[XmlElement("function", typeof(CareServicesRequestFunction))]
		public object Item { get; set; }
	}
}
