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
 * Date: 2017-1-21
 */
using OpenIZ.Core.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System.ComponentModel;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Services;
using OpenIZ.Core;
using System.Reflection;
using System.Linq.Expressions;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Exceptions;
using OpenIZ.OrmLite;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Diagnostics;
using OpenIZ.Core.Exceptions;
using OpenIZ.Persistence.Data.ADO.Data.Model.Concepts;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Versioned domain data
    /// </summary>
    public abstract class VersionedDataPersistenceService<TModel, TDomain, TDomainKey> : BaseDataPersistenceService<TModel, TDomain, CompositeResult<TDomain, TDomainKey>>
        where TDomain : class, IDbVersionedData, new()
        where TModel : VersionedEntityData<TModel>, new()
        where TDomainKey : IDbIdentified, new()
    {

        /// <summary>
        /// Insert the data
        /// </summary>
        public override TModel InsertInternal(DataContext context, TModel data, IPrincipal principal)
        {
            // Ensure exists
            data.CreatedBy?.EnsureExists(context, principal);
            data.CreatedByKey = data.CreatedBy?.Key ?? data.CreatedByKey;

            // first we map the TDataKey entity
            var nonVersionedPortion = m_mapper.MapModelInstance<TModel, TDomainKey>(data);

            // Domain object
            var domainObject = this.FromModelInstance(data, context, principal) as TDomain;

            // First we must assign non versioned portion data
            if (nonVersionedPortion.Key == Guid.Empty &&
                domainObject.Key != Guid.Empty)
                nonVersionedPortion.Key = domainObject.Key;

            if (nonVersionedPortion.Key == null ||
                nonVersionedPortion.Key == Guid.Empty)
            {
                data.Key = Guid.NewGuid();
                domainObject.Key = nonVersionedPortion.Key = data.Key.Value;
            }
            if (domainObject.VersionKey == null ||
                domainObject.VersionKey == Guid.Empty)
            {
                data.VersionKey = Guid.NewGuid();
                domainObject.VersionKey = data.VersionKey.Value;
            }

            // Now we want to insert the non versioned portion first
            nonVersionedPortion = context.Insert(nonVersionedPortion);

            // Ensure created by exists
            data.CreatedByKey = domainObject.CreatedByKey = domainObject.CreatedByKey == Guid.Empty ? principal.GetUserKey(context).Value : domainObject.CreatedByKey;

            if (data.CreationTime == DateTimeOffset.MinValue || data.CreationTime.Year < 100)
                data.CreationTime = DateTimeOffset.Now;
            domainObject.CreationTime = data.CreationTime;
            domainObject.VersionSequenceId = null;
            domainObject = context.Insert(domainObject);
            data.VersionSequence = domainObject.VersionSequenceId;
            data.VersionKey = domainObject.VersionKey;
            data.Key = domainObject.Key;
            data.CreationTime = (DateTimeOffset)domainObject.CreationTime;
            return data;

        }

        /// <summary>
        /// Update the data with new version information
        /// </summary>
        public override TModel UpdateInternal(DataContext context, TModel data, IPrincipal principal)
        {
            if (data.Key == Guid.Empty)
                throw new AdoFormalConstraintException(AdoFormalConstraintType.NonIdentityUpdate);

            data.CreatedBy.EnsureExists(context, principal);
            data.CreatedByKey = data.CreatedBy?.Key ?? data.CreatedByKey;


            // This is technically an insert and not an update
            SqlStatement currentVersionQuery = context.CreateSqlStatement<TDomain>().SelectFrom()
                .Where(o => o.Key == data.Key && !o.ObsoletionTime.HasValue)
                .OrderBy<TDomain>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);

            var existingObject = context.FirstOrDefault<TDomain>(currentVersionQuery); // Get the last version (current)
            var nonVersionedObect = context.FirstOrDefault<TDomainKey>(o => o.Key == data.Key);

            if (existingObject == null)
                throw new KeyNotFoundException(data.Key.ToString());
            else if ((existingObject as IDbReadonly)?.IsReadonly == true ||
                (nonVersionedObect as IDbReadonly)?.IsReadonly == true)
                throw new AdoFormalConstraintException(AdoFormalConstraintType.UpdatedReadonlyObject);

            // Map existing
            var storageInstance = this.FromModelInstance(data, context, principal);

            // Create a new version
            var user = principal.GetUserKey(context);
            var newEntityVersion = new TDomain();
            newEntityVersion.CopyObjectData(storageInstance);

            // Client did not change on update, so we need to update!!!
            if (!data.VersionKey.HasValue ||
               data.VersionKey.Value == existingObject.VersionKey ||
               context.Any<TDomain>(o => o.VersionKey == data.VersionKey))
                data.VersionKey = newEntityVersion.VersionKey = Guid.NewGuid();

            data.VersionSequence = newEntityVersion.VersionSequenceId = null;
            newEntityVersion.Key = data.Key.Value;
            data.PreviousVersionKey = newEntityVersion.ReplacesVersionKey = existingObject.VersionKey;
            data.CreatedByKey = newEntityVersion.CreatedByKey = data.CreatedByKey ?? user.Value;
            // Obsolete the old version 
            existingObject.ObsoletedByKey = data.CreatedByKey ?? user;
            existingObject.ObsoletionTime = DateTimeOffset.Now;
            newEntityVersion.CreationTime = DateTimeOffset.Now;

	        context.Update(existingObject);

	        newEntityVersion = context.Insert<TDomain>(newEntityVersion);
			nonVersionedObect = context.Update<TDomainKey>(nonVersionedObect);

            // Pull database generated fields
            data.VersionSequence = newEntityVersion.VersionSequenceId;
            data.CreationTime = newEntityVersion.CreationTime;

            return data;
            //return base.Update(context, data, principal);
        }


        /// <summary>
        /// Order by
        /// </summary>
        /// <param name="rawQuery"></param>
        /// <returns></returns>
        protected override SqlStatement AppendOrderBy(SqlStatement rawQuery)
        {
            return rawQuery.OrderBy<TDomain>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
        }
        /// <summary>
        /// Query internal
        /// </summary>
        protected override IEnumerable<Object> QueryInternal(DataContext context, Expression<Func<TModel, bool>> query, Guid queryId, int offset, int? count, out int totalResults, bool countResults = true)
        {
            // Is obsoletion time already specified?
            if (!query.ToString().Contains("ObsoletionTime"))
            {
                var obsoletionReference = Expression.MakeBinary(ExpressionType.Equal, Expression.MakeMemberAccess(query.Parameters[0], typeof(TModel).GetProperty(nameof(BaseEntityData.ObsoletionTime))), Expression.Constant(null));
                query = Expression.Lambda<Func<TModel, bool>>(Expression.MakeBinary(ExpressionType.AndAlso, obsoletionReference, query.Body), query.Parameters);
            }

            // Query has been registered?
            if (this.m_queryPersistence?.IsRegistered(queryId.ToString()) == true)
            {
                totalResults = (int)this.m_queryPersistence.QueryResultTotalQuantity(queryId.ToString());
                var keyResults = this.m_queryPersistence.GetQueryResults<Guid>(queryId.ToString(), offset, count.Value);
                return keyResults.Select(p => p.Id).OfType<Object>();
            }

            SqlStatement domainQuery = null;
            try
            {
                domainQuery = context.CreateSqlStatement<TDomain>().SelectFrom()
                    .InnerJoin<TDomain, TDomainKey>(o => o.Key, o => o.Key)
                    .Where<TDomain>(m_mapper.MapModelExpression<TModel, TDomain>(query)).Build();

            }
            catch (Exception e)
            {
                m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Verbose, e.HResult, "Will use slow query construction due to {0}", e.Message);
                domainQuery = AdoPersistenceService.GetQueryBuilder().CreateQuery(query).Build();
            }

            domainQuery = this.AppendOrderBy(domainQuery);
                

            // Query id just get the UUIDs in the db
            if (queryId != Guid.Empty)
            {
                ColumnMapping pkColumn = TableMapping.Get(typeof(TDomainKey)).Columns.SingleOrDefault(o => o.IsPrimaryKey);

                var keyQuery = AdoPersistenceService.GetQueryBuilder().CreateQuery(query, pkColumn).Build();
                var resultKeys = context.Query<Guid>(keyQuery.Build());
                this.m_queryPersistence?.RegisterQuerySet(queryId.ToString(), resultKeys.Count(), resultKeys.Take(1000).Select(o => new Identifier<Guid>(o)).ToArray(), query);

                ApplicationContext.Current.GetService<IThreadPoolService>().QueueNonPooledWorkItem(o =>
                {
                    int ofs = 1000;
                    var rkeys = o as Guid[];
	                if (rkeys == null)
	                {
		                return;
	                }
                    while (ofs < rkeys.Length)
                    {
                        this.m_queryPersistence?.AddResults(queryId.ToString(), rkeys.Skip(ofs).Take(1000).Select(k => new Identifier<Guid>(k)).ToArray());
                        ofs += 1000;
                    }
                }, resultKeys.ToArray());

                if (countResults)
                    totalResults = (int)resultKeys.Count();
                else
                    totalResults = 0;

                var retVal = resultKeys.Skip(offset);
                if (count.HasValue)
                    retVal = retVal.Take(count.Value);
                return retVal.OfType<Object>();
            }
            else if (countResults)
            {
                totalResults = context.Count(domainQuery);
                if (totalResults == 0)
                    return new List<CompositeResult<TDomain, TDomainKey>>();
            }
            else
                totalResults = 0;

            if (offset > 0)
                domainQuery.Offset(offset);
            if (count.HasValue)
                domainQuery.Limit(count.Value);
            return context.Query<CompositeResult<TDomain, TDomainKey>>(domainQuery).OfType<Object>();

        }

        /// <summary>
        /// Perform a version aware get
        /// </summary>
        internal override TModel Get(DataContext context, Guid key, IPrincipal principal)
        {
            // Attempt to get a cahce item
            var cacheService = new AdoPersistenceCache(context);
            var retVal = cacheService.GetCacheItem<TModel>(key);
            if (retVal != null)
                return retVal;
            else
            {
                var domainQuery = context.CreateSqlStatement<TDomain>().SelectFrom()
                    .InnerJoin<TDomain, TDomainKey>(o => o.Key, o => o.Key)
                    .Where<TDomain>(o => o.Key == key && o.ObsoletionTime == null)
                    .OrderBy<TDomain>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
                return this.CacheConvert(context.FirstOrDefault<CompositeResult<TDomain, TDomainKey>>(domainQuery), context, principal);
            }
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public override TModel Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            var tr = 0;
            var uuid = containerId as Identifier<Guid>;

            if (uuid.Id != Guid.Empty)
            {

                var cacheItem = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem<TModel>(uuid.Id) as TModel;
                if (cacheItem != null && (cacheItem.VersionKey.HasValue && uuid.VersionId == cacheItem.VersionKey.Value || uuid.VersionId == Guid.Empty) &&
                    (loadFast && cacheItem.LoadState >= LoadState.PartialLoad || !loadFast && cacheItem.LoadState == LoadState.FullLoad))
                    return cacheItem;
            }

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            PreRetrievalEventArgs preArgs = new PreRetrievalEventArgs(containerId, principal);
            this.FireRetrieving(preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort retrieve {0}", containerId.Id);
                return null;
            }

            // Query object
            using (var connection = m_configuration.Provider.GetReadonlyConnection())
                try
                {
                    connection.Open();
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "GET {0}", containerId);

                    TModel retVal = null;
                    if (loadFast)
                        connection.LoadState = LoadState.PartialLoad;
                    else
                        connection.LoadState = LoadState.FullLoad;

                    // Get most recent version
                    if (uuid.VersionId == Guid.Empty)
                        retVal = this.Get(connection, uuid.Id, principal);
                    else
                        retVal = this.CacheConvert(this.QueryInternal(connection, o => o.Key == uuid.Id && o.VersionKey == uuid.VersionId && o.ObsoletionTime == null || o.ObsoletionTime != null, Guid.Empty, 0, 1, out tr).FirstOrDefault(), connection, principal);

                    var postData = new PostRetrievalEventArgs<TModel>(retVal, principal);
                    this.FireRetrieved(postData);

                    return retVal;

                }
                catch (NotSupportedException e)
                {
                    throw new DataPersistenceException("Cannot perform LINQ query", e);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    throw;
                }
                finally
                {
#if DEBUG
                    sw.Stop();
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Retrieve took {0} ms", sw.ElapsedMilliseconds);
#endif
                }

        }

        /// <summary>
        /// Update versioned association items
        /// </summary>
        internal virtual void UpdateVersionedAssociatedItems<TAssociation, TDomainAssociation>(IEnumerable<TAssociation> storage, TModel source, DataContext context, IPrincipal principal)
            where TAssociation : VersionedAssociation<TModel>, new()
            where TDomainAssociation : class, IDbVersionedAssociation, IDbIdentified, new()
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAssociation>>() as AdoBasePersistenceService<TAssociation>;
            if (persistenceService == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, "Missing persister for type {0}", typeof(TAssociation).Name);
                return;
            }

            Dictionary<Guid, Decimal> sourceVersionMaps = new Dictionary<Guid, decimal>();

            // Ensure the source key is set
            foreach (var itm in storage)
                if (itm.SourceEntityKey.GetValueOrDefault() == Guid.Empty ||
                    itm.SourceEntityKey == null)
                    itm.SourceEntityKey = source.Key;
                else if (itm.SourceEntityKey != source.Key && !sourceVersionMaps.ContainsKey(itm.SourceEntityKey ?? Guid.Empty)) // The source comes from somewhere else
                {

                    SqlStatement versionQuery = null;
                    // Get the current tuple 
                    IDbVersionedData currentVersion = null;

                    // We need to figure out what the current version of the source item is ... 
                    // Since this is a versioned association an a versioned association only exists between Concept, Act, or Entity
                    if (itm is VersionedAssociation<Concept>)
                    {
                        versionQuery = context.CreateSqlStatement<DbConceptVersion>().SelectFrom().Where(o => o.VersionKey == itm.SourceEntityKey && !o.ObsoletionTime.HasValue).OrderBy<DbConceptVersion>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
                        currentVersion = context.FirstOrDefault<DbConceptVersion>(versionQuery);
                    }
                    else if (itm is VersionedAssociation<Act>)
                    {
                        versionQuery = context.CreateSqlStatement<DbActVersion>().SelectFrom().Where(o => o.Key == itm.SourceEntityKey && !o.ObsoletionTime.HasValue).OrderBy<DbActVersion>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
                        currentVersion = context.FirstOrDefault<DbActVersion>(versionQuery);
                    }
                    else if (itm is VersionedAssociation<Entity>)
                    {
                        versionQuery = context.CreateSqlStatement<DbEntityVersion>().SelectFrom().Where(o => o.Key == itm.SourceEntityKey && !o.ObsoletionTime.HasValue).OrderBy<DbEntityVersion>(o => o.VersionSequenceId, Core.Model.Map.SortOrderType.OrderByDescending);
                        currentVersion = context.FirstOrDefault<DbEntityVersion>(versionQuery);
                    }

                    if(currentVersion != null)
                        sourceVersionMaps.Add(itm.SourceEntityKey.Value, currentVersion.VersionSequenceId.Value);
                }

            // Get existing
            // TODO: What happens which this is reverse?
            var existing = context.Query<TDomainAssociation>(o => o.SourceKey == source.Key);

            // Remove old
            var obsoleteRecords = existing.Where(o => !storage.Any(ecn => ecn.Key == o.Key));
            foreach (var del in obsoleteRecords)
            {
                decimal obsVersion = 0;
                if (!sourceVersionMaps.TryGetValue(del.SourceKey, out obsVersion))
                    obsVersion = source.VersionSequence.GetValueOrDefault();

#if DEBUG
                this.m_tracer.TraceInformation("----- OBSOLETING {0} {1} ---- ", del.GetType().Name, del.Key);
#endif
                del.ObsoleteVersionSequenceId = obsVersion;
                context.Update<TDomainAssociation>(del);
            }

            // Update those that need it
            var updateRecords = storage.Select(o => new { store = o, existing = existing.FirstOrDefault(ecn => ecn.Key == o.Key && o.Key != Guid.Empty && o != ecn) }).Where(o=>o.existing != null);
            foreach (var upd in updateRecords)
            { 
                // Update by key, these lines make no sense we just update the existing versioned association
                //upd.store.EffectiveVersionSequenceId = upd.existing.EffectiveVersionSequenceId;
                //upd.store.ObsoleteVersionSequenceId = upd.existing.EffectiveVersionSequenceId;
                persistenceService.UpdateInternal(context, upd.store as TAssociation, principal);
            }

            // Insert those that do not exist
            var insertRecords = storage.Where(o => !existing.Any(ecn => ecn.Key == o.Key));
            foreach (var ins in insertRecords)
            {
                decimal eftVersion = 0;
                if (!sourceVersionMaps.TryGetValue(ins.SourceEntityKey.Value, out eftVersion))
                    eftVersion = source.VersionSequence.GetValueOrDefault();
                ins.EffectiveVersionSequenceId = eftVersion;

                persistenceService.InsertInternal(context, ins, principal);
            }
        }
    }
}
