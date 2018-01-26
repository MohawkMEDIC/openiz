/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model.RISI;
using SwaggerWcf.Attributes;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.RISI.Wcf
{
	/// <summary>
	/// RISI contract members for the data-warehouse
	/// </summary>
	[ServiceKnownType(typeof(RisiCollection<DatamartDefinition>))]
	[ServiceKnownType(typeof(RisiCollection<DatamartStoredQuery>))]
	[ServiceKnownType(typeof(RisiCollection<DataWarehouseObject>))]
	[ServiceKnownType(typeof(DatamartDefinition))]
	[ServiceKnownType(typeof(DatamartStoredQuery))]
	[ServiceKnownType(typeof(DataWarehouseObject))]
	public partial interface IRisiContract
	{
		/// <summary>
		/// Create a datamart
		/// </summary>
		[WebInvoke(Method = "POST", UriTemplate = "/datamart")]
        [SwaggerWcfPath("Create Ad-Hoc Datamart", "Creates an ad-hoc datamart on the RISI which can be used to store arbitrary values in the data warehouse server")]
		DatamartDefinition CreateDatamart(DatamartDefinition definition);

		/// <summary>
		/// Create a stored query
		/// </summary>
		[WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/query")]
        [SwaggerWcfPath("Create Stored Query", "Creates a stored query definition in the specified datamart. Store queries can be later executed by name by those with QueryDataWarehouse policy permission")]
		DatamartStoredQuery CreateStoredQuery(String datamartId, DatamartStoredQuery queryDefinition);

		/// <summary>
		/// Create warehouse object
		/// </summary>
		[WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/data")]
        [SwaggerWcfPath("Create Warehouse Data", "Inserts one or more tuples of warehouse data into the specified datamart within the data warehouse")]
		DataWarehouseObject CreateWarehouseObject(String datamartId, DataWarehouseObject obj);

		/// <summary>
		/// Delete data mart
		/// </summary>
		[WebInvoke(Method = "DELETE", UriTemplate = "/datamart/{id}")]
        [SwaggerWcfPath("Delete Ad-Hoc Datamart", "Deletes an ad-hoc datamart from the warehouse and removes all associated data and stored queries")]
		void DeleteDatamart(String id);

		/// <summary>
		/// Execute adhoc query
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/data")]
        [SwaggerWcfPath("Execute Ad-hoc Query", "Filters data from the specified datamart using filtering by the exposed properties of the datamart schema in the IMSI query format")]
		RisiCollection<DataWarehouseObject> ExecuteAdhocQuery(String datamartId);

		/// <summary>
		/// Executes a stored query
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/query/{queryId}")]
        [SwaggerWcfPath("Execute Stored Query", "Runs a previously registered stored query and returns the results. Filters can be applied using the IMSI query patterns")]
		RisiCollection<DataWarehouseObject> ExecuteStoredQuery(String datamartId, String queryId);

		/// <summary>
		/// Gets a specified datamart
		/// </summary>
        [SwaggerWcfPath("Get Ad-hoc Datamart", "Retrieves datamart metadata for the identified data mart")]
		[WebGet(UriTemplate = "/datamart/{id}")]
		DatamartDefinition GetDatamart(String id);

		/// <summary>
		/// Gets a list of all datamarts from the warehouse
		/// </summary>
		[WebGet(UriTemplate = "/datamart")]
        [SwaggerWcfPath("Get Ad-hoc Datamarts", "Retrieves a listing of all registered ad-hoc datamarts")]
		RisiCollection<DatamartDefinition> GetDatamarts();

		/// <summary>
		/// Get stored queries
		/// </summary>
        [SwaggerWcfPath("Get Stored Queries", "Gets all stored queries registered for a particular ad-hoc datamart")]
		[WebGet(UriTemplate = "/datamart/{datamartId}/query")]
		RisiCollection<DatamartStoredQuery> GetStoredQueries(String datamartId);

		/// <summary>
		/// Get warehouse object
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/data/{objectId}")]
        [SwaggerWcfPath("Retrieve Warehouse Data", "Retrieves a single identified tuple from the specified ad-hoc datamart")]
		DataWarehouseObject GetWarehouseObject(String datamartId, String objectId);
	}
}