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
 * Date: 2017-1-11
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a data type persistence service.
	/// </summary>
	public class ParameterTypePersistenceService : IDataPersistenceService<ParameterType>
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Persistence.Reporting");

		/// <summary>
		/// Fired after a data type is inserted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ParameterType>> Inserted;

		/// <summary>
		/// Fired while a data type is being inserted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ParameterType>> Inserting;

		/// <summary>
		/// Fired after a data type is obsoleted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ParameterType>> Obsoleted;

		/// <summary>
		/// Fired while a data type is being obsoleted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ParameterType>> Obsoleting;

		/// <summary>
		/// Fired after a data type is queried.
		/// </summary>
		public event EventHandler<PostQueryEventArgs<ParameterType>> Queried;

		/// <summary>
		/// Fired while a data type is being queried.
		/// </summary>
		public event EventHandler<PreQueryEventArgs<ParameterType>> Querying;

		/// <summary>
		/// Fired after a data type is been retrieved.
		/// </summary>
		public event EventHandler<PostRetrievalEventArgs<ParameterType>> Retrieved;

		/// <summary>
		/// Fired while a data type is being retrieved.
		/// </summary>
		public event EventHandler<PreRetrievalEventArgs<ParameterType>> Retrieving;

		/// <summary>
		/// Fired after a data type is updated.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ParameterType>> Updated;

		/// <summary>
		/// Fired while a data type is being updated.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ParameterType>> Updating;

		/// <summary>
		/// Gets the count of a query.
		/// </summary>
		/// <param name="query">The query for which to determine the count.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns the count of the query.</returns>
		public int Count(Expression<Func<ParameterType, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a report by id.
		/// </summary>
		/// <typeparam name="TIdentifier">The type of identifier.</typeparam>
		/// <param name="containerId">The id of the ReportDefinition.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="loadFast">Whether the result should load fast.</param>
		/// <returns>Returns the report or null if not found.</returns>
		public ParameterType Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts a report.
		/// </summary>
		/// <param name="storageData">The data type to insert.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the inserted data type.</returns>
		public ParameterType Insert(ParameterType storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes a data type.
		/// </summary>
		/// <param name="storageData">The data type to obsolete.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the obsoleted report.</returns>
		public ParameterType Obsolete(ParameterType storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for a data type.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of data types.</returns>
		public IEnumerable<ParameterType> Query(Expression<Func<ParameterType, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for a data type.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of data types.</returns>
		public IEnumerable<ParameterType> Query(Expression<Func<ParameterType, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates a data type.
		/// </summary>
		/// <param name="storageData">The data type to update.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the updated data type.</returns>
		public ParameterType Update(ParameterType storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}
	}
}
