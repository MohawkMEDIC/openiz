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

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Core.Security;
using OpenIZ.Persistence.Reporting.MSSQL.Context;
using OpenIZ.Persistence.Reporting.MSSQL.Exceptions;

namespace OpenIZ.Persistence.Reporting.MSSQL.Services
{
	/// <summary>
	/// Represents a ReportDefinition persistence service.
	/// </summary>
	internal class ReportDefinitionPersistenceService : ReportPersistenceServiceBase<ReportDefinition, MSSQL.Model.ReportDefinition>, IDataPersistenceService<ReportDefinition>
	{
		/// <summary>
		/// The internal reference to the <see cref="TraceSource"/> instance.
		/// </summary>
		private readonly TraceSource tracer = new TraceSource("OpenIZ.Persistence.Reporting");

		/// <summary>
		/// The internal reference to the <see cref="ParameterType"/> <see cref="IDataPersistenceService{TData}"/> instance.
		/// </summary>
		private readonly IDataPersistenceService<ParameterType> parameterTypePersistenceService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReportDefinitionPersistenceService"/> class.
		/// </summary>
		public ReportDefinitionPersistenceService()
		{
			this.parameterTypePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ParameterType>>();
		}

		/// <summary>
		/// Fired after a report is inserted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportDefinition>> Inserted;

		/// <summary>
		/// Fired while a report is being inserted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportDefinition>> Inserting;

		/// <summary>
		/// Fired after a report is obsoleted.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportDefinition>> Obsoleted;

		/// <summary>
		/// Fired while a report is being obsoleted.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportDefinition>> Obsoleting;

		/// <summary>
		/// Fired after a report is queried.
		/// </summary>
		public event EventHandler<PostQueryEventArgs<ReportDefinition>> Queried;

		/// <summary>
		/// Fired while a report is being queried.
		/// </summary>
		public event EventHandler<PreQueryEventArgs<ReportDefinition>> Querying;

		/// <summary>
		/// Fired after a report is been retrieved.
		/// </summary>
		public event EventHandler<PostRetrievalEventArgs<ReportDefinition>> Retrieved;

		/// <summary>
		/// Fired while a report is being retrieved.
		/// </summary>
		public event EventHandler<PreRetrievalEventArgs> Retrieving;

		/// <summary>
		/// Fired after a report is updated.
		/// </summary>
		public event EventHandler<PostPersistenceEventArgs<ReportDefinition>> Updated;

		/// <summary>
		/// Fired while a report is being updated.
		/// </summary>
		public event EventHandler<PrePersistenceEventArgs<ReportDefinition>> Updating;

		/// <summary>
		/// Gets the count of a query.
		/// </summary>
		/// <param name="query">The query for which to determine the count.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns the count of the query.</returns>
		public int Count(Expression<Func<ReportDefinition, bool>> query, IPrincipal authContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override MSSQL.Model.ReportDefinition FromModelInstance(ReportDefinition modelInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(MSSQL.Model.ReportDefinition) } to { nameof(ReportDefinition) }");

			return modelInstance == null ? null : ModelMapper.MapModelInstance<ReportDefinition, MSSQL.Model.ReportDefinition>(modelInstance);
		}

		/// <summary>
		/// Gets a report by id.
		/// </summary>
		/// <typeparam name="TIdentifier">The type of identifier.</typeparam>
		/// <param name="containerId">The id of the ReportDefinition.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="loadFast">Whether the result should load fast.</param>
		/// <returns>Returns the report or null if not found.</returns>
		public ReportDefinition Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			ReportDefinition result = null;

			using (var context = new ApplicationDbContext())
			{
                var evt = new PreRetrievalEventArgs(containerId, principal);
                this.Retrieving?.Invoke(this, evt);
                if (evt.Cancel)
                    throw new OperationCanceledException();

                var reportDefinition = context.ReportDefinitions.Find(containerId.Id);

				result = this.ToModelInstance(reportDefinition);

				if (result != null)
				{
					if (!loadFast)
					{
						result.Parameters = reportDefinition.Parameters.ToList().Select(p => new ReportParameter
						{
							Key = p.Id,
							CreationTime = p.CreationTime,
							Name = p.Name,
							IsNullable = p.IsNullable,
							Order = p.Order,
							ParameterType = this.parameterTypePersistenceService.Get<Guid>(new Identifier<Guid>(p.ParameterTypeId), AuthenticationContext.Current.Principal, true),
							Value = p.Value
						}).ToList();
					}
				}
			}

			this.Retrieved?.Invoke(this, new PostRetrievalEventArgs<ReportDefinition>(result, principal));

			return result;
		}

		/// <summary>
		/// Inserts a report.
		/// </summary>
		/// <param name="storageData">The report to insert.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the inserted report.</returns>
		public ReportDefinition Insert(ReportDefinition storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportDefinition result = null;

			this.Inserting?.Invoke(this, new PrePersistenceEventArgs<ReportDefinition>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = this.FromModelInstance(storageData);

				if (string.IsNullOrEmpty(entity.Author) || string.IsNullOrWhiteSpace(entity.Author))
				{
					entity.Author = "SYSTEM";
				}

				context.ReportDefinitions.Add(entity);
				context.SaveChanges();

				result = this.ToModelInstance(entity);
			}

			this.Inserted?.Invoke(this, new PostPersistenceEventArgs<ReportDefinition>(result, principal));

			return result;
		}

		/// <summary>
		/// Obsoletes a report.
		/// </summary>
		/// <param name="storageData">The report to obsolete.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the obsoleted report.</returns>
		public ReportDefinition Obsolete(ReportDefinition storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportDefinition result = null;

			this.Obsoleting?.Invoke(this, new PrePersistenceEventArgs<ReportDefinition>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = context.ReportDefinitions.Find(storageData.Key);

				if (entity == null)
				{
					throw new EntityNotFoundException();
				}

				result = this.ToModelInstance(context.ReportDefinitions.Remove(entity));
			}

			this.Obsoleted?.Invoke(this, new PostPersistenceEventArgs<ReportDefinition>(result, principal));

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
		public IEnumerable<ReportDefinition> Query(Expression<Func<ReportDefinition, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
		{
			IEnumerable<ReportDefinition> results = new List<ReportDefinition>();

			this.Querying?.Invoke(this, new PreQueryEventArgs<ReportDefinition>(query, authContext));

			using (var context = new ApplicationDbContext())
			{
				var expression = ModelMapper.MapModelExpression<ReportDefinition, MSSQL.Model.ReportDefinition>(query);

				results = context.ReportDefinitions.Where(expression).ToList().Select(this.ToModelInstance);
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

			this.Queried?.Invoke(this, new PostQueryEventArgs<ReportDefinition>(query, results.AsQueryable(), authContext));

			return results;
		}

		/// <summary>
		/// Queries for a report.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of reports.</returns>
		public IEnumerable<ReportDefinition> Query(Expression<Func<ReportDefinition, bool>> query, IPrincipal authContext)
		{
			var totalCount = 0;
			return this.Query(query, 0, null, authContext, out totalCount);
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override ReportDefinition ToModelInstance(MSSQL.Model.ReportDefinition domainInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportDefinition) } to { nameof(MSSQL.Model.ReportDefinition) }");

			return domainInstance == null ? null : ModelMapper.MapDomainInstance<MSSQL.Model.ReportDefinition, ReportDefinition>(domainInstance);
		}

		/// <summary>
		/// Updates a report.
		/// </summary>
		/// <param name="storageData">The report to update.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="mode">The mode of the transaction.</param>
		/// <returns>Returns the updated report.</returns>
		public ReportDefinition Update(ReportDefinition storageData, IPrincipal principal, TransactionMode mode)
		{
			ReportDefinition result = null;

			this.Updating?.Invoke(this, new PrePersistenceEventArgs<ReportDefinition>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var domainInstance = this.FromModelInstance(storageData);

				if (string.IsNullOrEmpty(domainInstance.Author) || string.IsNullOrWhiteSpace(domainInstance.Author))
				{
					domainInstance.Author = "SYSTEM";
				}

				context.Entry(domainInstance).State = EntityState.Modified;

				context.SaveChanges();

				result = this.ToModelInstance(domainInstance);
			}

			this.Updated?.Invoke(this, new PostPersistenceEventArgs<ReportDefinition>(result, principal));

			return result;
		}
	}
}