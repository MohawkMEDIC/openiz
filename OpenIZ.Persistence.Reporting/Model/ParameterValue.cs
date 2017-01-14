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
 * Date: 2017-1-6
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIZ.Persistence.Reporting.Model
{
	/// <summary>
	/// Represents a parameter value.
	/// </summary>
	public class ParameterValue
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValue"/> class.
		/// </summary>
		public ParameterValue()
		{
			this.CreationTime = DateTimeOffset.UtcNow;
			this.Id = Guid.NewGuid();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValue"/> class
		/// with a specific value.
		/// </summary>
		/// <param name="value">The value of the report parameter.</param>
		public ParameterValue(object value) : this()
		{
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the creation time of the parameter.
		/// </summary>
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the id of the parameter.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the report parameter reference associated with the parameter value.
		/// </summary>
		[ForeignKey("ReportParameterId")]
		public virtual ReportParameter ReportParameter { get; set; }

		/// <summary>
		/// Gets or sets the report parameter id associated with the parameter value.
		/// </summary>
		[Required]
		public Guid ReportParameterId { get; set; }

		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		public object Value { get; set; }
	}
}