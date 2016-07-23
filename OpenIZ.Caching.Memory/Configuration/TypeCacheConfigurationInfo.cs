/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-19
 */
using System;
using System.Collections.Generic;

namespace OpenIZ.Caching.Memory.Configuration
{
    /// <summary>
    /// Represents type cache configuration
    /// </summary>
    public class TypeCacheConfigurationInfo
    {

        /// <summary>
        /// Type cache configuration
        /// </summary>
        public TypeCacheConfigurationInfo()
        {
            this.SeedQueries = new List<String>();
        }

        /// <summary>
        /// Gets or sets the type of cache entry
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the items in the cache
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum age of items in the cache
        /// </summary>
        public long MaxCacheAge { get; set; }

        /// <summary>
        /// Gets or sets the seed query data
        /// </summary>
        internal List<String> SeedQueries { get; set; }

        /// <summary>
        /// Gets or sets the value of type
        /// </summary>
        public string TypeXml {
            get { return this.Type?.AssemblyQualifiedName; }
            set
            {
                if (value == null)
                    this.Type = null;
                else
                    this.Type = Type.GetType(value);
            }
        }
    }
}