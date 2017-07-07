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

using OpenIZ.Core.Model.RISI;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceKnownType(typeof(ReportBundle))]
	[ServiceKnownType(typeof(ReportFormat))]
	[ServiceKnownType(typeof(ParameterType))]
	[ServiceKnownType(typeof(ReportDefinition))]
	[ServiceKnownType(typeof(ReportParameter))]
	[ServiceKnownType(typeof(AutoCompleteSourceDefinition))]
	[ServiceKnownType(typeof(RisiCollection<ReportFormat>))]
	[ServiceKnownType(typeof(RisiCollection<ReportParameter>))]
	[ServiceKnownType(typeof(RisiCollection<ReportDefinition>))]
	[ServiceKnownType(typeof(ListAutoCompleteSourceDefinition))]
	[ServiceKnownType(typeof(QueryAutoCompleteSourceDefinition))]
	[ServiceContract(Namespace = "http://openiz.org/risi/1.0", Name = "RISI", ConfigurationName = "RISI_1.0")]
	public partial interface IRisiContract
	{
		/// <summary>
		/// Creates a new parameter type.
		/// </summary>
		/// <param name="parameterType">The parameter type to create.</param>
		/// <returns>Returns the created parameter type.</returns>
		[WebInvoke(UriTemplate = "/type", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		ParameterType CreateParameterType(ParameterType parameterType);

		/// <summary>
		/// Creates a new report definition.
		/// </summary>
		/// <param name="reportDefinition">The report definition to create.</param>
		/// <returns>Returns the created report definition.</returns>
		[WebInvoke(UriTemplate = "/report", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		ReportDefinition CreateReportDefinition(ReportDefinition reportDefinition);

		/// <summary>
		/// Creates a report format.
		/// </summary>
		/// <param name="reportFormat">The report format to create.</param>
		/// <returns>Returns the created report format.</returns>
		[WebInvoke(UriTemplate = "/format", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		ReportFormat CreateReportFormat(ReportFormat reportFormat);

		/// <summary>
		/// Deletes a report parameter type.
		/// </summary>
		/// <param name="id">The id of the report parameter type to delete.</param>
		/// <returns>Returns the deleted report parameter type.</returns>
		[WebInvoke(UriTemplate = "/type/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		ParameterType DeleteParameterType(string id);

		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		[WebInvoke(UriTemplate = "/report/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		ReportDefinition DeleteReportDefinition(string id);

		/// <summary>
		/// Deletes a report format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <returns>Returns the report deleted report format.</returns>
		[WebInvoke(UriTemplate = "/format/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		ReportFormat DeleteReportFormat(string id);

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		[WebGet(UriTemplate = "/type", BodyStyle = WebMessageBodyStyle.Bare)]
		RisiCollection<ParameterType> GetAllReportParameterTypes();

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		[WebGet(UriTemplate = "/report/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		ReportDefinition GetReportDefinition(string id);

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		[WebGet(UriTemplate = "/report", BodyStyle = WebMessageBodyStyle.Bare)]
		RisiCollection<ReportDefinition> GetReportDefinitions();

		/// <summary>
		/// Gets a report format by id.
		/// </summary>
		/// <param name="id">The id of the report format to retrieve.</param>
		/// <returns>Returns a report format.</returns>
		[WebGet(UriTemplate = "/format/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		ReportFormat GetReportFormat(string id);

		/// <summary>
		/// Gets the report formats.
		/// </summary>
		/// <returns>Returns a list of report formats.</returns>
		[WebGet(UriTemplate = "/format", BodyStyle = WebMessageBodyStyle.Bare)]
		RisiCollection<ReportFormat> GetReportFormats();

		/// <summary>
		/// Gets a report parameter by id.
		/// </summary>
		/// <param name="id">The id of the report parameter to retrieve.</param>
		/// <returns>Returns a report parameter.</returns>
		[WebGet(UriTemplate = "/type/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		ReportParameter GetReportParameter(string id);

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		[WebGet(UriTemplate = "/report/{id}/parm", BodyStyle = WebMessageBodyStyle.Bare)]
		RisiCollection<ReportParameter> GetReportParameters(string id);

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		[WebGet(UriTemplate = "/report/{id}/parm/{parameterId}/values")]
		AutoCompleteSourceDefinition GetReportParameterValues(string id, string parameterId);

		/// <summary>
		/// Gets the report source.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve the source.</param>
		/// <returns>Returns the report source.</returns>
		[WebGet(UriTemplate = "/report/{id}/source", BodyStyle = WebMessageBodyStyle.Bare)]
		Stream GetReportSource(string id);

		/// <summary>
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="bundle">The report parameters.</param>
		/// <returns>Returns the report in raw format.</returns>
		[WebInvoke(UriTemplate = "/report/{id}/format/{format}", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		Stream RunReport(string id, string format, ReportBundle bundle);

		/// <summary>
		/// Updates a parameter type definition.
		/// </summary>
		/// <param name="id">The id of the parameter type.</param>
		/// <param name="parameterType">The parameter type to update.</param>
		/// <returns>Returns the updated parameter type definition.</returns>
		[WebInvoke(UriTemplate = "/type/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		ParameterType UpdateParameterType(string id, ParameterType parameterType);

		/// <summary>
		/// Updates a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to update.</param>
		/// <param name="reportDefinition">The updated report definition.</param>
		/// <returns>Returns the updated report definition.</returns>
		[WebInvoke(UriTemplate = "/report/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		ReportDefinition UpdateReportDefinition(string id, ReportDefinition reportDefinition);

		/// <summary>
		/// Updates a report format.
		/// </summary>
		/// <param name="id">The id of the report format to update.</param>
		/// <param name="reportFormat">The updated report format.</param>
		/// <returns>Returns the update report format.</returns>
		[WebInvoke(UriTemplate = "/format/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT")]
		ReportFormat UpdateReportFormat(string id, ReportFormat reportFormat);
	}
}