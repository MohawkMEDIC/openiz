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
 * Date: 2017-1-6
 */

using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a report persistence service.
	/// </summary>
	public class ReportPersistenceService : IDataPersistenceService<Report>
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Persistence.Reporting");

		/// <summary>
		/// Fired after a report is inserted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<Report>> Inserted;

		/// <summary>
		/// Fired while a report is being inserted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<Report>> Inserting;

		/// <summary>
		/// Fired after a report is obsoleted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<Report>> Obsoleted;

		/// <summary>
		/// Fired while a report is being obsoleted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<Report>> Obsoleting;

		/// <summary>
		/// Fired after a report is queried.
		/// </summary>
		public event EventHandler<PostQueryEventArgs<Report>> Queried;

		/// <summary>
		/// Fired while a report is being queried.
		/// </summary>
		public event EventHandler<PreQueryEventArgs<Report>> Querying;

		/// <summary>
		/// Fired after a report is been retrieved.
		/// </summary>
		public event EventHandler<PostRetrievalEventArgs<Report>> Retrieved;

		/// <summary>
		/// Fired while a report is being retrieved.
		/// </summary>
		public event EventHandler<PreRetrievalEventArgs<Report>> Retrieving;

		/// <summary>
		/// Fired after a report is updated.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<Report>> Updated;

		/// <summary>
		/// Fired while a report is being updated.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<Report>> Updating;

		/// <summary>
		/// Gets the count of a query.
		/// </summary>
		/// <param name="query">The query for which to determine the count.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns the count of the query.</returns>
		public int Count(Expression<Func<Report, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a report by id.
		/// </summary>
		/// <typeparam name="TIdentifier">The type of identifier.</typeparam>
		/// <param name="containerId">The id of the report.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="loadFast">Whether the result should load fast.</param>
		/// <returns>Returns the report or null if not found.</returns>
		public Report Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			Report report = null;

			using (var context = ApplicationDbContext.Create())
			{
				var result = context.ReportDefinitions.Find(containerId);

				if (result != null)
				{
					report = new Report
					{
						Key = result.Id,
						CreatedBy = new Core.Model.Security.SecurityUser
						{
							UserName = result.Author
						}
					};
				}
			}

			return report;
		}

		/// <summary>
		/// Inserts a report.
		/// </summary>
		/// <param name="storageData">The report to insert.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the inserted report.</returns>
		public Report Insert(Report storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obsoletes a report.
		/// </summary>
		/// <param name="storageData">The report to obsolete.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the obsoleted report.</returns>
		public Report Obsolete(Report storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for a report.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of reports.</returns>
		public IEnumerable<Report> Query(Expression<Func<Report, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for a report.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of reports.</returns>
		public IEnumerable<Report> Query(Expression<Func<Report, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates a report.
		/// </summary>
		/// <param name="storageData">The report to update.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the updated report.</returns>
		public Report Update(Report storageData, IPrincipal principal, TransactionMode mode)
		{
			throw new NotImplementedException();
		}
	}
}