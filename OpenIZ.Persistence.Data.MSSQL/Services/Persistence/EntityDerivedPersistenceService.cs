using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entity derived persistence services
    /// </summary>
    public class EntityDerivedPersistenceService<TModel, TData> : IdentifiedPersistenceService<TModel, TData>
        where TModel : Core.Model.Entities.Entity, new()
        where TData : class, IDbIdentified, new()
    {

        // Entity persister
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();
        
        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        public override TModel Insert(ModelDataContext context, TModel data, IPrincipal principal)
        {
            var inserted = this.m_entityPersister.Insert(context, data, principal);
            data.Key = inserted.Key;
            data.VersionKey = inserted.VersionKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(ModelDataContext context, TModel data, IPrincipal principal)
        {
            this.m_entityPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(ModelDataContext context, TModel data, IPrincipal principal)
        {
            var retVal = this.m_entityPersister.Obsolete(context, data, principal);
            return data;
        }

    }
}