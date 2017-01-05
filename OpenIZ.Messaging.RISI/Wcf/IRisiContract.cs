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

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using OpenIZ.Core.Model.RISI;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// Provides operations for running and managing reports.
	/// </summary>
	[ServiceContract(Namespace = "http://openiz.org/risi/1.0", Name = "RISI", ConfigurationName = "RISI_1.0")]
	public interface IRisiContract
	{
		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		[WebInvoke(UriTemplate = "/report/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
		ReportDefinition DeleteReportDefinition(string id);

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		[WebGet(UriTemplate = "/report/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
		ReportDefinition GetReportDefinition(string id);

		[WebGet(UriTemplate = "/report/{id}/parm", BodyStyle = WebMessageBodyStyle.Bare)]
		List<ParameterDefinition> GetReportParameters(string id);

		[WebGet(UriTemplate = "/report/{id}/source", BodyStyle = WebMessageBodyStyle.Bare)]
		ReportDefinition GetReportSource(string id);
	}
}