using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Authorization;
using MARC.HI.EHRS.SVC.Core.Event;
using OpenIZ.Core.Model;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security;
using System.Linq.Expressions;

namespace OpenIZ.Persistence.Data.MSSQL.Services
{
    /// <summary>
    /// Represents a basic persistence service
    /// </summary>
    /// <typeparam name="TModel">The OpenIZ Model type</typeparam>
    public abstract class BaseDataPersistenceService<TModel> : IDataPersistenceService<TModel> where TModel : BaseData, new()
    {

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
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of insertion</param>
        /// <returns>The inserted model object</returns>
        public TModel Insert(TModel storageData, AuthorizationContext authContext, DataPersistenceMode mode)
        {
            using (ModelDataContext dataContext = new ModelDataContext())
            {
                var retVal = this.Insert(storageData, authContext, dataContext);
                if (mode == DataPersistenceMode.Production)
                    dataContext.SubmitChanges();
                return retVal;
            }
        }

        /// <summary>
        /// Update the specified storage data container creating a new version if necessary
        /// </summary>
        /// <param name="storageData">The container to be updated</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of update</param>
        /// <returns>The new version of the object</returns>
        public TModel Update(TModel storageData, AuthorizationContext authContext, DataPersistenceMode mode)
        {
            using (ModelDataContext dataContext = new ModelDataContext())
            {
                var retVal = this.Update(storageData, authContext, dataContext);
                if (mode == DataPersistenceMode.Production)
                    dataContext.SubmitChanges();
                return retVal;
            }
        }

        /// <summary>
        /// Obsoletes the specified storage data
        /// </summary>
        /// <param name="storageData">The storage data object to be obsoleted</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of obsoletion (debug = rollback)</param>
        /// <returns>The obsoleted object</returns>
        public TModel Obsolete(TModel storageData, AuthorizationContext authContext, DataPersistenceMode mode)
        {
            using (ModelDataContext dataContext = new ModelDataContext())
            {
                var retVal = this.Obsolete(storageData, authContext, dataContext);
                if (mode == DataPersistenceMode.Production)
                    dataContext.SubmitChanges();
                return retVal;
            }
        }

        /// <summary>
        /// Gets the specified container
        /// </summary>
        /// <typeparam name="TIdentifier">The type of identifier which is supported by the Get operation</typeparam>
        /// <param name="containerId">The unique identifier for the container</param>
        /// <param name="authContext">The authorization context for the current session</param>
        /// <param name="loadFast">True if only the current version should be loaded (i.e. no deep loading)</param>
        /// <returns>The specified container object of <typeparamref name="T"/></returns>
        public TModel Get<TIdentifier>(Identifier<TIdentifier> containerId, AuthorizationContext authContext, bool loadFast)
        {
            using (ModelDataContext dataContext = new ModelDataContext())
            {
                return this.Get(containerId, authContext, loadFast, dataContext);
            }
        }

        /// <summary>
        /// Queries the database for an object of type <paramref name="T"/>
        /// </summary>
        /// <param name="query">The query to be executed expressed as an expression tree in using the model view classes</param>
        /// <param name="authContext">The authorization context</param>
        /// <returns>A delay load IQueryable instance which converts the query objects to the model view class</returns>
        public IQueryable<TModel> Query(Expression<Func<TModel, bool>> query, AuthorizationContext authContext)
        {
            using (ModelDataContext dataContext = new ModelDataContext())
            {
                return this.Query(query, authContext, dataContext);
            }
        }
        
        #endregion

        #region Internal Implementation 
        /// <summary>
        /// Gets the specified container
        /// </summary>
        /// <typeparam name="TIdentifier">The type of identifier which is supported by the Get operation</typeparam>
        /// <param name="containerId">The unique identifier for the container</param>
        /// <param name="authContext">The authorization context for the current session</param>
        /// <param name="loadFast">True if only the current version should be loaded (i.e. no deep loading)</param>
        /// <returns>The specified container object of <typeparamref name="T"/></returns>
        public TModel Get<TIdentifier>(MARC.HI.EHRS.SVC.Core.Data.Identifier<TIdentifier> containerId, AuthorizationContext authContext, bool loadFast, ModelDataContext dataContext) 
        {
            PreRetrievalEventArgs<TModel> preEvt = new PreRetrievalEventArgs<TModel>(new TModel() { Id = containerId as Identifier<Guid> }, authContext);
            this.Retrieving?.Invoke(this, preEvt);
            if (preEvt.Cancel)
                return preEvt.Data;

            TModel retVal = this.DoGet(preEvt.Data.Id, authContext, loadFast, dataContext);

            PostRetrievalEventArgs<TModel> postEvt = new PostRetrievalEventArgs<TModel>(retVal, authContext);
            this.Retrieved?.Invoke(this, postEvt);

            return retVal;
        }

        /// <summary>
        /// Inserts the specified storage data into the database
        /// </summary>
        /// <param name="storageData">The data to be stored</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of storage, if debug then the transaction is only tested</param>
        /// <returns>The instance of <paramref name="storagedata"/> with data elements persisted</returns>
        public TModel Insert(TModel storageData, AuthorizationContext authContext, ModelDataContext dataContext)
        {
            PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, authContext);
            this.Inserting?.Invoke(this, preEvt);
            if (preEvt.Cancel)
                return preEvt.Data;

            TModel retVal = this.DoInsert(preEvt.Data, authContext, dataContext);

            PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, authContext);
            this.Inserted?.Invoke(this, postEvt);

            return retVal;
        }

        /// <summary>
        /// Obsoletes the specified storage data
        /// </summary>
        /// <param name="storageData">The storage data object to be obsoleted</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of obsoletion (debug = rollback)</param>
        /// <returns>The obsoleted object</returns>
        public TModel Obsolete(TModel storageData, AuthorizationContext authContext, ModelDataContext dataContext)
        {
            PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, authContext);
            this.Obsoleting?.Invoke(this, preEvt);
            if (preEvt.Cancel)
                return preEvt.Data;

            TModel retVal = this.DoObsolete(preEvt.Data, authContext, dataContext);

            PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, authContext);
            this.Obsoleted?.Invoke(this, postEvt);

            return retVal;
        }

        /// <summary>
        /// Queries the database for an object of type <paramref name="T"/>
        /// </summary>
        /// <param name="query">The query to be executed expressed as an expression tree in using the model view classes</param>
        /// <param name="authContext">The authorization context</param>
        /// <returns>A delay load IQueryable instance which converts the query objects to the model view class</returns>
        public IQueryable<TModel> Query(Expression<Func<TModel, bool>> query, AuthorizationContext authContext, ModelDataContext dataContext)
        {
            PreQueryEventArgs<TModel> preEvt = new PreQueryEventArgs<TModel>(query, authContext);
            this.Querying?.Invoke(this, preEvt);
            if (preEvt.Cancel)
                return null;

            IQueryable<TModel> retVal = this.DoQuery(preEvt.Query, authContext, dataContext);

            PostQueryEventArgs<TModel> postEvt = new PostQueryEventArgs<TModel>(preEvt.Query, retVal, authContext);
            this.Queried?.Invoke(this, postEvt);

            return retVal;
        }

        /// <summary>
        /// Update the specified storage data container creating a new version if necessary
        /// </summary>
        /// <param name="storageData">The container to be updated</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of update</param>
        /// <returns>The new version of the object</returns>
        public TModel Update(TModel storageData, AuthorizationContext authContext,ModelDataContext dataContext)
        {
            PrePersistenceEventArgs<TModel> preEvt = new PrePersistenceEventArgs<TModel>(storageData, authContext);
            this.Updating?.Invoke(this, preEvt);
            if (preEvt.Cancel)
                return preEvt.Data;

            TModel retVal = this.DoUpdate(preEvt.Data, authContext, dataContext);

            PostPersistenceEventArgs<TModel> postEvt = new PostPersistenceEventArgs<TModel>(retVal, authContext);
            this.Updated?.Invoke(this, postEvt);

            return retVal;
        }

        /// <summary>
        /// Convert a data type into the model class
        /// </summary>
        /// <typeparam name="TData">The data model type</typeparam>
        /// <param name="data">The data instance to be converted</param>
        /// <returns>The converted data</returns>
        internal abstract TModel Convert<TData>(TData data);

        /// <summary>
        /// Convert a model class into a data representation
        /// </summary>
        /// <typeparam name="TData">The data model representation type</typeparam>
        /// <param name="model">The model to be converted</param>
        /// <returns>The converted model class</returns>
        internal abstract TData Convert<TData>(TModel model);

        /// <summary>
        /// Must be implemented by all derivative classes, performs the actual get operation
        /// </summary>
        /// <param name="containerId">The container identifier to retrieve</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="loadFast">True if loading fast should be enabled</param>
        protected abstract TModel DoGet(Identifier<Guid> containerId, AuthorizationContext authContext, bool loadFast, ModelDataContext dataContext);

        /// <summary>
        /// Do the query for the object
        /// </summary>
        /// <param name="query">The lambda expression representing the query</param>
        /// <param name="authContext">The authorization context</param>
        /// <returns>An IQueryable which represents the TModel</returns>
        protected abstract IQueryable<TModel> DoQuery(Expression<Func<TModel, bool>> query, AuthorizationContext authContext, ModelDataContext dataContext);

        /// <summary>
        /// Perform the insert of the object
        /// </summary>
        /// <param name="storageData">The object data to be inserted</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of insert</param>
        /// <returns>The inserted model object</returns>
        protected abstract TModel DoInsert(TModel storageData, AuthorizationContext authContext, ModelDataContext dataContext);

        /// <summary>
        /// Perform the obsoletion of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of obsoletion</param>
        /// <returns>The new version (obsoleted) of the model</returns>
        protected abstract TModel DoObsolete(TModel storageData, AuthorizationContext authContext, ModelDataContext dataContext);

        /// <summary>
        /// Perform the update of the data
        /// </summary>
        /// <param name="storageData">The data to be obsoleted</param>
        /// <param name="authContext">The authorization context</param>
        /// <param name="mode">The mode of obsoletion</param>
        /// <returns>The new version of the model object</returns>
        protected abstract TModel DoUpdate(TModel storageData, AuthorizationContext authContext, ModelDataContext dataContext);

        #endregion

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="authContext">The current authorization context</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        protected Guid GetUserFromAuthContext(AuthorizationContext authContext, ModelDataContext dataContext)
        {

            var user = dataContext.SecurityUsers.FirstOrDefault(o => o.UserName == authContext.Identity.Name && !o.ObsoletionTime.HasValue);
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user.UserId;

        }

    }
}
