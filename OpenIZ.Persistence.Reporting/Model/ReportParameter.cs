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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIZ.Persistence.Reporting.Model
{
	/// <summary>
	/// Represents a report parameter.
	/// </summary>
	public class ReportParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class.
		/// </summary>
		public ReportParameter()
		{
			this.DefaultValues = new List<ParameterValue>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportParameter"/> class
		/// with a specific <see cref="Core.Model.RISI.ReportParameter"/> instance.
		/// </summary>
		/// <param name="reportParameter">The report parameter instance.</param>
		public ReportParameter(Core.Model.RISI.ReportParameter reportParameter) : this()
		{
			this.DataTypeId = reportParameter.DataType.Key.Value;
			this.DefaultValues.Add(new ParameterValue(reportParameter.Value));
			this.Id = reportParameter.Key.Value;
			this.IsNullable = reportParameter.IsNullable;
			this.Name = reportParameter.Name;
		}

		/// <summary>
		/// Gets or sets the creation time of the parameter.
		/// </summary>
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the data type reference associated with the report parameter.
		/// </summary>
		[ForeignKey("DataTypeId")]
		public virtual ParameterType DataType { get; set; }

		/// <summary>
		/// Gets or sets the id of the data type associated with the report parameter.
		/// </summary>
		[Required]
		public Guid DataTypeId { get; set; }

		/// <summary>
		/// Gets or sets the default values associated with the report parameter.
		/// </summary>
		public virtual ICollection<ParameterValue> DefaultValues { get; set; }

		/// <summary>
		/// Gets or sets the id of the parameter.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets whether the report parameter is nullable.
		/// </summary>
		[Required]
		public bool IsNullable { get; set; }

		/// <summary>
		/// Gets or sets the name of the report parameter.
		/// </summary>
		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the order of the report parameter.
		/// </summary>
		[Required]
		public int Order { get; set; }

		/// <summary>
		/// Gets or sets the report reference associated with the report parameter.
		/// </summary>
		[ForeignKey("ReportId")]
		public virtual ReportDefinition ReportDefinition { get; set; }

		/// <summary>
		/// Gets or sets the report ID associated with the report parameter.
		/// </summary>
		[Required]
		public Guid ReportId { get; set; }
	}
}