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
 * Date: 2017-1-9
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Reporting.Model
{
	/// <summary>
	/// Represents a data type.
	/// </summary>
	[Table("data_type")]
	public class DataType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataType"/> class.
		/// </summary>
		public DataType()
		{
			this.CreationTime = DateTimeOffset.UtcNow;
			this.Id = Guid.NewGuid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataType"/> class
		/// with a specific name.
		/// </summary>
		/// <param name="name">The name of the data type.</param>
		public DataType(string name) : this()
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the creation time of the report.
		/// </summary>
		[Required]
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the report.
		/// </summary>
		[Key]
		[Column("id")]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the data type.
		/// </summary>
		[Required]
		[Column("name")]
		[StringLength(128)]
		public string Name { get; set; }
	}
}
