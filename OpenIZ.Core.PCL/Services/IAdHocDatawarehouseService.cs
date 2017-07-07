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
 * Date: 2017-1-11
 */
using OpenIZ.Core.Data.Warehouse;
using System;
using System.Collections.Generic;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents a simple data warehousing service which allows business rules to stash
	/// pre-computed values.
	/// </summary>
	public interface IAdHocDatawarehouseService
	{
		/// <summary>
		/// Gets the provider mnemonic
		/// </summary>
		String DataProvider { get; }

		/// <summary>
		/// Creates an ad-hoc datamart which is not based on an ETL process, rather created
		/// by a trigger.
		/// </summary>
		DatamartDefinition CreateDatamart(String name, object schema);

		/// <summary>
		/// Gets a list of all registered adhoc data marts
		/// </summary>
		List<DatamartDefinition> GetDatamarts();

		/// <summary>
		/// Gets the specified datamart
		/// </summary>
		DatamartDefinition GetDatamart(String name);

        /// <summary>
        /// Gets the specified datamart
        /// </summary>
        DatamartDefinition GetDatamart(Guid id);

        /// <summary>
        /// Deletes a datamart
        /// </summary>
        void DeleteDatamart(Guid datamartId);

        /// <summary>
        /// Truncates (drops all data) the specified data mart
        /// </summary>
        void Truncate(Guid datamartId);

		/// <summary>
		/// Gets data from an ad-hoc data mart
		/// </summary>
		dynamic Get(Guid datamartId, Guid tupleId);

        /// <summary>
        /// Executes the specified query 
        /// </summary>
        IEnumerable<dynamic> AdhocQuery(String queryText);

		/// <summary>
		/// Perform an adhoc query on data
		/// </summary>
		IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic filterParameters);

		/// <summary>
		/// Performs an adhoc query with the specified query control
		/// </summary>
		IEnumerable<dynamic> AdhocQuery(Guid datamartId, dynamic filterParameters, int offset, int count, out int totalResults);

		/// <summary>
		/// Create the specified stored query on the warehouse provider
		/// </summary>
		DatamartStoredQuery CreateStoredQuery(Guid datamartId, object queryDefinition);

		/// <summary>
		/// Executes a predefined query against a datamart
		/// </summary>
		IEnumerable<dynamic> StoredQuery(Guid datamartId, String queryId, dynamic queryParameters);

		/// <summary>
		/// Adds the specified object to the specified datamart returning the tupleId
		/// </summary>
		Guid Add(Guid datamartId, dynamic obj);

		/// <summary>
		/// Delete a tuple from the datamart
		/// </summary>
		void Delete(Guid datamartId, dynamic matchingQuery);
	}
}