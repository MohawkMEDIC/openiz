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
    [JsonObject(nameof(VersionedAssociationCollection<TEntity>))]
    public class VersionedAssociationCollection<TEntity> : SimpleAssociationCollection<TEntity> where TEntity : IdentifiedData, IVersionedAssociation, new()
    {


        /// <summary>
        /// Entity collection
        /// </summary>
        public VersionedAssociationCollection() : base()
        {
        }
        
        /// <summary>
        /// Entity collection
        /// </summary>
        public VersionedAssociationCollection(IEnumerable<TEntity> source) : base(source)
        {
        }

        /// <summary>
        /// Creates the specified entity collection in the specified context
        /// </summary>
        public VersionedAssociationCollection(IVersionedEntity context) : base(context)
        {
        }

        /// <summary>
        /// Perform a delay load on the entire collection
        /// </summary>
        protected override IList<TEntity> Ensure()
        {
            if (this.m_context == null) return this.m_sourceData;

            // Load if needed
            if (this.m_sourceData == null)
            {
                if (this.m_context.Key.HasValue && (this.m_load.HasValue && this.m_load.Value || !this.m_load.HasValue && this.m_context.IsDelayLoadEnabled))
                {
                    this.m_sourceData = new List<TEntity>(EntitySource.Current.Provider.GetRelations<TEntity>(this.m_context.Key, (this.m_context as IVersionedEntity)?.VersionSequence));
                }
                else
                    this.m_sourceData = new List<TEntity>();
            }

            return this.m_sourceData;
        }

        /// <summary>
        /// Creates a new simple association from the specified list
        /// </summary>
        public static implicit operator VersionedAssociationCollection<TEntity>(List<TEntity> entity)
        {
            return new VersionedAssociationCollection<TEntity>() { m_sourceData = entity };
        }
    }
}
