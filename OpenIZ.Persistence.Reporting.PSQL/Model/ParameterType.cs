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
 * Date: 2017-4-1
 */

using System;

namespace OpenIZ.Persistence.Reporting.PSQL.Model
{
	/// <summary>
	/// Represents a parameter type.
	/// </summary>
	public class ParameterType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterType"/> class.
		/// </summary>
		public ParameterType()
		{
			this.CreationTime = DateTimeOffset.UtcNow;
			this.Id = Guid.NewGuid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterType"/> class
		/// with a specific name.
		/// </summary>
		/// <param name="type">The type of parameter.</param>
		public ParameterType(string type) : this()
		{
			this.Type = type;
		}

		/// <summary>
		/// Gets or sets the creation time of the parameter type.
		/// </summary>
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the parameter type.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the type of the parameter type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the values provider of the parameter type.
		/// </summary>
		public string ValuesProvider { get; set; }
	}
}