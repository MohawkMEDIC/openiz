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
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Event;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using OpenIZ.Persistence.Reporting.Context;
using OpenIZ.Persistence.Reporting.Exceptions;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a data type persistence service.
	/// </summary>
	internal class ParameterTypePersistenceService : ReportPersistenceServiceBase<ParameterType, Model.ParameterType>, IDataPersistenceService<ParameterType>
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
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override Model.ParameterType FromModelInstance(ParameterType modelInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(Model.ParameterType) } to { nameof(ParameterType) }");
			return modelInstance == null ? null : modelMapper.MapModelInstance<ParameterType, Model.ParameterType>(modelInstance);
		}

		/// <summary>
		/// Gets a report by id.
		/// </summary>
		/// <typeparam name="TIdentifier">The type of identifier.</typeparam>
		/// <param name="containerId">The id of the report definition.</param>
		/// <param name="principal">The authorization context.</param>
		/// <param name="loadFast">Whether the result should load fast.</param>
		/// <returns>Returns the report or null if not found.</returns>
		public ParameterType Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
		{
			ParameterType result = null;

			using (var context = new ApplicationDbContext())
			{
				var parameterType = context.ParameterTypes.Find(containerId.Id);

				result = this.ToModelInstance(parameterType);

				this.Retrieving?.Invoke(this, new PreRetrievalEventArgs<ParameterType>(result, principal));
			}

			this.Retrieved?.Invoke(this, new PostRetrievalEventArgs<ParameterType>(result, principal));

			return result;
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
			ParameterType result = null;

			this.Inserting?.Invoke(this, new PrePersistenceEventArgs<ParameterType>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = this.FromModelInstance(storageData);

				context.ParameterTypes.Add(entity);
				context.SaveChanges();

				result = this.ToModelInstance(entity);
			}

			this.Inserted?.Invoke(this, new PostPersistenceEventArgs<ParameterType>(result, principal));

			return result;
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
			ParameterType result = null;

			this.Obsoleting?.Invoke(this, new PrePersistenceEventArgs<ParameterType>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var entity = context.ParameterTypes.Find(storageData.Key);

				if (entity == null)
				{
					throw new EntityNotFoundException();
				}

				result = this.ToModelInstance(context.ParameterTypes.Remove(entity));
			}

			this.Obsoleted?.Invoke(this, new PostPersistenceEventArgs<ParameterType>(result, principal));

			return result;
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
			IEnumerable<ParameterType> results = new List<ParameterType>();

			this.Querying?.Invoke(this, new PreQueryEventArgs<ParameterType>(query, authContext));

			using (var context = new ApplicationDbContext())
			{
				var expression = modelMapper.MapModelExpression<ParameterType, Model.ParameterType>(query);

				results = context.ParameterTypes.Where(expression).ToList().Select(this.ToModelInstance);
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

			this.Queried?.Invoke(this, new PostQueryEventArgs<ParameterType>(query, results.AsQueryable(), authContext));

			return results;
		}

		/// <summary>
		/// Queries for a data type.
		/// </summary>
		/// <param name="query">The query for which to retrieve results.</param>
		/// <param name="authContext">The authorization context.</param>
		/// <returns>Returns a list of data types.</returns>
		public IEnumerable<ParameterType> Query(Expression<Func<ParameterType, bool>> query, IPrincipal authContext)
		{
			var totalCount = 0;
			return this.Query(query, 0, null, authContext, out totalCount);
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal override ParameterType ToModelInstance(Model.ParameterType domainInstance)
		{
			this.tracer.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ParameterType) } to { nameof(Model.ParameterType) }");
			return domainInstance == null ? null : modelMapper.MapDomainInstance<Model.ParameterType, ParameterType>(domainInstance);
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
			ParameterType result = null;

			this.Updating?.Invoke(this, new PrePersistenceEventArgs<ParameterType>(storageData, principal));

			using (var context = new ApplicationDbContext())
			{
				var domainInstance = this.FromModelInstance(storageData);

				context.Entry(domainInstance).State = EntityState.Modified;

				context.SaveChanges();

				result = this.ToModelInstance(domainInstance);
			}

			this.Updated?.Invoke(this, new PostPersistenceEventArgs<ParameterType>(result, principal));

			return result;
		}
	}
}
