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
		/// Deletes a datamart
		/// </summary>
		void DeleteDatamart(Guid datamartId);

		/// <summary>
		/// Gets data from an ad-hoc data mart
		/// </summary>
		dynamic Get(Guid datamartId, Guid tupleId);

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
		void CreateStoredQuery(Guid datamartId, object queryDefinition);

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