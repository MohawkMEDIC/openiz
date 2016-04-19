/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-19
 */
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Event;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Map;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using System.Configuration;
using OpenIZ.Core.Model.Security;
using System.Diagnostics;
using System.Security.Claims;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.Linq;
using System.Security.Principal;
using OpenIZ.Core.Exceptions;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using OpenIZ.Core.Model.Interfaces;
using MARC.HI.EHRS.SVC.Core;
using System.Threading;
using System.Data.Linq.Mapping;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{

    /// <summary>
    /// Represents a basic persistence service
    /// </summary>
    /// <typeparam name="TModel">The OpenIZ Model type</typeparam>
    public abstract class BaseDataPersistenceService<TModel> : IDataPersistenceService<TModel> where TModel : IdentifiedData, new()
    {
        
        /// <summary>
        /// Cache of loaded objects if desired
        /// </summary>
        protected Dictionary<Guid, CacheItem> m_cache = new Dictionary<Guid, CacheItem>();

        /// <summary>
        /// Lock object
        /// </summary>
        protected Object m_lockObject = new object();

        /// <summary>
        /// Identifies a source of trace logs from this object
        /// </summary>
        protected TraceSource m_traceSource = new TraceSource(SqlServerConstants.PersistenceTraceSourceName);

        /// <summary>
        /// The local configuration for this connector
        /// </summary>
        protected static SqlConfiguration s_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection(SqlServerConstants.ConfigurationSectionName) as SqlConfiguration;

        /// <summary>
        /// The current mapping instance
        /// </summary>
        protected static ModelMapper s_mapper = new ModelMapper(typeof(BaseDataPersistenceService<>).Assembly.GetManifestResourceStream(SqlServerConstants.ModelMapResourceName));
        
        // Primary key map
        private static Dictionary<Type, PropertyInfo> s_primaryKeys = new Dictionary<Type, PropertyInfo>();

        #region IDataPersistence<T> Members 
        /// <summary>
        /// Fired after inserting an entity
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<TModel>> Inserted;
        /// <summary>
        /// Fired prior to inserting an entity
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<TModel>> Inserting;
        /// <summary>
        /// Fired after obsoleting an entity
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<TModel>> Obsoleted;
        /// <summary>
        /// Fired prior to obsoleting an entity
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<TModel>> Obsoleting;
        /// <summary>
        /// Fired when an entity has been queried
        /// </summary>
        public event EventHandler<PostQueryEventArgs<TModel>> Queried;
        /// <summary>
        /// Fired prior to querying an entity
        /// </summary>
        public event EventHandler<PreQueryEventArgs<TModel>> Querying;
        /// <summary>
        /// Fired after an entity has been retrieved
        /// </summary>
        public event EventHandler<PostRetrievalEventArgs<TModel>> Retrieved;
        /// <summary>
        /// Fired prior to retrieving an entity
        /// </summary>
        public event EventHandler<PreRetrievalEventArgs<TModel>> Retrieving;
        /// <summary>
        /// Fired prior to updating an entity
        /// </summary>
        public event EventHandler<PostPersistenceEventArgs<TModel>> Updated;
        /// <summary>
        /// Fired after updating an entity
        /// </summary>
        public event EventHandler<PrePersistenceEventArgs<TModel>> Updating;


       
        /// <summary>
        /// Performs an insert of the data using a new data context
        /// </summary>
        /// <param name="storageData">The data to be inserted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of insertion</param>
        /// <returns>The inserted model object</returns>
        public TModel Insert(TModel storageData, IPrincipal principal, TransactionMode mode)
        {

            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (storageData.Key != Guid.Empty &&
                !s_configuration.AllowKeyedInsert)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            this.ThrowIfInvalid(storageData);

            // Create data context with given transaction
            using (ModelDataContext dataContext = new ModelDataContext(s_configuration.ReadWriteConnectionString))
            {
                this.m_traceSource.TraceInformation("{0}: INSERT {1}", this.GetType().Name, storageData);
                try
                {

                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    dataContext.Connection.Open();
                    dataContext.Transaction = dataContext.Connection.BeginTransaction();

                    PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                    this.Inserting?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return preEvt.Data;

                    preEvt.Data.SetDelayLoad(false);
                    var retVal = this.Insert(preEvt.Data as TModel, principal, dataContext);
                    preEvt.Data.SetDelayLoad(true);

                    PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                    this.Inserted?.Invoke(this, postEvt);
                    dataContext.SubmitChanges();

                    return postEvt.Data;
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                    throw;
                }
                finally
                {
                    if(dataContext.Transaction != null)
                        switch (mode)
                        {
                            case TransactionMode.Rollback:
                                dataContext.Transaction.Rollback();
                                break;
                            case TransactionMode.Commit:
                                dataContext.Transaction.Commit();
                                break;
                        }

                    this.m_traceSource.TraceInformation("{0}: END INSERT {1}", this.GetType().Name, storageData.Key);

                }
            }

        }

        /// <summary>
        /// Throw an exception if the passed instance is invalid
        /// </summary>
        private void ThrowIfInvalid(TModel storageData)
        {
            
            var details = storageData.Validate();
            if (details.Any(d => d.Level == ResultDetailType.Error))
                throw new ModelValidationException("Will not persist invalid model data", details);
        }

        /// <summary>
        /// Update the specified storage data container creating a new version if necessary
        /// </summary>
        /// <param name="storageData">The container to be updated</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of update</param>
        /// <returns>The new version of the object</returns>
        public TModel Update(TModel storageData, IPrincipal principal, TransactionMode mode)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            this.ThrowIfInvalid(storageData);

            using (ModelDataContext dataContext = new ModelDataContext(s_configuration.ReadWriteConnectionString))
            {
                try
                {

                    this.m_traceSource.TraceInformation("{0}: UPDATE {1}", this.GetType().Name, storageData);

                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    dataContext.Connection.Open();
                    dataContext.Transaction = dataContext.Connection.BeginTransaction();

                    // Perform the update
                    PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                    this.Updating?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return preEvt.Data;

                    preEvt.Data.SetDelayLoad(false);
                    var retVal = this.Update(preEvt.Data as TModel, principal, dataContext);
                    preEvt.Data.SetDelayLoad(true);


                    PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                    this.Updated?.Invoke(this, postEvt);

                    dataContext.SubmitChanges();

                    return postEvt.Data;
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                    throw;
                }
                finally
                {
                    if (dataContext.Transaction != null)
                        switch (mode)
                        {
                            case TransactionMode.Rollback:
                                dataContext.Transaction.Rollback();
                                break;
                            case TransactionMode.Commit:
                                dataContext.Transaction.Commit();
                                break;
                        }

                    this.m_traceSource.TraceInformation("{0}: END UPDATE {1}", this.GetType().Name, storageData.Key);

                }
            }
        }

        /// <summary>
        /// Obsoletes the specified storage data
        /// </summary>
        /// <param name="storageData">The storage data object to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of obsoletion (debug = rollback)</param>
        /// <returns>The obsoleted object</returns>
        public TModel Obsolete(TModel storageData, IPrincipal principal, TransactionMode mode)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            else if (storageData.Key == Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.NonIdentityUpdate);
            else if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            this.ThrowIfInvalid(storageData);

            // Create data context with given transaction
            using (ModelDataContext dataContext = new ModelDataContext(s_configuration.ReadWriteConnectionString))
            {
                try
                {

                    this.m_traceSource.TraceInformation("{0}: OBSOLETE {1}", this.GetType().Name, storageData);

                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    dataContext.Connection.Open();
                    dataContext.Transaction = dataContext.Connection.BeginTransaction();

                    PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                    this.Obsoleting?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return preEvt.Data;

                    preEvt.Data.SetDelayLoad(false);
                    var retVal = this.Obsolete(preEvt.Data as TModel, principal, dataContext);
                    preEvt.Data.SetDelayLoad(true);

                    PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                    this.Obsoleted?.Invoke(this, postEvt);

                    dataContext.SubmitChanges();

                    return postEvt.Data;

                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                    throw;
                }
                finally
                {
                    if (dataContext.Transaction != null)
                        switch (mode)
                        {
                            case TransactionMode.Rollback:
                                dataContext.Transaction.Rollback();
                                break;
                            case TransactionMode.Commit:
                                dataContext.Transaction.Commit();
                                break;
                        }

                    this.m_traceSource.TraceInformation("{0}: END OBSOLETE {1}", this.GetType().Name, storageData.Key);

                }
            }

        }

        /// <summary>
        /// Gets the specified container
        /// </summary>
        /// <typeparam name="TIdentifier">The type of identifier which is supported by the Get operation</typeparam>
        /// <param name="containerId">The unique identifier for the container</param>
        /// <param name="principal">The authorization context for the current session</param>
        /// <param name="loadFast">True if only the current version should be loaded (i.e. no deep loading)</param>
        /// <returns>The specified container object of <typeparamref name="TIdentifier"/></returns>
        public TModel Get<TIdentifier>(Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast) 
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            using (var dataContext = new ModelDataContext(s_configuration.ReadonlyConnectionString))
            {
                try
                {

                    this.m_traceSource.TraceInformation("{0}: GET {1}", this.GetType().Name, containerId);
                    dataContext.DeferredLoadingEnabled = !loadFast;
                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    var preEvtModel = new TModel() { Key = (Guid)(Object)containerId.Id };
                    if (preEvtModel is IVersionedEntity)
                        (preEvtModel as IVersionedEntity).VersionKey = (Guid)(Object)containerId.VersionId;

                    PreRetrievalEventArgs<TModel> preEvt = new PreRetrievalEventArgs<TModel>(preEvtModel, principal);
                    this.Retrieving?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return preEvt.Data;

                    // In cache?
                    var retVal = this.Get(new Identifier<Guid>(preEvtModel.Key, (preEvtModel as IVersionedEntity)?.VersionKey ?? Guid.Empty), principal, loadFast, dataContext);

                    PostRetrievalEventArgs<TModel> postEvt = new PostRetrievalEventArgs<TModel>(retVal, principal);
                    this.Retrieved?.Invoke(this, postEvt);

                    return postEvt.Data;
                }
                catch (Exception e)
                {
                    this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                    throw;
                }
                finally
                {
                    this.m_traceSource.TraceInformation("{0}: END GET {1}", this.GetType().Name, containerId);
                }

            }
        }

        /// <summary>
        /// Query with no limiting
        /// </summary>
        public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, IPrincipal principal)
        {
            int count = 0;
            return this.Query(query, 0, null, principal, out count);
        }

        /// <summary>
        /// Count the number of results
        /// </summary>
        public int Count(Expression<Func<TModel, bool>> query, IPrincipal principal)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            try
            {

                using (var dataContext = new ModelDataContext(s_configuration.ReadonlyConnectionString))
                {
                    this.m_traceSource.TraceInformation("{0}: COUNT {1}", this.GetType().Name, query);

                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    PreQueryEventArgs<TModel> preEvt = new PreQueryEventArgs<TModel>(query, principal);
                    this.Querying?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return 0;

                    // TODO: Keep an eye for this one.. not sure what it will...
                    return this.Query(preEvt.Query, principal, dataContext).Count();

                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                throw;
            }
            finally
            {
                this.m_traceSource.TraceInformation("{0}: END COUNT {1}", this.GetType().Name, query);
            }

        }

        /// <summary>
        /// Queries the database for an object of type <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="query">The query to be executed expressed as an expression tree in using the model view classes</param>
        /// <param name="principal">The authorization context</param>
        /// <returns>A delay load IQueryable instance which converts the query objects to the model view class</returns>
        /// <param name="offset">The offset in the result set to start returning</param>
        /// <param name="count">The number of objects to include in ths result set</param>
        public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, int offset, int? count, IPrincipal principal, out Int32 totalCount)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            try
            {
                using (var dataContext = new ModelDataContext(s_configuration.ReadonlyConnectionString))
                {
                    dataContext.DeferredLoadingEnabled = true;
                    totalCount = 0;
                    this.m_traceSource.TraceInformation("{0}: QUERY {1}", this.GetType().Name, query);

                    if (s_configuration.TraceSql)
                        dataContext.Log = new LinqTraceWriter();

                    PreQueryEventArgs<TModel> preEvt = new PreQueryEventArgs<TModel>(query, principal);
                    this.Querying?.Invoke(this, preEvt);
                    if (preEvt.Cancel)
                        return null;

                    // TODO: Keep an eye for this one.. not sure what it will perform like...
                    // Alternateive is to skip/take before filtering results
                    IQueryable<TModel> retVal = this.Query(preEvt.Query, principal, dataContext);
                    
                    PostQueryEventArgs<TModel> postEvt = new PostQueryEventArgs<TModel>(preEvt.Query, retVal, principal);
                    this.Queried?.Invoke(this, postEvt);

                    // If we're going to limit, get the total count now
                    if(count.HasValue)
                        totalCount = retVal.Count();

                    // Offset and count?
                    if(offset > 0)
                        retVal = postEvt.Results.Skip(offset);
                    if (count.HasValue)
                        retVal = retVal.Take(count.Value);

                    
                    return retVal.AsParallel().ToList();
                }
            }
            catch (Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, e.ToString());
                throw;
            }
            finally
            {
                this.m_traceSource.TraceInformation("{0}: END QUERY {1}", this.GetType().Name, query);
            }

        }

        #endregion

        #region Internal Implementation 


        /// <summary>
        /// Gets a cache item
        /// </summary>
        internal TModel ConvertItem<TDomainClass>(TDomainClass domainItem) where TDomainClass : class, new()
        {
            // Get PK
            PropertyInfo pkProperty = null;
            if (!s_primaryKeys.TryGetValue(typeof(TDomainClass), out pkProperty))
            {
                pkProperty = typeof(TDomainClass).GetProperties().SingleOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.IsPrimaryKey == true);
                lock (this.m_lockObject)
                    if (!s_primaryKeys.ContainsKey(typeof(TDomainClass)))
                        s_primaryKeys.Add(typeof(TDomainClass), pkProperty);
            }

            TModel existingItem = default(TModel);

            if (pkProperty != null)
                existingItem = DataCache.Current.GetOrAdd<TModel, TDomainClass>((Guid)pkProperty.GetValue(domainItem), domainItem);
            else
                existingItem = s_mapper.MapDomainInstance<TDomainClass, TModel>(domainItem);
            return existingItem;
        }
        
        /// <summary>
        /// Convert a data type into the model class
        /// </summary>
        /// <param name="data">The data instance to be converted</param>
        /// <returns>The converted data</returns>
        internal abstract TModel ConvertToModel(object data);

        /// <summary>
        /// Convert a model class into a data representation
        /// </summary>
        /// <param name="model">The model to be converted</param>
        /// <returns>The converted model class</returns>
        internal abstract Object ConvertFromModel(TModel model);

        /// <summary>
        /// Must be implemented by all derivative classes, performs the actual get operation
        /// </summary>
        /// <param name="containerId">The container identifier to retrieve</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="dataContext">The context from which data should be loaded</param>
        /// <param name="loadFast">When true, deep loading should occur</param>
        internal abstract TModel Get(Identifier<Guid> containerId, IPrincipal principal, bool loadFast, ModelDataContext dataContext);

        /// <summary>
        /// Do the query for the object
        /// </summary>
        /// <param name="query">The lambda expression representing the query</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="dataContext">The context from which data should be loaded</param>
        /// <returns>An IQueryable which represents the TModel</returns>
        internal abstract IQueryable<TModel> Query(Expression<Func<TModel, bool>> query, IPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the insert of the object
        /// </summary>
        /// <param name="storageData">The object data to be inserted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="dataContext">The context from which data should be loaded</param>
        /// <returns>The inserted model object</returns>
        internal abstract TModel Insert(TModel storageData, IPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the obsoletion of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="dataContext">The context from which data should be loaded</param>
        /// <returns>The new version (obsoleted) of the model</returns>
        internal abstract TModel Obsolete(TModel storageData, IPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the update of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="dataContext">The context from which data should be loaded</param>
        /// <returns>The new version of the model object</returns>
        internal abstract TModel Update(TModel storageData, IPrincipal principal, ModelDataContext dataContext);

        #endregion

        
    }
}
