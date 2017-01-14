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

using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Persistence.Reporting.Context;
using OpenIZ.Persistence.Reporting.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a report persistence service.
	/// </summary>
	internal class ReportParameterPersistenceService : ReportPersistenceServiceBase<ReportParameter, Model.ReportParameter>, IDataPersistenceService<ReportParameter>
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Persistence.Reporting");

		/// <summary>
		/// Fired after a report is inserted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportParameter>> Inserted;

		/// <summary>
		/// Fired while a report is being inserted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportParameter>> Inserting;

		/// <summary>
		/// Fired after a report is obsoleted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportParameter>> Obsoleted;

		/// <summary>
		/// Fired while a report is being obsoleted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportParameter>> Obsoleting;

		/// <summary>
		/// Fired after a report is queried.
		/// </summary>
		public event EventHandler<PostQueryEventArgs<ReportParameter>> Queried;

		/// <summary>
		/// Fired while a report is being queried.
		/// </summary>
		public event EventHandler<PreQueryEventArgs<ReportParameter>> Querying;

		/// <summary>
		/// Fired after a report is been retrieved.
		/// </summary>
		public event EventHandler<PostRetrievalEventArgs<ReportParameter>> Retrieved;

		/// <summary>
		/// Fired while a report is being retrieved.
		/// </summary>
		public event EventHandler<PreRetrievalEventArgs<ReportParameter>> Retrieving;

		/// <summary>
		/// Fired after a report is updated.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportParameter>> Updated;

		/// <summary>
		/// Fired while a report is being updated.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportParameter>> Updating;

		/// <summary>
		/// Gets the count of a query.
		/// </summary>
		/// <param name="query">The query for which to determine the count.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns the count of the query.</returns>
		public int Count(Expression<Func<ReportParameter, bool>> query, IPrincipal authContext)
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
		public ReportParameter Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			ReportParameter result = null;

			using (var context = new ApplicationDbContext())
			{
				var parameterType = context.ReportParameters.Find(containerId.Id);

				result = this.ToModelInstance(parameterType);

				this.Retrieving?.Invoke(this, new PreRetrievalEventArgs<ReportParameter>(result, principal));
			}

			this.Retrieved?.Invoke(this, new PostRetrievalEventArgs<ReportParameter>(result, principal));

			return result;
		}

		/// <summary>
		/// Inserts a report.
		/// </summary>
		/// <param name="storageData">The report to insert.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the inserted report.</returns>
		public ReportParameter Insert(ReportParameter storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportParameter result = null;

			this.Inserting?.Invoke(this, new PrePersistenceEventArgs<ReportParameter>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = this.FromModelInstance(storageData);

				context.ReportParameters.Add(entity);
				context.SaveChanges();

				result = this.ToModelInstance(entity);
			}

			this.Inserted?.Invoke(this, new PostPersistenceEventArgs<ReportParameter>(result, principal));

			return result;
		}

		/// <summary>
		/// Loads the relations for a given domain instance.
		/// </summary>
		/// <param name="context">The application database context.</param>
		/// <param name="domainInstance">The domain instance for which the load the relations.</param>
		/// <returns>Returns the updated domain instance.</returns>
		protected override Model.ReportParameter LoadRelations(ApplicationDbContext context, Model.ReportParameter domainInstance)
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
		public ReportParameter Obsolete(ReportParameter storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportParameter result = null;

			this.Obsoleting?.Invoke(this, new PrePersistenceEventArgs<ReportParameter>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = context.ReportParameters.Find(storageData.Key);

				if (entity == null)
				{
					throw new EntityNotFoundException();
				}

				result = this.ToModelInstance(context.ReportParameters.Remove(entity));
			}

			this.Obsoleted?.Invoke(this, new PostPersistenceEventArgs<ReportParameter>(result, principal));

			return result;
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
		public IEnumerable<ReportParameter> Query(Expression<Func<ReportParameter, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
		{
			IEnumerable<ReportParameter> results = new List<ReportParameter>();

			this.Querying?.Invoke(this, new PreQueryEventArgs<ReportParameter>(query, authContext));

			using (var context = new ApplicationDbContext())
			{
				var expression = ModelMapper.MapModelExpression<ReportParameter, Model.ReportParameter>(query);

				results = context.ReportParameters.Where(expression).ToList().Select(this.ToModelInstance);
			}

			totalCount = results.Count();

			if (offset > 0)
			{
				results = results.Skip(offset);
			}

			if (count != null)
			{
				results = results.Take(count.Value);
			}

			this.Queried?.Invoke(this, new PostQueryEventArgs<ReportParameter>(query, results.AsQueryable(), authContext));

			return results;
		}

		/// <summary>
		/// Queries for a report.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of reports.</returns>
		public IEnumerable<ReportParameter> Query(Expression<Func<ReportParameter, bool>> query, IPrincipal authContext)
		{
			var totalCount = 0;
			return this.Query(query, 0, null, authContext, out totalCount);
		}

		/// <summary>
		/// Updates a report.
		/// </summary>
		/// <param name="storageData">The report to update.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the updated report.</returns>
		public ReportParameter Update(ReportParameter storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportParameter result = null;

			this.Updating?.Invoke(this, new PrePersistenceEventArgs<ReportParameter>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var domainInstance = this.FromModelInstance(storageData);

				context.Entry(domainInstance).State = EntityState.Modified;

				context.SaveChanges();

				result = this.ToModelInstance(domainInstance);
			}

			this.Updated?.Invoke(this, new PostPersistenceEventArgs<ReportParameter>(result, principal));

			return result;
		}

		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override Model.ReportParameter FromModelInstance(ReportParameter modelInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(Model.ReportParameter) } to { nameof(ReportParameter) }");
			return modelInstance == null ? null : ModelMapper.MapModelInstance<ReportParameter, Model.ReportParameter>(modelInstance);
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override ReportParameter ToModelInstance(Model.ReportParameter domainInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportParameter) } to { nameof(Model.ReportParameter) }");
			return domainInstance == null ? null : ModelMapper.MapDomainInstance<Model.ReportParameter, ReportParameter>(domainInstance);
		}
	}
}