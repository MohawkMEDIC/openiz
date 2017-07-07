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
 * User: justi
 * Date: 2017-4-1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model
{
	/// <summary>
	/// Represents a resource lookup.
	/// </summary>
	[XmlType("resourceLookup")]
	public class ResourceLookup
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceLookup"/> class.
		/// </summary>
		public ResourceLookup()
		{
			
		}

		/// <summary>
		/// Gets or sets the creation time.
		/// </summary>
		/// <value>The creation time.</value>
		[XmlElement("creationDate")]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		/// <value>The label.</value>
		[XmlElement("label")]
		public string Label { get; set; }
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[XmlElement("description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the URI.
		/// </summary>
		/// <value>The URI.</value>
		[XmlElement("uri")]
		public string Uri { get; set; }

		/// <summary>
		/// Gets or sets the type of the resource.
		/// </summary>
		/// <value>The type of the resource.</value>
		[XmlElement("resourceType")]
		public string ResourceType { get; set; }

		/// <summary>
		/// Gets or sets the permission mask.
		/// </summary>
		/// <value>The permission mask.</value>
		[XmlElement("permissionMask")]
		public int PermissionMask { get; set; }

		/// <summary>
		/// Gets or sets the update time.
		/// </summary>
		/// <value>The update time.</value>
		[XmlElement("updateDate")]
		public DateTime UpdateTime { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		[XmlElement("version")]
		public int Version { get; set; }
	}
}
