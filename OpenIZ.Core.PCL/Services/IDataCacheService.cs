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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model;
using System;

namespace OpenIZ.Core.Services
{

    /// <summary>
    /// Data cache event args
    /// </summary>
    public class DataCacheEventArgs : EventArgs
    {
        /// <summary>
        /// The object added or removed from the cache
        /// </summary>
        public Object Object { get; private set; }

        /// <summary>
        /// Data cache event args ctor
        /// </summary>
        public DataCacheEventArgs(Object obj)
        {
            this.Object = obj;
        }
    }

	/// <summary>
	/// Represents a data caching service which allows services to retrieve
	/// cached objects
	/// </summary>
	public interface IDataCachingService
	{
        /// <summary>
        /// Item was added to cache
        /// </summary>
        event EventHandler<DataCacheEventArgs> Added;
        /// <summary>
        /// Item was updated from cache
        /// </summary>
        event EventHandler<DataCacheEventArgs> Updated;
        /// <summary>
        /// Item was removed from cache
        /// </summary>
        event EventHandler<DataCacheEventArgs> Removed;

		/// <summary>
		/// Get the specified cache item
		/// </summary>
		TData GetCacheItem<TData>(Guid key) where TData : IdentifiedData;

		/// <summary>
		/// Gets the specified cache item
		/// </summary>
		object GetCacheItem(Guid key);

		/// <summary>
		/// Adds the specified item to the cache
		/// </summary>
		void Add(IdentifiedData data);

		/// <summary>
		/// Removes an object from the cache
		/// </summary>
		void Remove(Guid key);
	}
}