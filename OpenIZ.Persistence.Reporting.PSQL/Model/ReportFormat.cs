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
using System.Collections.Generic;

namespace OpenIZ.Persistence.Reporting.PSQL.Model
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
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the report format.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the report format.
		/// </summary>
		public string Name { get; set; }
	}
}