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
 * Date: 2017-1-13
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a RISI collection.
	/// </summary>
	/// <typeparam name="T">The type of the collection.</typeparam>
	[XmlType(Namespace = "http://openiz.org/risi")]
	public class RisiCollection<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RisiCollection{T}"/> class.
		/// </summary>
		public RisiCollection()
		{
			this.Items = new List<T>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RisiCollection{T}"/> class
		/// with a specific <see cref="List{T}"/> of items.
		/// </summary>
		/// <param name="items">The list of items.</param>
		public RisiCollection(IEnumerable<T> items)
		{
			this.Size = items.Count();
			this.Items = items.ToList();
		}

		/// <summary>
		/// Gets or sets the list of items in the <see cref="RisiCollection{T}"/>.
		/// </summary>
		[XmlElement("item")]
		public List<T> Items { get; set; }

		/// <summary>
		/// Gets or sets the count of the items.
		/// </summary>
		[XmlAttribute("size")]
		public int Size { get; set; }
	}
}
