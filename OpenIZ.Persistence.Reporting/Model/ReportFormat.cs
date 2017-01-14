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
 * Date: 2017-1-13
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
	/// Represents a report format.
	/// </summary>
	public class ReportFormat
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportFormat"/> class.
		/// </summary>
		public ReportFormat()
		{
			this.CreationTime = DateTimeOffset.UtcNow;
			this.Id = Guid.NewGuid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportFormat"/> class
		/// with a specific report format name.
		/// </summary>
		/// <param name="name">The name of the report format.</param>
		public ReportFormat(string name) : this()
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the creation time of the report format.
		/// </summary>
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the report format.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the report format.
		/// </summary>
		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the report definitions associated with the report format.
		/// </summary>
		public virtual ICollection<ReportDefinition> ReportDefinitions { get; set; }
	}
}