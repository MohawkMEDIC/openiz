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
 * Date: 2017-4-4
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Reporting.Jasper.Model.DataSource
{
	/// <summary>
	/// Represents a virtual data source.
	/// </summary>
	/// <seealso cref="OpenIZ.Reporting.Jasper.Model.ResourceBase" />
	[XmlType("virtualDataSource")]
	[XmlRoot("virtualDataSource")]
	public class VirtualDataSource : ResourceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VirtualDataSource"/> class.
		/// </summary>
		public VirtualDataSource()
		{
			
		}

		/// <summary>
		/// Gets or sets the sub data sources.
		/// </summary>
		/// <value>The sub data sources.</value>
		[XmlElement("subDataSources")]
		public List<SubDataSource> SubDataSources { get; set; }
	}
}
