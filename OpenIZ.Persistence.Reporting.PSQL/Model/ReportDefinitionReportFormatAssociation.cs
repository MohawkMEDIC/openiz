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
	/// Represents a report definition report format association.
	/// </summary>
	public class ReportDefinitionReportFormatAssociation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinitionReportFormatAssociation"/> class.
		/// </summary>
		public ReportDefinitionReportFormatAssociation()
		{
			
		}

		/// <summary>
		/// Gets or sets the report definition identifier.
		/// </summary>
		/// <value>The report definition identifier.</value>
		public Guid ReportDefinitionId { get; set; }

		/// <summary>
		/// Gets or sets the report format identifier.
		/// </summary>
		/// <value>The report format identifier.</value>
		public Guid ReportFormatId { get; set; }

		/// <summary>
		/// Gets or sets the report definition.
		/// </summary>
		/// <value>The report definition.</value>
		public virtual ReportDefinition ReportDefinition { get; set; }

		/// <summary>
		/// Gets or sets the report format.
		/// </summary>
		/// <value>The report format.</value>
		public virtual ReportFormat ReportFormat { get; set; }
	}
}
