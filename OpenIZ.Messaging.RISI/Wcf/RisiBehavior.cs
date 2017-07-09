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
 * Date: 2016-8-28
 */

using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Reporting.Core;
using System;
using System.IO;
using System.ServiceModel;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceBehavior(ConfigurationName = "RISI")]
	public partial class RisiBehavior : IRisiContract
	{
		/// <summary>
		/// The internal reference to the <see cref="IReportExecutor"/> instance.
		/// </summary>
		private readonly IReportExecutor reportExecutor = ApplicationContext.Current.GetService<IReportExecutor>();

		/// <summary>
		/// Initializes a new instance of the <see cref="RisiBehavior"/> class.
		/// </summary>
		public RisiBehavior()
		{
		}

		/// <summary>
		/// Creates a new parameter type.
		/// </summary>
		/// <param name="parameterType">The parameter type to create.</param>
		/// <returns>Returns the created parameter type.</returns>
		public ParameterType CreateParameterType(ParameterType parameterType)
		{
			return this.reportExecutor.CreateParameterType(parameterType);
		}

		/// <summary>
		/// Creates a new report definition.
		/// </summary>
		/// <param name="reportDefinition">The report definition to create.</param>
		/// <returns>Returns the created report definition.</returns>
		public ReportDefinition CreateReportDefinition(ReportDefinition reportDefinition)
		{
			return this.reportExecutor.CreateReportDefinition(reportDefinition);
		}

		/// <summary>
		/// Creates a report format.
		/// </summary>
		/// <param name="reportFormat">The report format to create.</param>
		/// <returns>Returns the created report format.</returns>
		public ReportFormat CreateReportFormat(ReportFormat reportFormat)
		{
			return this.reportExecutor.CreateReportFormat(reportFormat);
		}

		/// <summary>
		/// Deletes a report parameter type.
		/// </summary>
		/// <param name="id">The id of the report parameter type to delete.</param>
		/// <returns>Returns the deleted report parameter type.</returns>
		public ParameterType DeleteParameterType(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.DeleteParameterType(key);
		}

		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		public ReportDefinition DeleteReportDefinition(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.DeleteReportDefinition(key);
		}

		/// <summary>
		/// Deletes a report format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <returns>Returns the report deleted report format.</returns>
		public ReportFormat DeleteReportFormat(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.DeleteReportFormat(key);
		}

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		public RisiCollection<ParameterType> GetAllReportParameterTypes()
		{
			return this.reportExecutor.GetAllReportParameterTypes();
		}

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		public ReportDefinition GetReportDefinition(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.GetReportDefinition(key);
		}

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		public RisiCollection<ReportDefinition> GetReportDefinitions()
		{
			return this.reportExecutor.GetReportDefinitions();
		}

		/// <summary>
		/// Gets a report format by id.
		/// </summary>
		/// <param name="id">The id of the report format to retrieve.</param>
		/// <returns>Returns a report format.</returns>
		public ReportFormat GetReportFormat(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.GetReportFormat(key);
		}

		/// <summary>
		/// Gets the report formats.
		/// </summary>
		/// <returns>Returns a list of report formats.</returns>
		public RisiCollection<ReportFormat> GetReportFormats()
		{
			return reportExecutor.GetReportFormats();
		}

		/// <summary>
		/// Gets a report parameter by id.
		/// </summary>
		/// <param name="id">The id of the report parameter to retrieve.</param>
		/// <returns>Returns a report parameter.</returns>
		public ReportParameter GetReportParameter(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.GetReportParameter(key);
		}

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		public RisiCollection<ReportParameter> GetReportParameters(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.GetReportParameters(key);
		}

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		public AutoCompleteSourceDefinition GetReportParameterValues(string id, string parameterId)
		{
			var reportKey = Guid.Empty;
			var parameterKey = Guid.Empty;

			if (!Guid.TryParse(id, out reportKey))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			if (!Guid.TryParse(parameterId, out parameterKey))
			{
				throw new ArgumentException($"The parameter { parameterId } must be a valid { nameof(Guid) }");
			}

			return this.reportExecutor.GetReportParameterValues(reportKey, parameterKey);
		}

		/// <summary>
		/// Gets the report source.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve the source.</param>
		/// <returns>Returns the report source.</returns>
		/// <exception cref="System.ArgumentException">If the id is not in a valid format.</exception>
		public Stream GetReportSource(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return new MemoryStream(this.reportExecutor.GetReportSource(key));
		}

		/// <summary>
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="bundle">The list of parameters of the report.</param>
		/// <returns>Returns the report in raw format.</returns>
		/// <exception cref="System.ArgumentException">If the id or format is not in a valid format.</exception>
		public Stream RunReport(string id, string format, ReportBundle bundle)
		{
			var reportId = Guid.Empty;
			var formatId = Guid.Empty;

			if (!Guid.TryParse(id, out reportId))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			if (!Guid.TryParse(format, out formatId))
			{
				throw new ArgumentException($"The parameter { format } must be a valid { nameof(Guid) }");
			}

			return new MemoryStream(this.reportExecutor.RunReport(reportId, formatId, bundle.Parameters.Items));
		}

		/// <summary>
		/// Updates a parameter type definition.
		/// </summary>
		/// <param name="id">The id of the parameter type.</param>
		/// <param name="parameterType">The parameter type.</param>
		/// <returns>Returns the updated parameter type definition.</returns>
		public ParameterType UpdateParameterType(string id, ParameterType parameterType)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			if (key != parameterType.Key)
			{
				throw new ArgumentException($"Unable to update parameter type using id: {id}, and id: {parameterType.Key}");
			}

			return this.reportExecutor.UpdateParameterType(parameterType);
		}

		/// <summary>
		/// Updates a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to update.</param>
		/// <param name="reportDefinition">The updated report definition.</param>
		/// <returns>Returns the updated report definition.</returns>
		public ReportDefinition UpdateReportDefinition(string id, ReportDefinition reportDefinition)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			if (key != reportDefinition.Key)
			{
				throw new ArgumentException($"Unable to update report definition using id: {id}, and id: {reportDefinition.Key}");
			}

			return this.reportExecutor.UpdateReportDefinition(reportDefinition);
		}

		/// <summary>
		/// Updates a report format.
		/// </summary>
		/// <param name="id">The id of the report format to update.</param>
		/// <param name="reportFormat">The updated report format.</param>
		/// <returns>Returns the update report format.</returns>
		public ReportFormat UpdateReportFormat(string id, ReportFormat reportFormat)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			if (key != reportFormat.Key)
			{
				throw new ArgumentException($"Unable to update report format using id: {id}, and id: {reportFormat.Key}");
			}

			return this.reportExecutor.UpdateReportFormat(reportFormat);
		}
	}
}