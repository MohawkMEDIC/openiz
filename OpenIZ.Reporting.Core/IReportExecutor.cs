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
 * User: justi
 * Date: 2017-4-7
 */
using OpenIZ.Core.Model.RISI;
using OpenIZ.Reporting.Core.Configuration;
using System;
using System.Collections.Generic;

namespace OpenIZ.Reporting.Core
{
	/// <summary>
	/// Represents a report handler for a reporting engine.
	/// </summary>
	public interface IReportExecutor : IDisposable
	{
		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		ReportingConfiguration Configuration { get; }

		/// <summary>
		/// Gets or sets the report URI.
		/// </summary>
		Uri ReportUri { get; }

		/// <summary>
		/// Creates a new report parameter type.
		/// </summary>
		/// <param name="parameterType">The parameter type to create.</param>
		/// <returns>Returns the created parameter type.</returns>
		ParameterType CreateParameterType(ParameterType parameterType);

		/// <summary>
		/// Creates a new report definition.
		/// </summary>
		/// <param name="reportDefinition">The report definition to create.</param>
		/// <returns>Returns the created report definition.</returns>
		ReportDefinition CreateReportDefinition(ReportDefinition reportDefinition);

		/// <summary>
		/// Creates a report format.
		/// </summary>
		/// <param name="reportFormat">The report format to create.</param>
		/// <returns>Returns the created report format.</returns>
		ReportFormat CreateReportFormat(ReportFormat reportFormat);

		/// <summary>
		/// Deletes a parameter type.
		/// </summary>
		/// <param name="id">The id of the parameter type to delete.</param>
		/// <returns>Returns the deleted parameter type.</returns>
		ParameterType DeleteParameterType(Guid id);

		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		ReportDefinition DeleteReportDefinition(Guid id);

		/// <summary>
		/// Deletes a report format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <returns>Returns the report deleted report format.</returns>
		ReportFormat DeleteReportFormat(Guid id);

		/// <summary>
		/// Converts a <see cref="byte" /> array instance to an <see cref="object" /> instance.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the converted object instance.</returns>
		object FromByteArray(byte[] data);

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		RisiCollection<ParameterType> GetAllReportParameterTypes();

		/// <summary>
		/// Gets a parameter type by id.
		/// </summary>
		/// <param name="id">The id of the parameter type to retrieve.</param>
		/// <returns>Returns a parameter type.</returns>
		ParameterType GetParameterType(Guid id);

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		ReportDefinition GetReportDefinition(Guid id);

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		RisiCollection<ReportDefinition> GetReportDefinitions();

		/// <summary>
		/// Gets a report format by id.
		/// </summary>
		/// <param name="id">The id of the report format to retrieve.</param>
		/// <returns>Returns a report format.</returns>
		ReportFormat GetReportFormat(Guid id);

		/// <summary>
		/// Gets the report formats.
		/// </summary>
		/// <returns>Returns a list of report formats.</returns>
		RisiCollection<ReportFormat> GetReportFormats();

		/// <summary>
		/// Gets a report parameter by id.
		/// </summary>
		/// <param name="id">The id of the report parameter to retrieve.</param>
		/// <returns>Returns a report parameter.</returns>
		ReportParameter GetReportParameter(Guid id);

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		RisiCollection<ReportParameter> GetReportParameters(Guid id);

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		AutoCompleteSourceDefinition GetReportParameterValues(Guid id, Guid parameterId);

		/// <summary>
		/// Gets the report source.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve the source.</param>
		/// <returns>Returns the report source.</returns>
		byte[] GetReportSource(Guid id);

		/// <summary>
		/// Runs a report.
		/// </summary>
		/// <param name="reportId">The id of the report.</param>
		/// <param name="reportFormatId">The format of the report.</param>
		/// <param name="parameters">The parameters of the report.</param>
		/// <returns>Returns the raw report.</returns>
		byte[] RunReport(Guid reportId, Guid reportFormatId, IEnumerable<ReportParameter> parameters);

		/// <summary>
		/// Converts an object to a byte array.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the converted byte array.</returns>
		byte[] ToByteArray(object data);

		/// <summary>
		/// Updates a parameter type.
		/// </summary>
		/// <param name="parameterType">The updated parameter type.</param>
		/// <returns>Returns the updated parameter type.</returns>
		ParameterType UpdateParameterType(ParameterType parameterType);

		/// <summary>
		/// Updates a report definition.
		/// </summary>
		/// <param name="reportDefinition">The updated report definition.</param>
		/// <returns>Returns the updated report definition.</returns>
		ReportDefinition UpdateReportDefinition(ReportDefinition reportDefinition);

		/// <summary>
		/// Updates a report format.
		/// </summary>
		/// <param name="reportFormat">The updated report format.</param>
		/// <returns>Returns the update report format.</returns>
		ReportFormat UpdateReportFormat(ReportFormat reportFormat);
	}
}