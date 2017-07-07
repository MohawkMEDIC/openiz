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
 * Date: 2016-11-30
 */
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