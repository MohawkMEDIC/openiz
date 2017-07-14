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
 * Date: 2017-6-25
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using OpenIZ.OrmLite;
using System;
using System.Linq;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a temporary cache
    /// </summary>
    internal class AdoPersistenceCache
    {
        // Context
        private DataContext m_context;

        // Cache
        private IDataCachingService m_cache = ApplicationContext.Current.GetService<IDataCachingService>();

        /// <summary>
        /// Create a new persistence cache
        /// </summary>
        public AdoPersistenceCache(DataContext context)
        {
            this.m_context = context;
        }

        /// <summary>
        /// Add to cache
        /// </summary>
        public void Add(IdentifiedData data)
        {
            if(data != null)
                this.m_context.AddCacheCommit(data);
        }

        /// <summary>
        /// Get cache item
        /// </summary>
        public TReturn GetCacheItem<TReturn>(Guid key)
        {
            return (TReturn)(this.m_context.GetCacheCommit(key) ??
                this.m_cache?.GetCacheItem(key));
        }

        /// <summary>
        /// Get cache item
        /// </summary>
        public IdentifiedData GetCacheItem(Guid key)
        {
            return (this.m_context.GetCacheCommit(key) ??
                this.m_cache?.GetCacheItem(key)) as IdentifiedData;
        }
    }
}