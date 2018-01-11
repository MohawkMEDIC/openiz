/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;
using System.Data.Linq;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using System;
using OpenIZ.Core.Services;
using System.Linq;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entity derived persistence services
    /// </summary>
    public class EntityDerivedPersistenceService<TModel, TData> : SimpleVersionedEntityPersistenceService<TModel, TData>
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
            if (typeof(TModel).BaseType == typeof(Core.Model.Entities.Entity))
            {
                var inserted = this.m_entityPersister.Insert(context, data, principal);
                data.Key = inserted.Key;
                data.VersionKey = inserted.VersionKey;
            }
            return base.Insert(context, data, principal);

        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(ModelDataContext context, TModel data, IPrincipal principal)
        {
            if(typeof(TModel).BaseType == typeof(Core.Model.Entities.Entity))
                this.m_entityPersister.Update(context, data, principal);
            return base.Insert(context, data, principal);
            //return base.Update(context, data, principal);
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

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(ModelDataContext context, TModel data, IPrincipal principal)
        {
            if (typeof(TModel).BaseType == typeof(Core.Model.Entities.Entity))
                this.m_entityPersister.Obsolete(context, data, principal);
            return data;
        }

        /// <summary>
        /// Get data load options
        /// </summary>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var retVal = this.m_entityPersister.GetDataLoadOptions();
            return retVal;
        }

    }
}