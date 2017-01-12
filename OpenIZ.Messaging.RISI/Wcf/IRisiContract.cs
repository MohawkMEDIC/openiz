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

using OpenIZ.Core.Model.RISI;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceKnownType(typeof(ReportDefinition))]
	[ServiceKnownType(typeof(ParameterType))]
	[ServiceKnownType(typeof(ParameterDefinition))]
	[ServiceKnownType(typeof(AutoCompleteSourceDefinition))]
	[ServiceKnownType(typeof(ListAutoCompleteSourceDefinition))]
	[ServiceKnownType(typeof(QueryAutoCompleteSourceDefinition))]
	[ServiceContract(Namespace = "http://openiz.org/risi/1.0", Name = "RISI", ConfigurationName = "RISI_1.0")]
	public interface IRisiContract
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
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="parameters">The list of parameters of the report.</param>
		/// <returns>Returns the report in raw format.</returns>
		[WebInvoke(UriTemplate = "/report/{id}/{format}", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST")]
		byte[] ExecuteReport(string id, string format, List<ReportParameter> parameters);

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		[WebGet(UriTemplate = "/type", BodyStyle = WebMessageBodyStyle.Bare)]
		List<ReportParameter> GetAllReportParamterTypes();

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
		List<ReportDefinition> GetReportDefintions();

		/// <summary>
		/// Gets detailed information about a given report parameter.
		/// </summary>
		/// <param name="id">The id of the report parameter for which to retrieve information.</param>
		/// <returns>Returns a report parameter manifest.</returns>
		[WebGet(UriTemplate = "/type/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		ParameterManifest GetReportParameterManifest(string id);

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		[WebGet(UriTemplate = "/report/{id}/parm", BodyStyle = WebMessageBodyStyle.Bare)]
		List<ReportParameter> GetReportParameters(string id);

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
		ReportDefinition GetReportSource(string id);

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
	}
}