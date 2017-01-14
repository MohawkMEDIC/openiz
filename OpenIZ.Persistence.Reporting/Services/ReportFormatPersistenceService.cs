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
 * Date: 2017-1-13
 */
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Reporting.Context;
using OpenIZ.Persistence.Reporting.Exceptions;
using OpenIZ.Core.Model.RISI;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a report format persistence service.
	/// </summary>
	internal class ReportFormatPersistenceService : ReportPersistenceServiceBase<ReportFormat, Model.ReportFormat>, IDataPersistenceService<ReportFormat>
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Persistence.Reporting");

		/// <summary>
		/// Fired after a report format is inserted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportFormat>> Inserted;

		/// <summary>
		/// Fired while a report format is being inserted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportFormat>> Inserting;

		/// <summary>
		/// Fired after a report format is obsoleted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportFormat>> Obsoleted;

		/// <summary>
		/// Fired while a report format is being obsoleted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportFormat>> Obsoleting;

		/// <summary>
		/// Fired after a report format is queried.
		/// </summary>
		public event EventHandler<PostQueryEventArgs<ReportFormat>> Queried;

		/// <summary>
		/// Fired while a report format is being queried.
		/// </summary>
		public event EventHandler<PreQueryEventArgs<ReportFormat>> Querying;

		/// <summary>
		/// Fired after a report format is been retrieved.
		/// </summary>
		public event EventHandler<PostRetrievalEventArgs<ReportFormat>> Retrieved;

		/// <summary>
		/// Fired while a report format is being retrieved.
		/// </summary>
		public event EventHandler<PreRetrievalEventArgs<ReportFormat>> Retrieving;

		/// <summary>
		/// Fired after a report format is updated.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportFormat>> Updated;

		/// <summary>
		/// Fired while a report format is being updated.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportFormat>> Updating;

		/// <summary>
		/// Gets the count of a query.
		/// </summary>
		/// <param name="query">The query for which to determine the count.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns the count of the query.</returns>
		public int Count(Expression<Func<ReportFormat, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override Model.ReportFormat FromModelInstance(ReportFormat modelInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(Model.ReportFormat) } to { nameof(ReportFormat) }");
			return modelInstance == null ? null : modelMapper.MapModelInstance<ReportFormat, Model.ReportFormat>(modelInstance);
		}

		/// <summary>
		/// Gets a report by id.
		/// </summary>
		/// <typeparam name="TIdentifier">The type of identifier.</typeparam>
		/// <param name="containerId">The id of the ReportDefinition.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="loadFast">Whether the result should load fast.</param>
		/// <returns>Returns the report or null if not found.</returns>
		public ReportFormat Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			ReportFormat result = null;

			using (var context = new ApplicationDbContext())
			{
				var reportDefinition = context.ReportFormats.Find(containerId.Id);

				result = this.ToModelInstance(reportDefinition);

				this.Retrieving?.Invoke(this, new PreRetrievalEventArgs<ReportFormat>(result, principal));
			}

			this.Retrieved?.Invoke(this, new PostRetrievalEventArgs<ReportFormat>(result, principal));

			return result;
		}

		/// <summary>
		/// Inserts a report.
		/// </summary>
		/// <param name="storageData">The report to insert.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the inserted report.</returns>
		public ReportFormat Insert(ReportFormat storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportFormat result = null;

			this.Inserting?.Invoke(this, new PrePersistenceEventArgs<ReportFormat>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = this.FromModelInstance(storageData);

				context.ReportFormats.Add(entity);
				context.SaveChanges();

				result = this.ToModelInstance(entity);
			}

			this.Inserted?.Invoke(this, new PostPersistenceEventArgs<ReportFormat>(result, principal));

			return result;
		}

		/// <summary>
		/// Obsoletes a report.
		/// </summary>
		/// <param name="storageData">The report to obsolete.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the obsoleted report.</returns>
		public ReportFormat Obsolete(ReportFormat storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportFormat result = null;

			this.Obsoleting?.Invoke(this, new PrePersistenceEventArgs<ReportFormat>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = context.ReportFormats.Find(storageData.Key);

				if (entity == null)
				{
					throw new EntityNotFoundException();
				}

				result = this.ToModelInstance(context.ReportFormats.Remove(entity));
			}

			this.Obsoleted?.Invoke(this, new PostPersistenceEventArgs<ReportFormat>(result, principal));

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
		public IEnumerable<ReportFormat> Query(Expression<Func<ReportFormat, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
		{
			IEnumerable<ReportFormat> results = new List<ReportFormat>();

			this.Querying?.Invoke(this, new PreQueryEventArgs<ReportFormat>(query, authContext));

			using (var context = new ApplicationDbContext())
			{
				var expression = modelMapper.MapModelExpression<ReportFormat, Model.ReportFormat>(query);

				results = context.ReportFormats.Where(expression).ToList().Select(this.ToModelInstance);
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

			this.Queried?.Invoke(this, new PostQueryEventArgs<ReportFormat>(query, results.AsQueryable(), authContext));

			return results;
		}

		/// <summary>
		/// Queries for a report.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of reports.</returns>
		public IEnumerable<ReportFormat> Query(Expression<Func<ReportFormat, bool>> query, IPrincipal authContext)
		{
			var totalCount = 0;
			return this.Query(query, 0, null, authContext, out totalCount);
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override ReportFormat ToModelInstance(Model.ReportFormat domainInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportFormat) } to { nameof(Model.ReportFormat) }");
			return domainInstance == null ? null : modelMapper.MapDomainInstance<Model.ReportFormat, ReportFormat>(domainInstance);
		}

		/// <summary>
		/// Updates a report.
		/// </summary>
		/// <param name="storageData">The report to update.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the updated report.</returns>
		public ReportFormat Update(ReportFormat storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportFormat result = null;

			this.Updating?.Invoke(this, new PrePersistenceEventArgs<ReportFormat>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var domainInstance = this.FromModelInstance(storageData);

				context.Entry(domainInstance).State = EntityState.Modified;

				context.SaveChanges();

				result = this.ToModelInstance(domainInstance);
			}

			this.Updated?.Invoke(this, new PostPersistenceEventArgs<ReportFormat>(result, principal));

			return result;
		}
	}
}