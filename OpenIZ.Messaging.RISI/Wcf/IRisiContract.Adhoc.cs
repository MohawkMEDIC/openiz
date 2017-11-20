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
 * Date: 2017-4-22
 */

using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model.RISI;
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
		DatamartDefinition CreateDatamart(DatamartDefinition definition);

		/// <summary>
		/// Create a stored query
		/// </summary>
		[WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/query")]
		DatamartStoredQuery CreateStoredQuery(String datamartId, DatamartStoredQuery queryDefinition);

		/// <summary>
		/// Create warehouse object
		/// </summary>
		[WebInvoke(Method = "POST", UriTemplate = "/datamart/{datamartId}/data")]
		DataWarehouseObject CreateWarehouseObject(String datamartId, DataWarehouseObject obj);

		/// <summary>
		/// Delete data mart
		/// </summary>
		[WebInvoke(Method = "DELETE", UriTemplate = "/datamart/{id}")]
		void DeleteDatamart(String id);

		/// <summary>
		/// Execute adhoc query
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/data")]
		RisiCollection<DataWarehouseObject> ExecuteAdhocQuery(String datamartId);

		/// <summary>
		/// Executes a stored query
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/query/{queryId}")]
		RisiCollection<DataWarehouseObject> ExecuteStoredQuery(String datamartId, String queryId);

		/// <summary>
		/// Gets a specified datamart
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{id}")]
		DatamartDefinition GetDatamart(String id);

		/// <summary>
		/// Gets a list of all datamarts from the warehouse
		/// </summary>
		[WebGet(UriTemplate = "/datamart")]
		RisiCollection<DatamartDefinition> GetDatamarts();

		/// <summary>
		/// Get stored queries
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/query")]
		RisiCollection<DatamartStoredQuery> GetStoredQueries(String datamartId);

		/// <summary>
		/// Get warehouse object
		/// </summary>
		[WebGet(UriTemplate = "/datamart/{datamartId}/data/{objectId}")]
		DataWarehouseObject GetWarehouseObject(String datamartId, String objectId);
	}
}