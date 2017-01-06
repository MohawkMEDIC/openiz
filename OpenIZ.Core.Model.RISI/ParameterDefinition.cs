/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-12-4
 */

using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.RISI
{
	/// <summary>
	/// Represents a parameter definition.
	/// </summary>
	[XmlType(nameof(ParameterDefinition), Namespace = "http://openiz.org/risi")]
	public class ParameterDefinition : BaseEntityData
	{
		protected ParameterDefinition()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterDefinition"/> class
		/// with a specific name and type.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="typeId">The type of the parameter.</param>
		public ParameterDefinition(string name, Guid typeId)
		{
			this.Name = name;
			this.TypeId = typeId;
		}

		/// <summary>
		/// Gets or sets whether the parameter is a required parameter.
		/// </summary>
		[XmlElement("isRequired")]
		public bool IsRequired { get; set; }

		/// <summary>
		/// Gets or sets the name of the parameter.
		/// </summary>
		[XmlElement("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type of parameter.
		/// </summary>
		[XmlElement("type")]
		public Guid TypeId { get; set; }
	}
}