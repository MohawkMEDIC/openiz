/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-8-28
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Messaging.RISI.Configuration;
using OpenIZ.Reporting.Core;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceBehavior(ConfigurationName = "RISI")]
	public class RisiBehavior : IRisiContract
	{
		/// <summary>
		/// The internal reference to the RISI configuration.
		/// </summary>
		private static readonly RisiConfiguration configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.messaging.risi") as RisiConfiguration;

		/// <summary>
		/// The internal reference to the <see cref="IReportHandler"/> instance.
		/// </summary>
		private readonly IReportHandler reportHandler = ApplicationContext.Current.GetService<IReportHandler>();

		/// <summary>
		/// Initializes a new instance of the <see cref="RisiBehavior"/> class.
		/// </summary>
		public RisiBehavior()
		{
			this.reportHandler.ReportUri = new Uri(configuration.Address);
		}

		/// <summary>
		/// Creates a new parameter type.
		/// </summary>
		/// <param name="parameterType">The parameter type to create.</param>
		/// <returns>Returns the created parameter type.</returns>
		public ParameterType CreateParameterType(ParameterType parameterType)
		{
			return this.reportHandler.CreateParameterType(parameterType);
		}

		/// <summary>
		/// Creates a new report definition.
		/// </summary>
		/// <param name="reportDefinition">The report definition to create.</param>
		/// <returns>Returns the created report definition.</returns>
		public ReportDefinition CreateReportDefinition(ReportDefinition reportDefinition)
		{
			return this.reportHandler.CreateReportDefinition(reportDefinition);
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

			return this.reportHandler.DeleteParameterType(key);
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

			return this.reportHandler.DeleteReportDefinition(key);
		}

		/// <summary>
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="parameters">The list of parameters of the report.</param>
		/// <returns>Returns the report in raw format.</returns>
		public byte[] ExecuteReport(string id, string format, List<ReportParameter> parameters)
		{
			Guid reportId;

			if (!Guid.TryParse(id, out reportId))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		public List<ReportParameter> GetAllReportParamterTypes()
		{
			throw new NotImplementedException();
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

			return this.reportHandler.GetReportDefinition(key);
		}

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		public List<ReportDefinition> GetReportDefintions()
		{
			return this.reportHandler.GetReportDefintions().ToList();
		}

		/// <summary>
		/// Gets detailed information about a given report parameter.
		/// </summary>
		/// <param name="id">The id of the report parameter for which to retrieve information.</param>
		/// <returns>Returns a report parameter manifest.</returns>
		public ParameterManifest GetReportParameterManifest(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportHandler.GetReportParameterManifest(key);
		}

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		public List<ReportParameter> GetReportParameters(string id)
		{
			var key = Guid.Empty;

			if (!Guid.TryParse(id, out key))
			{
				throw new ArgumentException($"The parameter { id } must be a valid { nameof(Guid) }");
			}

			return this.reportHandler.GetReportParameters(key).ToList();
		}

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		public AutoCompleteSourceDefinition GetReportParameterValues(string id, string parameterId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the report source.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve the source.</param>
		/// <returns>Returns the report source.</returns>
		public ReportDefinition GetReportSource(string id)
		{
			throw new NotImplementedException();
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
				throw new ArgumentException($"Unable to update device using id: {id}, and id: {parameterType.Key}");
			}

			return this.reportHandler.UpdateParameterType(parameterType);
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
				throw new ArgumentException($"Unable to update device using id: {id}, and id: {reportDefinition.Key}");
			}

			return this.reportHandler.UpdateReportDefinition(reportDefinition);
		}
	}
}