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
using OpenIZ.OrmLite.Attributes;

namespace OpenIZ.Persistence.Reporting.PSQL.Model
{
	/// <summary>
	/// Represents a report definition report format association.
	/// </summary>
	[Table("report_definition_report_format_association")]
	public class ReportDefinitionReportFormatAssociation : DbAssociation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinitionReportFormatAssociation"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public ReportDefinitionReportFormatAssociation(Guid key) : base(key)
		{
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[Column("report_format_id")]
		[ForeignKey(typeof(ReportFormat), nameof(ReportFormat.Key))]
		public override Guid Key { get; set; }

		/// <summary>
		/// Gets or sets the source key.
		/// </summary>
		/// <value>The source key.</value>
		[Column("report_definition_id")]
		[ForeignKey(typeof(ReportDefinition), nameof(ReportDefinition.Key))]
		public override Guid SourceKey { get; set; }
	}
}