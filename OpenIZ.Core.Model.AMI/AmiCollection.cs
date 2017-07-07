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
 * User: khannan
 * Date: 2016-8-2
 */
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
	/// <summary>
	/// Represents an administrative collection item.
	/// </summary>
	/// <typeparam name="T">The type of collection of the collection items.</typeparam>
	[XmlType(Namespace = "http://openiz.org/ami")]
	public class AmiCollection<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AmiCollection{T}"/> class.
		/// </summary>
		public AmiCollection()
		{
			this.CollectionItem = new List<T>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AmiCollection{T}"/> class
		/// with a specific list of collection items.
		/// </summary>
		public AmiCollection(List<T> collectionItems)
		{
			this.CollectionItem = collectionItems;
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="AmiCollection{T}"/> class
        /// with a specific list of collection items.
        /// </summary>
        public AmiCollection(IEnumerable<T> collectionItems)
        {
            this.CollectionItem = new List<T>(collectionItems);
        }

        /// <summary>
        /// Gets or sets a list of collection items.
        /// </summary>
        [XmlElement("item")]
		public List<T> CollectionItem { get; set; }

		/// <summary>
		/// Gets or sets the total offset.
		/// </summary>
		[XmlAttribute("offset")]
		public int Offset { get; set; }

		/// <summary>
		/// Gets or sets the total collection size.
		/// </summary>
		[XmlAttribute("size")]
		public int Size { get; set; }
	}
}