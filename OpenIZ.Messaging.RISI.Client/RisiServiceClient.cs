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

using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Http;
using OpenIZ.Core.Interop.Clients;
using OpenIZ.Core.Model.RISI;
using System;
using System.IO;

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
		/// Creates the datamart.
		/// </summary>
		/// <param name="definition">The definition.</param>
		/// <returns>Returns the created datamart.</returns>
		public DatamartDefinition CreateDatamart(DatamartDefinition definition)
		{
			return this.Client.Post<DatamartDefinition, DatamartDefinition>("datamart", this.Client.Accept, definition);
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
		/// Creates the stored query.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="queryDefinition">The query definition.</param>
		/// <returns>Returns the created stored query.</returns>
		public DatamartStoredQuery CreateStoredQuery(Guid id, DatamartStoredQuery queryDefinition)
		{
			return this.Client.Post<DatamartStoredQuery, DatamartStoredQuery>($"datamart/{id}/query", this.Client.Accept, queryDefinition);
		}

		/// <summary>
		/// Creates the warehouse object.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="dataWarehouseObject">The data warehouse object.</param>
		/// <returns>Returns the created warehouse object.</returns>
		public DataWarehouseObject CreateWarehouseObject(Guid id, DataWarehouseObject dataWarehouseObject)
		{
			return this.Client.Post<DataWarehouseObject, DataWarehouseObject>($"/datamart/{id}/data", this.Client.Accept, dataWarehouseObject);
		}

		/// <summary>
		/// Deletes the datamart.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public void DeleteDatamart(Guid id)
		{
			this.Client.Delete<DatamartDefinition>($"datamart/{id}");
		}

		/// <summary>
		/// Deletes a report parameter type.
		/// </summary>
		/// <param name="id">The id of the report parameter type to delete.</param>
		/// <returns>Returns the deleted report parameter type.</returns>
		public ParameterType DeleteParameterType(Guid id)
		{
			return this.Client.Delete<ParameterType>($"type/{id}");
		}

		/// <summary>
		/// Deletes a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to delete.</param>
		/// <returns>Returns the deleted report definition.</returns>
		public ReportDefinition DeleteReportDefinition(Guid id)
		{
			return this.Client.Delete<ReportDefinition>($"report/{id}");
		}

		/// <summary>
		/// Deletes a report format.
		/// </summary>
		/// <param name="id">The id of the report format.</param>
		/// <returns>Returns the report deleted report format.</returns>
		public ReportFormat DeleteReportFormat(Guid id)
		{
			return this.Client.Delete<ReportFormat>($"format/{id}");
		}

		/// <summary>
		/// Executes the adhoc query.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the executed datawarehouse object.</returns>
		public RisiCollection<DataWarehouseObject> ExecuteAdhocQuery(Guid id)
		{
			return this.Client.Get<RisiCollection<DataWarehouseObject>>($"datamart/{id}/data");
		}

		/// <summary>
		/// Executes the stored query.
		/// </summary>
		/// <param name="datamartId">The datamart identifier.</param>
		/// <param name="queryId">The query identifier.</param>
		/// <returns>Returns the list of object which were executed as a prt of the stored query.</returns>
		public RisiCollection<DataWarehouseObject> ExecuteStoredQuery(Guid datamartId, Guid queryId)
		{
			return this.Client.Get<RisiCollection<DataWarehouseObject>>($"datamart/{datamartId}/query/{queryId}");
		}

		/// <summary>
		/// Gets a list of all report parameter types.
		/// </summary>
		/// <returns>Returns a list of report parameter types.</returns>
		public RisiCollection<ParameterType> GetAllReportParameterTypes()
		{
			return this.Client.Get<RisiCollection<ParameterType>>("type", null);
		}

		/// <summary>
		/// Gets the datamart.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the datamar definition.</returns>
		public DatamartDefinition GetDatamart(Guid id)
		{
			return this.Client.Get<DatamartDefinition>($"datamart/{id}");
		}

		/// <summary>
		/// Gets the datamarts.
		/// </summary>
		/// <returns>Returns a list of datamarts.</returns>
		public RisiCollection<DatamartDefinition> GetDatamarts()
		{
			return this.Client.Get<RisiCollection<DatamartDefinition>>("datamart");
		}

		/// <summary>
		/// Gets a report definition by id.
		/// </summary>
		/// <param name="id">The id of the report definition to retrieve.</param>
		/// <returns>Returns a report definition.</returns>
		public ReportDefinition GetReportDefinition(Guid id)
		{
			return this.Client.Get<ReportDefinition>($"report/{id}");
		}

		/// <summary>
		/// Gets a list of report definitions based on a specific query.
		/// </summary>
		/// <returns>Returns a list of report definitions.</returns>
		public RisiCollection<ReportDefinition> GetReportDefinitions()
		{
			return this.Client.Get<RisiCollection<ReportDefinition>>("report", null);
		}

		/// <summary>
		/// Gets a report format by id.
		/// </summary>
		/// <param name="id">The id of the report format to retrieve.</param>
		/// <returns>Returns a report format.</returns>
		public ReportFormat GetReportFormat(Guid id)
		{
			return this.Client.Get<ReportFormat>($"format/{id}", null);
		}

		/// <summary>
		/// Gets the report formats.
		/// </summary>
		/// <returns>Returns a list of report formats.</returns>
		public RisiCollection<ReportFormat> GetReportFormats()
		{
			return this.Client.Get<RisiCollection<ReportFormat>>("format");
		}

		/// <summary>
		/// Gets a report parameter by id.
		/// </summary>
		/// <param name="id">The id of the report parameter to retrieve.</param>
		/// <returns>Returns a report parameter.</returns>
		public ReportParameter GetReportParameter(Guid id)
		{
			return this.Client.Get<ReportParameter>($"type/{id}");
		}

		/// <summary>
		/// Gets a list of report parameters.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve parameters.</param>
		/// <returns>Returns a list of parameters.</returns>
		public RisiCollection<ReportParameter> GetReportParameters(Guid id)
		{
			return this.Client.Get<RisiCollection<ReportParameter>>($"report/{id}/parm");
		}

		/// <summary>
		/// Gets a list of auto-complete parameters which are applicable for the specified parameter.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="parameterId">The id of the parameter for which to retrieve detailed information.</param>
		/// <returns>Returns an auto complete source definition of valid parameters values for a given parameter.</returns>
		public AutoCompleteSourceDefinition GetReportParameterValues(Guid id, Guid parameterId)
		{
			return this.Client.Get<AutoCompleteSourceDefinition>($"report/{id}/parm/{parameterId}/values");
		}

		/// <summary>
		/// Gets the report source.
		/// </summary>
		/// <param name="id">The id of the report for which to retrieve the source.</param>
		/// <returns>Returns the report source.</returns>
		public Stream GetReportSource(Guid id)
		{
			return new MemoryStream(this.Client.Get($"report/{id}/source"));
		}

		/// <summary>
		/// Gets the stored queries.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the storied queries on the datamart.</returns>
		public RisiCollection<DatamartStoredQuery> GetStoredQueries(Guid id)
		{
			return this.Client.Get<RisiCollection<DatamartStoredQuery>>($"datamart/{id}/query");
		}

		/// <summary>
		/// Gets the warehouse object.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="objectId">The object identifier.</param>
		/// <returns>Returns the warehouse object.</returns>
		public DataWarehouseObject GetWarehouseObject(Guid id, Guid objectId)
		{
			return this.Client.Get<DataWarehouseObject>($"datamart/{id}/data/{objectId}");
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

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

		#endregion IDisposable Support

		/// <summary>
		/// Executes a report.
		/// </summary>
		/// <param name="id">The id of the report.</param>
		/// <param name="format">The output format of the report.</param>
		/// <param name="bundle">The list of parameters of the report.</param>
		/// <returns>Returns the report in raw format.</returns>
		public Stream RunReport(Guid id, string format, ReportBundle bundle)
		{
			return new MemoryStream(this.Client.Post<ReportBundle, byte[]>($"report/{id}/format/{format}", this.Client.Accept, bundle));
		}

		/// <summary>
		/// Updates a parameter type definition.
		/// </summary>
		/// <param name="id">The id of the parameter type.</param>
		/// <param name="parameterTypeDefinition"></param>
		/// <returns>Returns the updated parameter type definition.</returns>
		public ParameterType UpdateParameterTypeDefinition(Guid id, ParameterType parameterTypeDefinition)
		{
			return this.Client.Put<ParameterType, ParameterType>($"type/{id}", this.Client.Accept, parameterTypeDefinition);
		}

		/// <summary>
		/// Updates a report definition.
		/// </summary>
		/// <param name="id">The id of the report definition to update.</param>
		/// <param name="reportDefinition">The updated report definition.</param>
		/// <returns>Returns the updated report definition.</returns>
		public ReportDefinition UpdateReportDefinition(Guid id, ReportDefinition reportDefinition)
		{
			return this.Client.Put<ReportDefinition, ReportDefinition>($"report/{id}", this.Client.Accept, reportDefinition);
		}

		/// <summary>
		/// Updates a report format.
		/// </summary>
		/// <param name="id">The id of the report format to update.</param>
		/// <param name="reportFormat">The updated report format.</param>
		/// <returns>Returns the update report format.</returns>
		public ReportFormat UpdateReportFormat(Guid id, ReportFormat reportFormat)
		{
			return this.Client.Put<ReportFormat, ReportFormat>($"format/{id}", this.Client.Accept, reportFormat);
		}
	}
}