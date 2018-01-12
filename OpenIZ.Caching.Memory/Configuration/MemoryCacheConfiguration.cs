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
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Caching.Memory.Configuration
{
    /// <summary>
    /// Create memory cache configuration
    /// </summary>
    public class MemoryCacheConfiguration
    {

        /// <summary>
        /// Memory type configuration
        /// </summary>
        public MemoryCacheConfiguration()
        {
            this.Types = new List<TypeCacheConfigurationInfo>();
        }

        /// <summary>
        /// Autosubscribe types
        /// </summary>
        public bool AutoSubscribeTypes { get; set; }

        /// <summary>
        /// Type cache configuration information 
        /// </summary>
        public List<TypeCacheConfigurationInfo> Types { get; set; }

        /// <summary>
        /// Gets or sets the items in the cache
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum age of items in the cache
        /// </summary>
        public long MaxCacheAge { get; set; }

    }
}
