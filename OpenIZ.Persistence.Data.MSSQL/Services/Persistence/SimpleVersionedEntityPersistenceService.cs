using System.Security.Principal;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Persistence.Data.MSSQL.Data;
using MARC.HI.EHRS.SVC.Core.Data;
using System;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a simple versioned entity persistence service
    /// </summary>
    public class SimpleVersionedEntityPersistenceService<TModel, TData> : IdentifiedPersistenceService<TModel, TData>
        where TModel : BaseEntityData, IVersionedEntity, new()
        where TData : class, IDbIdentified, new()
    {

        /// <summary>
        /// Get the specified object
        /// </summary>
        internal override TModel Get(ModelDataContext context, Guid key, IPrincipal principal)
        {
            return this.Query(context, o => o.Key == key && o.ObsoletionTime == null, principal)?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the specified object taking version of the entity into consideration
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