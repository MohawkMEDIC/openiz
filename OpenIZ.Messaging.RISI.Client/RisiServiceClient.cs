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
 * Date: 2016-8-28
 */
using OpenIZ.Core.Interop.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenIZ.Core.Http;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.RISI;

namespace OpenIZ.Messaging.RISI.Client
{
	/// <summary>
	/// Represents a RISI service client for interfacing with the RISI engine.
	/// </summary>
	public class RisiServiceClient : ServiceClientBase, IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RisiServiceClient"/> class
		/// with a specified <see cref="IRestClient"/> instance.
		/// </summary>
		/// <param name="restClient">The REST client instance.</param>
		public RisiServiceClient(IRestClient restClient) : base(restClient)
		{
		}

		/// <summary>
		/// Creates a new report parameter type definition.
		/// </summary>
		/// <param name="parameterTypeDefinition">The report parameter type definition to create.</param>
		/// <returns>Returns the created report parameter type definition.</returns>
		public ParameterType CreateParameterType(ParameterType parameterTypeDefinition)
		{
			return this.Client.Post<ParameterType, ParameterType>("type", this.Client.Accept, parameterTypeDefinition);
		}

		/// <summary>
		/// Creates a new report definition.
		/// </summary>
		/// <param name="reportDefinition">The report definition to create.</param>
		/// <returns>Returns the created report definition.</returns>
		public ReportDefinition CreateReportDefinition(ReportDefinition reportDefinition)
		{
			return this.Client.Post<ReportDefinition, ReportDefinition>("report", this.Client.Accept, reportDefinition);
		}

		/// <summary>
		/// Creates a report format.
		/// </summary>
		/// <param name="reportFormat">The report format to create.</param>
		/// <returns>Returns the created report format.</returns>
		public ReportFormat CreateReportFormat(ReportFormat reportFormat)
		{
			return this.Client.Post<ReportFormat, ReportFormat>("format", this.Client.Accept, reportFormat);
		}

		/// <summary>
		/// Deletes a report parameter type.
		/// </summary>
		/// <param name="id">The id of the report parameter type to delete.</param>
		/// <returns>Returns the deleted report parameter type.</returns>
		public ParameterType DeleteParameterType(string id)
		{
			return this.Client.Delete<ParameterType>($"type/{id}");
		}

		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		public ReportDefinition DeleteReportDefinition(string id)
		{
			return this.Client.Delete<ReportDefinition>($"report/{id}");
		}

		/// <summary>
		/// Deletes a report format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <returns>Returns the report deleted report format.</returns>
		public ReportFormat DeleteReportFormat(string id)
		{
			return this.Client.Delete<ReportFormat>($"format/{id}");
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		/// <param name="disposing">Whether the current invocation is disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Client?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~RisiServiceClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		/// <summary>
		/// Dispose of any managed resources.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="parameters">The list of parameters of the report.</param>
		/// <returns>Returns the report in raw format.</returns>
		public byte[] ExecuteReport(string id, string format, RisiCollection<ReportParameter> parameters)
		{
			return this.Client.Post<RisiCollection<ReportParameter>, byte[]>($"report/{id}/{format}", this.Client.Accept, parameters);
		}

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		public RisiCollection<ReportParameter> GetAllReportParamterTypes()
		{
			return this.Client.Get<RisiCollection<ReportParameter>>("type", new KeyValuePair<string, object>("_", DateTimeOffset.Now));
		}

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		public ReportDefinition GetReportDefinition(string id)
		{
			return this.Client.Get<ReportDefinition>($"report/{id}");
		}

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		public RisiCollection<ReportDefinition> GetReportDefintions()
		{
			return this.Client.Get<RisiCollection<ReportDefinition>>("report", new KeyValuePair<string, object>("_", DateTimeOffset.UtcNow));
		}

		/// <summary>
		/// Gets a report format by id.
		/// </summary>
		/// <param name="id">The id of the report format to retrieve.</param>
		/// <returns>Returns a report format.</returns>
		public ReportFormat GetReportFormat(string id)
		{
			return this.Client.Get<ReportFormat>($"format/{id}", new KeyValuePair<string, object>("_", DateTimeOffset.UtcNow));
		}

		/// <summary>
		/// Gets detailed information about a given report parameter.
		/// </summary>
		/// <param name="id">The id of the report parameter for which to retrieve information.</param>
		/// <returns>Returns a report parameter manifest.</returns>
		public ParameterManifest GetReportParameterManifest(string id)
		{
			return this.Client.Get<ParameterManifest>($"type/{id}");
		}

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		public RisiCollection<ReportParameter> GetReportParameters(string id)
		{
			return this.Client.Get<RisiCollection<ReportParameter>>($"report/{id}/parm");
		}

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		public AutoCompleteSourceDefinition GetReportParameterValues(string id, string parameterId)
		{
			return this.Client.Get<AutoCompleteSourceDefinition>($"report/{id}/parm/{parameterId}/values");
		}

		/// <summary>
		/// Updates a parameter type definition.
		/// </summary>
		/// <param name="id">The id of the parameter type.</param>
		/// <param name="parameterTypeDefinition"></param>
		/// <returns>Returns the updated parameter type definition.</returns>
		public ParameterType UpdateParameterTypeDefinition(string id, ParameterType parameterTypeDefinition)
		{
			return this.Client.Put<ParameterType, ParameterType>($"type/{id}", this.Client.Accept, parameterTypeDefinition);
		}

		/// <summary>
		/// Updates a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to update.</param>
		/// <param name="reportDefinition">The updated report definition.</param>
		/// <returns>Returns the updated report definition.</returns>
		public ReportDefinition UpdateReportDefinition(string id, ReportDefinition reportDefinition)
		{
			return this.Client.Put<ReportDefinition, ReportDefinition>($"report/{id}", this.Client.Accept, reportDefinition);
		}

		/// <summary>
		/// Updates a report format.
		/// </summary>
		/// <param name="id">The id of the report format to update.</param>
		/// <param name="reportFormat">The updated report format.</param>
		/// <returns>Returns the update report format.</returns>
		public ReportFormat UpdateReportFormat(string id, ReportFormat reportFormat)
		{
			return this.Client.Put<ReportFormat, ReportFormat>($"format/{id}", this.Client.Accept, reportFormat);
		}
	}
}