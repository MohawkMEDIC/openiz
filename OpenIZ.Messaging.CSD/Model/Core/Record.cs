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
using System.Xml.Serialization;

namespace OpenIZ.Messaging.CSD.Model.Core
{
	/// <summary>
	/// Represents a record.
	/// </summary>
	[XmlType("record", Namespace = "urn:ihe:iti:csd:2013")]
	public class Record
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Record"/> class.
		/// </summary>
		public Record()
		{
			
		}

		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		[XmlAttribute("created")]
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets or sets the updated.
		/// </summary>
		/// <value>The updated.</value>
		[XmlAttribute("updated")]
		public DateTime Updated { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		[XmlAttribute("status")]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the source directory.
		/// </summary>
		/// <value>The source directory.</value>
		[XmlAttribute("sourceDirectory", DataType = "anyURI")]
		public string SourceDirectory { get; set; }
	}
}