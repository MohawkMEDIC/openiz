/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using System;
using OpenIZ.Core.Model.Security;
using System.Linq.Expressions;
using System.Linq;
using OpenIZ.Core.Model;
using System.Text;
using System.Collections.Generic;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using System.Data.Linq;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core;
using OpenIZ.Core.Model.Reflection;
using System.Reflection;
using OpenIZ.Core.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
	/// <summary>
	/// Generic persistence service which can persist between two simple types.
	/// </summary>
	public abstract class IdentifiedPersistenceService<TModel, TDomain> : SqlServerBasePersistenceService<TModel> 
		where TModel : IdentifiedData, new()
		where TDomain : class, IDbIdentified, new()
	{

        #region implemented abstract members of LocalDataPersistenceService
        /// <summary>
        /// Maps the data to a model instance
        /// </summary>
        /// <returns>The model instance.</returns>
        /// <param name="dataInstance">Data instance.</param>
        public override TModel ToModelInstance (object dataInstance, ModelDataContext context, IPrincipal principal)
		{
			var retVal = m_mapper.MapDomainInstance<TDomain, TModel> (dataInstance as TDomain);
            this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, "Model instance {0} created", dataInstance);

            return retVal;
		}


		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		/// <param name="context">Context.</param>
		public override object FromModelInstance (TModel modelInstance, ModelDataContext context, IPrincipal princpal)
		{
			return m_mapper.MapModelInstance<TModel, TDomain> (modelInstance);

		}

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public override TModel Insert (ModelDataContext context, TModel data, IPrincipal principal)
		{
			var domainObject = this.FromModelInstance (data, context, principal) as TDomain;

            if (domainObject.Id == null ||
                domainObject.Id == Guid.Empty)
                data.Key = domainObject.Id = Guid.NewGuid();

            context.GetTable<TDomain>().InsertOnSubmit (domainObject);
            context.SubmitChanges();

            return data;
		}

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Update (ModelDataContext context, TModel data, IPrincipal principal)
		{
            // Sanity 
            if(data.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            // Map and copy
            var newDomainObject = this.FromModelInstance (data, context, principal) as TDomain;
            var oldDomainObject = context.GetTable<TDomain>().Single(ExpressionRewriter.Rewrite<TDomain>(o => o.Id == newDomainObject.Id));
            if (oldDomainObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

            oldDomainObject.CopyObjectData(newDomainObject);
            context.SubmitChanges();
			return data;
		}

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public override TModel Obsolete (ModelDataContext context, TModel data, IPrincipal principal)
		{
            if (data.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);

            var domainObject = context.GetTable<TDomain>().FirstOrDefault(ExpressionRewriter.Rewrite<TDomain>(o => o.Id == data.Key));

            if (domainObject == null)
                throw new KeyNotFoundException(data.Key.ToString());

			context.GetTable<TDomain>().DeleteOnSubmit (domainObject);
			return data;
		}

        /// <summary>
        /// Performs the actual query
        /// </summary>
        public override IQueryable<TModel> Query (ModelDataContext context, Expression<Func<TModel, bool>> query, IPrincipal principal)
		{
            var access = typeof(TDomain)?.GetProperty("Id")?.GetCustomAttribute<LinqPropertyMapAttribute>();
            var cacheService = ApplicationContext.Current.GetService<IDataCachingService>();

            //if (access != null && cacheService != null)
            //{
            //    var parm = Expression.Parameter(typeof(TDomain));
            //    Expression<Func<TDomain, Guid>> selectExpression = Expression.Lambda<Func<TDomain, Guid>>(Expression.MakeMemberAccess(parm, typeof(TDomain).GetRuntimeProperty(access.LinqMember)), parm);

            //    var internalQuery = this.QueryInternal(context, query);
            //    return internalQuery.Select(selectExpression).Select(o => cacheService.GetCacheItem<TModel>(o) ?? this.ToModelInstance(context.GetTable<TDomain>().Where(ExpressionRewriter.Rewrite<TDomain>(p=>p.Id == o)).Single, context, principal));
            //}
            //else
                return this.QueryInternal(context, query).Select(o=>this.CacheLoad(o, context, principal));
		}

        /// <summary>
        /// Tru to load from cache
        /// </summary>
        private TModel CacheLoad(TDomain o, ModelDataContext context, IPrincipal principal)
        {
            return ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem<TModel>(o.Id) ?? this.ToModelInstance(o, context, principal);
        }


        /// <summary>
        /// Perform the query 
        /// </summary>
        protected IQueryable<TDomain> QueryInternal(ModelDataContext context, Expression<Func<TModel, bool>> query)
        {
            var domainQuery = m_mapper.MapModelExpression<TModel, TDomain>(query);
            this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Verbose, 0, "LINQ2SQL: {0}", domainQuery);
            return context.GetTable<TDomain>().Where(domainQuery);
        }

        /// <summary>
        /// Update associated items
        /// </summary>
        protected virtual void UpdateAssociatedItems<TAssociation, TDomainAssociation>(IEnumerable<TAssociation> storage, TModel source, ModelDataContext context, IPrincipal principal)
            where TAssociation : IdentifiedData, ISimpleAssociation, new()
            where TDomainAssociation : class, IDbAssociation, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as SqlServerBasePersistenceService<TAssociation>;
            if (persistenceService == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, "Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }
            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey == Guid.Empty ||
                    itm.SourceEntityKey == null)
                    itm.SourceEntityKey = source.Key;

            // Get existing
            var existing = context.GetTable<TDomainAssociation>().Where(ExpressionRewriter.Rewrite<TDomainAssociation>(o => o.AssociatedItemKey == source.Key)).ToList().Select(o=>m_mapper.MapDomainInstance<TDomainAssociation, TAssociation>(o) as TAssociation);
            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Any(ecn => ecn.Key == o.Key));
            foreach (var del in obsoleteRecords)
                persistenceService.Obsolete(context, del, principal);

            // Update those that need it
            var updateRecords = storage.Where(o => existing.Any(ecn => ecn.Key == o.Key && o.Key != Guid.Empty && o != ecn));
            foreach (var upd in updateRecords)
                persistenceService.Update(context, upd, principal);

            // Insert those that do not exist
            var insertRecords = storage.Where(o => !existing.Any(ecn => ecn.Key == o.Key));
            foreach (var ins in insertRecords)
                persistenceService.Insert(context, ins, principal);


        }
        #endregion
    }
}

