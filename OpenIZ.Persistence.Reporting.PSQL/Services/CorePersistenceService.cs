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
 * Date: 2017-4-16
 */

using OpenIZ.Core.Model;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Reporting.PSQL.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.PSQL.Services
{
	/// <summary>
	/// Represents a core persistence service.
	/// </summary>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	/// <typeparam name="TDomain">The type of the domain.</typeparam>
	/// <typeparam name="TQueryReturn">The type of the query return.</typeparam>
	/// <seealso cref="ReportPersistenceServiceBase{TModel}" />
	public abstract class CorePersistenceService<TModel, TDomain, TQueryReturn> : ReportPersistenceServiceBase<TModel> where TModel : IdentifiedData, new() where TDomain : DbIdentified, new()
	{
		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override object FromModelInstance(TModel modelInstance, DataContext context, IPrincipal principal)
		{
			return ModelMapper.MapModelInstance<TModel, TDomain>(modelInstance);
		}

		/// <summary>
		/// Inserts the model.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the inserted model.</returns>
		public override TModel InsertInternal(DataContext context, TModel model, IPrincipal principal)
		{
			var domainInstance = this.FromModelInstance(model, context, principal);

			domainInstance = context.Insert(domainInstance as TDomain);

			return this.ToModelInstance(domainInstance, context, principal);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the obsoleted data.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		public override TModel ObsoleteInternal(DataContext context, TModel model, IPrincipal principal)
		{
			if (model.Key == Guid.Empty)
			{
				throw new InvalidOperationException($"Cannot delete data using key: {model.Key}");
			}

			context.Delete<TDomain>(o => o.Key == model.Key);

			return model;
		}

		/// <summary>
		/// Queries for the specified model.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <param name="countResults">if set to <c>true</c> [count results].</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns a list of the specified model instance which match the given query expression.</returns>
		public override IEnumerable<TModel> QueryInternal(DataContext context, Expression<Func<TModel, bool>> query, int offset, int? count, out int totalResults, bool countResults, IPrincipal principal)
		{
			try
			{
				// Domain query
				SqlStatement domainQuery = context.CreateSqlStatement<TDomain>().SelectFrom();

				var expression = ModelMapper.MapModelExpression<TModel, TDomain>(query, false);

				if (expression != null)
				{
					Type lastJoined = typeof(TDomain);
					if (typeof(CompositeResult).IsAssignableFrom(typeof(TQueryReturn)))
						foreach (var p in typeof(TQueryReturn).GenericTypeArguments.Select(o => ReportingPersistenceService.ModelMapper.MapModelType(o)))
							if (p != typeof(TDomain))
							{
								// Find the FK to join
								domainQuery.InnerJoin(lastJoined, p);
								lastJoined = p;
							}

					domainQuery.Where(expression);
				}
				else
				{
					this.traceSource.TraceEvent(TraceEventType.Verbose, 0, "Will use slow query construction due to complex mapped fields");
					domainQuery = ReportingPersistenceService.QueryBuilder.CreateQuery(query);
				}

				// Build and see if the query already exists on the stack???
				domainQuery = domainQuery.Build();

				if (Configuration.TraceSql)
				{
					traceSource.TraceEvent(TraceEventType.Verbose, 0, "Trace SQL flag is set to true, printing SQL statement");
					traceSource.TraceEvent(TraceEventType.Verbose, 0, $"GENERATED SQL STATEMENT: {domainQuery.SQL}");
				}

				if (offset > 0)
				{
					domainQuery.Offset(offset);
				}
				if (count.HasValue)
				{
					domainQuery.Limit(count.Value);
				}

				var results = context.Query<TQueryReturn>(domainQuery).OfType<object>();
				totalResults = results.Count();

				return results.Select(r => ToModelInstance(r, context, principal));
			}
			catch (Exception e)
			{
				traceSource.TraceEvent(TraceEventType.Error, 0, $"Unable to query: {e}");
				throw;
			}
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override TModel ToModelInstance(object domainInstance, DataContext context, IPrincipal principal)
		{
			return ModelMapper.MapDomainInstance<TDomain, TModel>(domainInstance as TDomain);
		}

		/// <summary>
		/// Updates the specified storage data.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the updated model instance.</returns>
		public override TModel UpdateInternal(DataContext context, TModel model, IPrincipal principal)
		{
			var domainInstance = this.FromModelInstance(model, context, principal);

			domainInstance = context.Update(domainInstance as TDomain);

			return this.ToModelInstance(domainInstance, context, principal);
		}
	}
}