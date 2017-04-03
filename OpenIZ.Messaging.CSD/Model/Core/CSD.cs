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

namespace OpenIZ.Messaging.CSD.Model.Core
{
	/// <summary>
	/// Represents a care services discovery document.
	/// </summary>
	[XmlType("csd", Namespace = "urn:ihe:iti:csd:2013")]
	[XmlRoot("csd", Namespace = "urn:ihe:iti:csd:2013")]
	public class CSD
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CSD"/> class.
		/// </summary>
		public CSD()
		{
			
		}

		/// <summary>
		/// Gets or sets the organizations.
		/// </summary>
		/// <value>The organizations.</value>
		[XmlArrayItem("organizationDirectory", IsNullable = false)]
		public List<Organization> Organizations { get; set; }

		/// <summary>
		/// Gets or sets the providers.
		/// </summary>
		/// <value>The providers.</value>
		[XmlArrayItem("providerDirectory", IsNullable = false)]
		public List<Provider> Providers { get; set; }

		/// <summary>
		/// Gets or sets the facilities.
		/// </summary>
		/// <value>The facilities.</value>
		[XmlArrayItem("facilityDirectory", IsNullable = false)]
		public List<Facility> Facilities { get; set; }

		/// <summary>
		/// Gets or sets the services.
		/// </summary>
		/// <value>The services.</value>
		[XmlArrayItem("serviceDirectory", IsNullable = false)]
		public List<Service> Services { get; set; }
	}
}
