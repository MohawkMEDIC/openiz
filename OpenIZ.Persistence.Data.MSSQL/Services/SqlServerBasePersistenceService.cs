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
 * Date: 2016-8-2
 */
using System;
using System.Linq;
using OpenIZ.Core.Model.Map;
using System.Reflection;
using OpenIZ.Core.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Query;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Diagnostics;
using OpenIZ.Persistence.Data.MSSQL.Configuration;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Event;
using System.Security.Principal;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Data.SqlClient;
using OpenIZ.Persistence.Data.MSSQL.Exceptions;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;
using System.Data.Linq;
using System.Data.Common;
using OpenIZ.Core.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Represents a data persistence service which stores data in the local SQLite data store
    /// </summary>
    public abstract class SqlServerBasePersistenceService<TData> : IDataPersistenceService<TData>, IDataPersistenceService where TData : IdentifiedData
    {

        // Lock for editing 
        protected object m_synkLock = new object();

        // Get tracer
        protected TraceSource m_tracer = new TraceSource("OpenIZ.Persistence.Data.MSSQL.Services.Persistence");

        // Configuration
        protected static SqlConfiguration m_configuration = ApplicationContext.Current.GetService<IConfigurationManager>().GetSection("openiz.persistence.data.mssql") as SqlConfiguration;

        // Mapper
        protected static ModelMapper m_mapper = SqlServerPersistenceService.GetMapper();

        public event EventHandler<PrePersistenceEventArgs<TData>> Inserting;
        public event EventHandler<PostPersistenceEventArgs<TData>> Inserted;
        public event EventHandler<PrePersistenceEventArgs<TData>> Updating;
        public event EventHandler<PostPersistenceEventArgs<TData>> Updated;
        public event EventHandler<PrePersistenceEventArgs<TData>> Obsoleting;
        public event EventHandler<PostPersistenceEventArgs<TData>> Obsoleted;
        public event EventHandler<PreRetrievalEventArgs> Retrieving;
        public event EventHandler<PostRetrievalEventArgs<TData>> Retrieved;
        public event EventHandler<PreQueryEventArgs<TData>> Querying;
        public event EventHandler<PostQueryEventArgs<TData>> Queried;

		/// <summary>
		/// Maps the data to a model instance
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="dataInstance">Data instance.</param>
		public abstract TData ToModelInstance(Object dataInstance, ModelDataContext context, IPrincipal principal);

		/// <summary>
		/// Froms the model instance.
		/// </summary>
		/// <returns>The model instance.</returns>
		/// <param name="modelInstance">Model instance.</param>
		public abstract Object FromModelInstance (TData modelInstance, ModelDataContext context, IPrincipal principal);

		/// <summary>
		/// Performthe actual insert.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="data">Data.</param>
		public abstract TData Insert(ModelDataContext context, TData data, IPrincipal principal);

        /// <summary>
        /// Perform the actual update.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public abstract TData Update(ModelDataContext context, TData data, IPrincipal principal);

        /// <summary>
        /// Performs the actual obsoletion
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="data">Data.</param>
        public abstract TData Obsolete(ModelDataContext context, TData data, IPrincipal principal);

        /// <summary>
        /// Performs the actual query
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="query">Query.</param>
        public abstract IQueryable<TData> Query(ModelDataContext context, Expression<Func<TData, bool>> query, IPrincipal principal);

        /// <summary>
        /// Get data load options
        /// </summary>
        internal virtual DataLoadOptions GetDataLoadOptions()
        {
            return new DataLoadOptions();
        }

        /// <summary>
        /// Get the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        internal virtual TData Get(ModelDataContext context, Guid key, IPrincipal principal)
        {
            return this.Query(context, o => o.Key == key, principal)?.FirstOrDefault();
        }

        /// <summary>
        /// Inserts the specified data
        /// </summary>
        public TData Insert(TData data, IPrincipal principal, TransactionMode mode)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
           
            PrePersistenceEventArgs<TData> preArgs = new PrePersistenceEventArgs<TData>(data, principal);
            this.Inserting?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort insert for {0}", data);
                return data;
            }

            // Persist object
            using (var connection = new ModelDataContext(m_configuration.ReadWriteConnectionString))
            {
                DbTransaction tx = null;
                try
                {
                    connection.Connection.Open();
                    connection.Transaction = tx = connection.Connection.BeginTransaction();

                    if (m_configuration.TraceSql)
                        connection.Log = new LinqTraceWriter();

                    // Disable inserting duplicate classified objects
                    var existing = data.TryGetExisting(connection, principal);
                    if(existing != null)
                    {
                        if (m_configuration.AutoUpdateExisting)
                        {
                            this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "INSERT WOULD RESULT IN DUPLICATE CLASSIFIER: UPDATING INSTEAD {0}", data);
                            data = this.Update(connection, data, principal);
                        }
                        else
                            throw new DuplicateKeyException(data);
                    }
                    else
                    {
                        this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "INSERT {0}", data);
                        data = this.Insert(connection, data, principal);
                    }

                    connection.SubmitChanges();


                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

					var args = new PostPersistenceEventArgs<TData>(data, principal)
					{
						Mode = mode
					};

                    this.Inserted?.Invoke(this, args);

                    return data;

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new DataPersistenceException(e.Message, e);
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        /// <param name="storageData"></param>
        /// <param name="principal"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public TData Update(TData data, IPrincipal principal, TransactionMode mode)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (data.Key == Guid.Empty)
                throw new InvalidOperationException("Data missing key");

            PrePersistenceEventArgs<TData> preArgs = new PrePersistenceEventArgs<TData>(data, principal);
            this.Updating?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort update for {0}", data);
                return data;
            }

            // Persist object
            using (var connection = new ModelDataContext(m_configuration.ReadWriteConnectionString))
            {
                DbTransaction tx = null;
                try
                {
                    connection.Connection.Open();
                    connection.Transaction = tx = connection.Connection.BeginTransaction();

                    // Tracer
                    if (m_configuration.TraceSql)
                        connection.Log = new LinqTraceWriter();

                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "UPDATE {0}", data);

                    data = this.Update(connection, data, principal);
                    connection.SubmitChanges();


                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

					var args = new PostPersistenceEventArgs<TData>(data, principal)
					{
						Mode = mode
					};

					this.Updated?.Invoke(this, args);

                    return data;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new DataPersistenceException(e.Message, e);

                }
                finally
                {
                }

            }
        }

        /// <summary>
        /// Obsoletes the specified object
        /// </summary>
        public TData Obsolete(TData data, IPrincipal principal, TransactionMode mode)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (data.Key == Guid.Empty)
                throw new InvalidOperationException("Data missing key");

            PrePersistenceEventArgs<TData> preArgs = new PrePersistenceEventArgs<TData>(data);
            this.Obsoleting?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort for {0}", data);
                return data;
            }

            // Obsolete object
            using (var connection = new ModelDataContext(m_configuration.ReadWriteConnectionString))
            {
                DbTransaction tx = null;
                try
                {
                    connection.Connection.Open();

                    tx = connection.Transaction = connection.Connection.BeginTransaction();

                    // Tracer
                    if (m_configuration.TraceSql)
                        connection.Log = new LinqTraceWriter();

                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "OBSOLETE {0}", data);

                    data = this.Obsolete(connection, data, principal);
                    connection.SubmitChanges();


                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

                    var args = new PostPersistenceEventArgs<TData>(data, principal)
                    {
                        Mode = mode
                    };

                    this.Obsoleted?.Invoke(this, args);

                    return data;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new DataPersistenceException(e.Message, e);
                }
                finally
                {
                }

            }
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public virtual TData Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            // Try the cache if available
            var guidIdentifier = containerId as Identifier<Guid>;
            var cacheItem = ApplicationContext.Current.GetService<IDataCachingService>()?.GetCacheItem<TData>(guidIdentifier.Id) as TData;
	        if (loadFast && cacheItem != null)
	        {
				return cacheItem;
			}
            else
            {
                var tr = 0;
                return this.Query(o => o.Key == guidIdentifier.Id, 0, 1, principal, out tr)?.SingleOrDefault();
            }
        }

        /// <summary>
        /// Performs the specified query
        /// </summary>
        public int Count(Expression<Func<TData, bool>> query, IPrincipal authContext)
        {
            var tr = 0;
            this.Query(query, 0, null, authContext, out tr);
            return tr;
        }

        /// <summary>
        /// Performs query returning all results
        /// </summary>
        public virtual IEnumerable<TData> Query(Expression<Func<TData, bool>> query, IPrincipal authContext)
        {
            var tr = 0;
            return this.QueryInternal(query, 0, null, authContext, out tr, true);

        }

        /// <summary>
        /// Performs the specified query
        /// </summary>
        public virtual IEnumerable<TData> Query(Expression<Func<TData, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
        {
            return this.QueryInternal(query, offset, count, authContext, out totalCount, false);
        }

        /// <summary>
        /// Instructs the service 
        /// </summary>
        protected virtual IEnumerable<TData> QueryInternal(Expression<Func<TData, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount, bool fastQuery)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            PreQueryEventArgs<TData> preArgs = new PreQueryEventArgs<TData>(query, authContext);
            this.Querying?.Invoke(this, preArgs);
            if (preArgs.Cancel)
            {
                this.m_tracer.TraceEvent(TraceEventType.Warning, 0, "Pre-Event handler indicates abort query {0}", query);
                totalCount = 0;
                return null;
            }

            // Query object
            using (var connection = new ModelDataContext(m_configuration.ReadonlyConnectionString))
                try
                {
                    connection.LoadOptions = this.GetDataLoadOptions();

                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "QUERY {0}", query);

                    // Tracer
                    if (m_configuration.TraceSql)
                        connection.Log = new LinqTraceWriter();


                    var results = this.Query(connection, query, authContext);
                    var postData = new PostQueryEventArgs<TData>(query, results, authContext);
                    this.Queried?.Invoke(this, postData);

                    if (count == 1 && offset == 0)
                    {
                        var result = postData.Results.Take(1).ToList();
                        totalCount = result.Count;
                        this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Returning {0}..{1} or {2} results", offset, offset + (count ?? 1000), totalCount);

                        return result;
                    }
                    else
                    {
                        if (!fastQuery)
                            totalCount = postData.Results.Count();
                        else
                            totalCount = -1;
                        // Skip
                        postData.Results = postData.Results.Skip(offset);
                        if (count.HasValue)
                            postData.Results = postData.Results.Take(count.Value);

                        var retVal = postData.Results.AsParallel().ToList();
                        this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Returning {0}..{1} or {2} results", offset, offset + (count ?? 1000), totalCount);

                        return retVal;
                    }

                }
                catch (NotSupportedException e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Cannot perform LINQ query, switching to stored query sqp_{0}", typeof(TData).Name);
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
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Query {0} took {1} ms", query, sw.ElapsedMilliseconds);
#endif
                }
        }

        /// <summary>
        /// Insert
        /// </summary>
        public object Insert(object data)
        {
            return this.Insert((TData)data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Update
        /// </summary>
        public object Update(object data)
        {
            return this.Update((TData)data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        public object Obsolete(object data)
        {
            return this.Obsolete((TData)data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Get
        /// </summary>
        public object Get(Guid id)
        {
            return this.Get(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Query
        /// </summary>
        public IEnumerable Query(Expression query, int offset, int? count, out int totalResults)
        {
            return this.Query((Expression<Func<TData, bool>>)query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
        }
    }
}

