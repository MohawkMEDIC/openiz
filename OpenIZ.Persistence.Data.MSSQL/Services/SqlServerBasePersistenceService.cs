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

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Represents a data persistence service which stores data in the local SQLite data store
    /// </summary>
    public abstract class SqlServerBasePersistenceService<TData> : IDataPersistenceService<TData> where TData : IdentifiedData
    {

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
        public event EventHandler<PreRetrievalEventArgs<TData>> Retrieving;
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
        protected virtual DataLoadOptions GetDataLoadOptions()
        {
            return new DataLoadOptions();
        }

        /// <summary>
        /// Get the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        internal TData Get(ModelDataContext context, Guid key, IPrincipal principal)
        {
            return this.Query(context, o => o.Key == key, principal)?.SingleOrDefault();
        }

        /// <summary>
        /// Inserts the specified data
        /// </summary>
        public TData Insert(TData data, IPrincipal principal, TransactionMode mode)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            else if (!m_configuration.AllowKeyedInsert &&
                data.Key != Guid.Empty)
                throw new SqlFormalConstraintException(SqlFormalConstraintType.IdentityInsert);

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

                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "INSERT {0}", data);

                    data.SetDelayLoad(false);
                    data = this.Insert(connection, data, principal);
                    connection.SubmitChanges();

                    data.SetDelayLoad(true);

                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();
                    this.Inserted?.Invoke(this, new PostPersistenceEventArgs<TData>(data, principal));

                    return data;

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new Exception(e.Message, e);
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

                    data.SetDelayLoad(false);
                    data = this.Update(connection, data, principal);
                    connection.SubmitChanges();

                    data.SetDelayLoad(true);

                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

                    this.Updated?.Invoke(this, new PostPersistenceEventArgs<TData>(data, principal));

                    return data;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new Exception(e.Message, e);

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

                    data.SetDelayLoad(false);
                    data = this.Obsolete(connection, data, principal);
                    connection.SubmitChanges();

                    data.SetDelayLoad(true);

                    if (mode == TransactionMode.Commit)
                        tx.Commit();
                    else
                        tx.Rollback();

                    this.Obsoleted?.Invoke(this, new PostPersistenceEventArgs<TData>(data));

                    return data;
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    tx?.Rollback();
                    throw new Exception(e.Message, e);
                }
            }
        }

        /// <summary>
        /// Gets the specified object
        /// </summary>
        public virtual TData Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, IPrincipal principal, bool loadFast)
        {
            var tr = 0;
            var guidIdentifier = containerId as Identifier<Guid>;
            return this.Query(o => o.Key == guidIdentifier.Id, 0, 1, principal, out tr)?.SingleOrDefault();
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
            return this.Query(query, 0, null, authContext, out tr);
        }

        /// <summary>
        /// Performs the specified query
        /// </summary>
        public virtual IEnumerable<TData> Query(Expression<Func<TData, bool>> query, int offset, int? count, IPrincipal authContext, out int totalCount)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

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

                    totalCount = postData.Results.Count();

                    // Skip
                    postData.Results = postData.Results.Skip(offset);
                    if (count.HasValue)
                        postData.Results = postData.Results.Take(count.Value);

                    return postData.Results.AsParallel().ToList();

                }
                catch (NotSupportedException e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Cannot perform LINQ query, switching to stored query sqp_{0}", typeof(TData).Name);
                    throw new InvalidOperationException("Cannot perform LINQ query", e);
                }
                catch (Exception e)
                {
                    this.m_tracer.TraceEvent(TraceEventType.Error, 0, "Error : {0}", e);
                    throw;
                }

        }
    }
}

