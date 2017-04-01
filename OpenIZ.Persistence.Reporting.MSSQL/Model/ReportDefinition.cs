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
using System.Linq;

namespace OpenIZ.Persistence.Reporting.MSSQL.Model
{
	/// <summary>
	/// Represents a report definition.
	/// </summary>
	public class ReportDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinition"/> class.
		/// </summary>
		public ReportDefinition()
		{
			this.CreationTime = DateTimeOffset.UtcNow;
			this.Id = Guid.NewGuid();
			this.Parameters = new List<ReportParameter>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinition"/> class
		/// with a specific <see cref="Core.Model.RISI.ReportDefinition"/> instance.
		/// </summary>
		/// <param name="report">The report instance.</param>
		public ReportDefinition(Core.Model.RISI.ReportDefinition report) : this()
		{
			this.Author = report.CreatedBy.UserName;
			this.Description = report.Description;
			this.Id = report.Key.Value;
			this.Parameters = report.Parameters.Select(p => new ReportParameter(p)).ToList();
		}

		/// <summary>
		/// Gets or sets the author of the report.
		/// </summary>
		[Required]
		[StringLength(256)]
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the correlation id of the report.
		/// </summary>
		[Required]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Gets or sets the creation time of the report.
		/// </summary>
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the description of the report.
		/// </summary>
		[StringLength(1024)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the id of the report.
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the report.
		/// </summary>
		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the report parameters associated with the report.
		/// </summary>
		public virtual ICollection<ReportParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the report formats.
		/// </summary>
		/// <value>The report formats.</value>
		public virtual ICollection<ReportDefinitionReportFormatAssociation> ReportFormats { get; set; }
	}
}