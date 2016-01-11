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

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{

    /// <summary>
    /// Represents a basic persistence service
    /// </summary>
    /// <typeparam name="TModel">The OpenIZ Model type</typeparam>
    [Service(ServiceInstantiationType.Instance)]
    public abstract class BaseDataPersistenceService<TModel> : IDataPersistenceService<TModel> where TModel : BaseData, new()
    {

        // True if the context is disposed
        private bool m_disposed;
        // The current context
        private SqlDataPersistenceContext m_context;

        // Mapper
        protected static ModelMapper s_mapper = new ModelMapper(typeof(BaseDataPersistenceService<>).Assembly.GetManifestResourceStream("OpenIZ.Persistence.Data.MSSQL.Data.ModelMap.xml"));

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
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            if (!this.m_disposed)
                this.m_context?.Dispose();
            this.m_disposed = true;
        }

        /// <summary>
        /// Gets the data context
        /// </summary>
        public IDataPersistenceContext DataContext
        {
            get
            {
                if (this.m_context == null)
                    this.m_context = new SqlDataPersistenceContext();
                return this.m_context;
            }
            set
            {
                if (value is SqlDataPersistenceContext)
                    this.m_context = value as SqlDataPersistenceContext;
                else
                    throw new ArgumentException(nameof(value));
            }
        }

        /// <summary>
        /// Performs an insert of the data using a new data context
        /// </summary>
        /// <param name="storageData">The data to be inserted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of insertion</param>
        /// <returns>The inserted model object</returns>
        public TModel Insert(TModel storageData, ClaimsPrincipal principal, DataPersistenceMode mode)
        {
            this.ThrowIfDisposed();
            try
            {
                this.DataContext.Open();
                this.DataContext.BeginTransaction();

                Trace.TraceInformation("MSSQL: {0}: INSERT {1}", this.GetType().Name, storageData);

                PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                this.Inserting?.Invoke(this, preEvt);
                if (preEvt.Cancel)
                    return preEvt.Data;

                TModel retVal = this.Insert(preEvt.Data.AsFrozen() as TModel, principal, this.m_context.Context);

                PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                this.Inserted?.Invoke(this, postEvt);

                this.m_context?.Context.SubmitChanges();

                return retVal;
            }
            catch(Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
            finally
            {
                if (mode == DataPersistenceMode.Debugging)
                    this.DataContext.Rollback();
                else
                    this.DataContext.Commit();
            }
        }

        /// <summary>
        /// Update the specified storage data container creating a new version if necessary
        /// </summary>
        /// <param name="storageData">The container to be updated</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of update</param>
        /// <returns>The new version of the object</returns>
        public TModel Update(TModel storageData, ClaimsPrincipal principal, DataPersistenceMode mode)
        {
            this.ThrowIfDisposed();
            try
            {
                this.DataContext.Open();
                this.DataContext.BeginTransaction();

                Trace.TraceInformation("MSSQL: {0}: UPDATE {1}", this.GetType().Name, storageData);

                PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                this.Updating?.Invoke(this, preEvt);
                if (preEvt.Cancel)
                    return preEvt.Data;

                TModel retVal = this.Update(preEvt.Data.AsFrozen() as TModel, principal, this.m_context.Context);

                PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                this.Updated?.Invoke(this, postEvt);

                this.m_context?.Context.SubmitChanges();

                return retVal;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
            finally
            {
                if (mode == DataPersistenceMode.Debugging)
                    this.DataContext.Rollback();
                else
                    this.DataContext.Commit();
            }
        }

        /// <summary>
        /// Obsoletes the specified storage data
        /// </summary>
        /// <param name="storageData">The storage data object to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of obsoletion (debug = rollback)</param>
        /// <returns>The obsoleted object</returns>
        public TModel Obsolete(TModel storageData, ClaimsPrincipal principal, DataPersistenceMode mode)
        {
            this.ThrowIfDisposed();
            try
            {
                this.DataContext.Open();
                this.DataContext.BeginTransaction();

                Trace.TraceInformation("MSSQL: {0}: OBSOLETE {1}", this.GetType().Name, storageData);

                PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, principal);
                this.Obsoleting?.Invoke(this, preEvt);
                if (preEvt.Cancel)
                    return preEvt.Data;

                TModel retVal = this.Obsolete(preEvt.Data.AsFrozen() as TModel, principal, this.m_context?.Context);

                PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, principal);
                this.Obsoleted?.Invoke(this, postEvt);

                this.m_context?.Context.SubmitChanges();

                return retVal;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
            finally
            {
                if (mode == DataPersistenceMode.Debugging)
                    this.DataContext.Rollback();
                else
                    this.DataContext.Commit();
            }
        }

        /// <summary>
        /// Gets the specified container
        /// </summary>
        /// <typeparam name="TIdentifier">The type of identifier which is supported by the Get operation</typeparam>
        /// <param name="containerId">The unique identifier for the container</param>
        /// <param name="principal">The authorization context for the current session</param>
        /// <param name="loadFast">True if only the current version should be loaded (i.e. no deep loading)</param>
        /// <returns>The specified container object of <typeparamref name="T"/></returns>
        public TModel Get<TIdentifier>(Identifier<TIdentifier> containerId, ClaimsPrincipal principal, bool loadFast)
        {
            this.ThrowIfDisposed();
            try
            {
                this.DataContext.Open();

                Trace.TraceInformation("MSSQL: {0}: GET {1}", this.GetType().Name, containerId);

                PreRetrievalEventArgs<TModel> preEvt = new PreRetrievalEventArgs<TModel>(new TModel() { Id = containerId as Identifier<Guid> }, principal);
                this.Retrieving?.Invoke(this, preEvt);
                if (preEvt.Cancel)
                    return preEvt.Data;

                TModel retVal = this.Get(preEvt.Data.Id, principal, loadFast, this.m_context?.ReadonlyContext);

                PostRetrievalEventArgs<TModel> postEvt = new PostRetrievalEventArgs<TModel>(retVal, principal);
                this.Retrieved?.Invoke(this, postEvt);

                return retVal;
            }
            catch(Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Queries the database for an object of type <paramref name="T"/>
        /// </summary>
        /// <param name="query">The query to be executed expressed as an expression tree in using the model view classes</param>
        /// <param name="principal">The authorization context</param>
        /// <returns>A delay load IQueryable instance which converts the query objects to the model view class</returns>
        public IEnumerable<TModel> Query(Expression<Func<TModel, bool>> query, ClaimsPrincipal principal)
        {
            this.ThrowIfDisposed();

            try
            {
                this.DataContext.Open();

                Trace.TraceInformation("MSSQL: {0}: QUERY {1}", this.GetType().Name, query);

                PreQueryEventArgs<TModel> preEvt = new PreQueryEventArgs<TModel>(query, principal);
                this.Querying?.Invoke(this, preEvt);
                if (preEvt.Cancel)
                    return null;

                IQueryable<TModel> retVal = this.Query(preEvt.Query, principal, this.m_context?.ReadonlyContext);

                PostQueryEventArgs<TModel> postEvt = new PostQueryEventArgs<TModel>(preEvt.Query, retVal, principal);
                this.Queried?.Invoke(this, postEvt);

                return retVal;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        #endregion

        #region Internal Implementation 

        /// <summary>
        /// Throw an exception if the object is disposed
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.m_disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// Convert a data type into the model class
        /// </summary>
        /// <param name="data">The data instance to be converted</param>
        /// <returns>The converted data</returns>
        internal abstract TModel ConvertToModel(Object data);

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
        /// <param name="loadFast">True if loading fast should be enabled</param>
        internal abstract TModel Get(Identifier<Guid> containerId, ClaimsPrincipal principal, bool loadFast, ModelDataContext dataContext);

        /// <summary>
        /// Do the query for the object
        /// </summary>
        /// <param name="query">The lambda expression representing the query</param>
        /// <param name="principal">The authorization context</param>
        /// <returns>An IQueryable which represents the TModel</returns>
        internal abstract IQueryable<TModel> Query(Expression<Func<TModel, bool>> query, ClaimsPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the insert of the object
        /// </summary>
        /// <param name="storageData">The object data to be inserted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of insert</param>
        /// <returns>The inserted model object</returns>
        internal abstract TModel Insert(TModel storageData, ClaimsPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the obsoletion of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of obsoletion</param>
        /// <returns>The new version (obsoleted) of the model</returns>
        internal abstract TModel Obsolete(TModel storageData, ClaimsPrincipal principal, ModelDataContext dataContext);

        /// <summary>
        /// Perform the update of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="principal">The authorization context</param>
        /// <param name="mode">The mode of obsoletion</param>
        /// <returns>The new version of the model object</returns>
        internal abstract TModel Update(TModel storageData, ClaimsPrincipal principal, ModelDataContext dataContext);

        #endregion

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="principal">The current authorization context</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        protected Guid GetUserFromprincipal(ClaimsPrincipal principal, ModelDataContext dataContext)
        {

            var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == principal.Identity.Name && !o.ObsoletionTime.HasValue);
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user.UserId;

        }
        
    }
}
