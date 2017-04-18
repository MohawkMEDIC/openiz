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
 * Date: 2016-11-5
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq.Expressions;

namespace OpenIZ.Core.Model.Collection
{
    /// <summary>
    /// Represents a collection of entities
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model")]
    [JsonObject]
    public class SimpleAssociationCollection<TEntity> : IList<TEntity>, ICollection<TEntity>, IEnumerable<TEntity>, IList, ILockable where TEntity : IdentifiedData, ISimpleAssociation, new()
    {

        // Sync root
        private object m_syncRoot = new object();

        // Source data
        protected IList<TEntity> m_sourceData;

        // Context or where the key can be loaded from
        protected IdentifiedData m_context;

        // Always loading?
        protected bool? m_load = null;

        // Is readonly
        private bool m_isReadonly = false;

        /// <summary>
        /// Entity collection
        /// </summary>
        public SimpleAssociationCollection()
        {
            this.m_sourceData = new List<TEntity>();
        }

        /// <summary>
        /// Create simple assoc
        /// </summary>
        public SimpleAssociationCollection(IEnumerable<TEntity> source)
        {
            this.m_sourceData = new List<TEntity>(source.AsParallel());
        }

        /// <summary>
        /// Creates the specified entity collection in the specified context
        /// </summary>
        public SimpleAssociationCollection(IIdentifiedEntity context)
        {
            this.m_context = context as IdentifiedData;
        }

        /// <summary>
        /// Perform a delay load on the entire collection
        /// </summary>
        protected virtual IList<TEntity> Ensure()
        {
            if (this.m_context == null) return this.m_sourceData;

            // Load if needed
            if (this.m_sourceData == null)
            {
                if (this.m_context.Key.HasValue && (this.m_load.HasValue && this.m_load.Value || !this.m_load.HasValue && this.m_context.IsDelayLoadEnabled))
                    this.m_sourceData = new List<TEntity>(EntitySource.Current.Provider.GetRelations<TEntity>(this.m_context.Key));
                else
                    this.m_sourceData = new List<TEntity>();
            }

            return this.m_sourceData;
        }

        /// <summary>
        /// Forces the associations to be loaded from database regardless of current state
        /// </summary>
        public SimpleAssociationCollection<TEntity> AsAlwaysLoad()
        {
            var retVal = this.MemberwiseClone() as SimpleAssociationCollection<TEntity>;
            retVal.m_load = true;
            return retVal;
        }

        /// <summary>
        /// Forces the associations to be loaded from database regardless of current state
        /// </summary>
        public SimpleAssociationCollection<TEntity> GetLocked()
        {
            var retVal = this.MemberwiseClone() as SimpleAssociationCollection<TEntity>;
            retVal.m_load = false;
            this.m_isReadonly = true;
            return retVal;
        }

        /// <summary>
        /// Forces a refresh of the current associations
        /// </summary>
        public SimpleAssociationCollection<TEntity> Refresh()
        {
            this.m_sourceData = null;
            return this;
        }

        /// <summary>
        /// Index of the specified item
        /// </summary>
        public int IndexOf(TEntity item)
        {
            return this.Ensure().IndexOf(item);
        }

        /// <summary>
        /// Insert an item at the specified index
        /// </summary>
        public void Insert(int index, TEntity item)
        {
            if(!this.IsReadOnly)
                this.Ensure().Insert(index, item);
        }

        /// <summary>
        /// Remove the objects at the specified data element
        /// </summary>
        public void RemoveAt(int index)
        {
            if(!this.IsReadOnly)
                this.Ensure().RemoveAt(index);
        }

        /// <summary>
        /// Adds the speciifed entity
        /// </summary>
        public void Add(TEntity item)
        {
            if(!this.IsReadOnly && item != null)
                this.Ensure().Add(item);
        }

        /// <summary>
        /// Clear the specified object
        /// </summary>
        public void Clear()
        {
            if(!this.IsReadOnly)
                this.Ensure().Clear();
        }

        /// <summary>
        /// Return true if the array contains the list of objects
        /// </summary>
        public bool Contains(TEntity item)
        {
            return this.Ensure().Contains(item);
        }

        /// <summary>
        /// Copy to the specified array
        /// </summary>
        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            this.Ensure().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove the specified item from the array
        /// </summary>
        public bool Remove(TEntity item)
        {
            if(!this.IsReadOnly)
                return this.Ensure().Remove(item);
            return false;
        }

        /// <summary>
        /// Get the specified enumerator
        /// </summary>
        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Ensure().GetEnumerator();

        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Ensure().GetEnumerator();
        }

        /// <summary>
        /// Gets the entity data at the specified source
        /// </summary>
        public TEntity this[int index]
        {
            get
            {
                return this.Ensure()[index];
            }
            set
            {
                this.Ensure()[index] = value;
            }
        }

        /// <summary>
        /// Gets the count of data in the array
        /// </summary>
        public int Count
        {
            get
            {
                return this.Ensure().Count;
            }
        }

        /// <summary>
        /// Returns true if the object is readonly 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.m_isReadonly;
            }
        }

        /// <summary>
        /// True if list is fixed size
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// True if list is synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the synchronization root
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this.m_syncRoot;
            }
        }

        /// <summary>
        /// Get the item as an object
        /// </summary>
        object IList.this[int index]
        {
            get
            {
                return this.Ensure()[index];
            }
            set
            {
                this.Ensure()[index] = value as TEntity;
            }
        }

        /// <summary>
        /// Remove all elements matching the specified object
        /// </summary>
        public void RemoveAll(Func<TEntity, bool> predicate)
        {
            var results = this.Where(predicate).ToArray();
            foreach(var itm in results)
                this.Remove(itm);
        }

        /// <summary>
        /// Add this object 
        /// </summary>
        public int Add(object value)
        {
            if (value is TEntity)
            {
                this.Add((TEntity)value);
                return 1;
            }
            else
                return 0;
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            return this.Contains((TEntity)value);
        }

        /// <summary>
        /// Index of value
        /// </summary>
        public int IndexOf(object value)
        {
            return this.IndexOf((TEntity)value);
        }

        /// <summary>
        /// Insert the specified object at the specified index
        /// </summary>
        public void Insert(int index, object value)
        {
            this.Insert(index, (TEntity)value);
        }

        /// <summary>
        /// Remove the specified object
        /// </summary>
        public void Remove(object value)
        {
            this.Remove((TEntity)value);
        }

        /// <summary>
        /// Copy the specified data to an array
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            this.CopyTo((TEntity[])array, index);
        }

        /// <summary>
        /// Get locked version
        /// </summary>
        /// <returns></returns>
        IEnumerable ILockable.GetLocked()
        {
            return this.GetLocked();
        }

        /// <summary>
        /// Creates a new simple association from the specified list
        /// </summary>
        public static implicit operator SimpleAssociationCollection<TEntity>(List<TEntity> entity)
        {
            return new SimpleAssociationCollection<TEntity>() { m_sourceData = entity };
        } 
    }
}
