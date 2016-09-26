using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Services;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service which is derived from an act
    /// </summary>
    public class ActDerivedPersistenceService<TModel, TData> : IdentifiedPersistenceService<TModel, TData>
        where TModel : Core.Model.Acts.Act, new()
        where TData : class, IDbIdentified, new()
    {
        // act persister
        protected ActPersistenceService m_actPersister = new ActPersistenceService();

        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        public override TModel Insert(ModelDataContext context, TModel data, IPrincipal principal)
        {
            var inserted = this.m_actPersister.Insert(context, data, principal);
            data.Key = inserted.Key;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(ModelDataContext context, TModel data, IPrincipal principal)
        {
            this.m_actPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(ModelDataContext context, TModel data, IPrincipal principal)
        {
            var retVal = this.m_actPersister.Obsolete(context, data, principal);
            return data;
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
                if (cacheItem != null && (cacheItem.VersionKey.HasValue && uuid.VersionId == cacheItem.VersionKey.Value || uuid.VersionId == Guid.Empty))
                    return cacheItem;
            }

            // Get most recent version
            if (uuid.VersionId == Guid.Empty)
                return base.Query(o => o.Key == uuid.Id && o.ObsoletionTime == null, 0, 1, principal, out tr).FirstOrDefault();
            else
                return base.Query(o => o.Key == uuid.Id && o.VersionKey == uuid.VersionId, 0, 1, principal, out tr).FirstOrDefault();
        }
    }
}
