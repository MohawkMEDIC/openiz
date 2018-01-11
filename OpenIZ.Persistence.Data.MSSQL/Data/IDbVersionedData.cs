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
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Versioned Database data
    /// </summary>
    public interface IDbVersionedData : IDbBaseData
    {
        /// <summary>
        /// Gets whether the object is readonly 
        /// </summary>
        bool IsReadonly { get; }

        /// <summary>
        /// Gets or sets the version identifier
        /// </summary>
        Guid VersionId { get; set; }

        /// <summary>
        /// Gets or sets the version sequence
        /// </summary>
        decimal VersionSequenceId { get; set; }

        /// <summary>
        /// Gets or sets teh version of the object this replaces
        /// </summary>
        Guid? ReplacesVersionId { get; set; }
    }

    /// <summary>
    /// Versioned data with chained property
    /// </summary>
    /// <typeparam name="TDomainKey"></typeparam>
    public interface IDbVersionedData<TDomainKey> : IDbVersionedData
    { 
        /// <summary>
        /// Gets or sets the non versioned object
        /// </summary>
        TDomainKey NonVersionedObject { get; set; }
    }

    /// <summary>
    /// Represents the implementation of interfaces for entity version
    /// </summary>
    public partial class EntityVersion : IDbVersionedData<Entity>
    {
        /// <summary>
        /// Indication whether value is readonly 
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(EntityId))]
        public Guid Id
        {
            get
            {
                return this.EntityId;
            }

            set
            {
                this.EntityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid VersionId
        {
            get
            {
                return this.EntityVersionId;
            }

            set
            {
                this.EntityVersionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the non versioned object
        /// </summary>
        [LinqPropertyMap(nameof(Entity))]

        public Entity NonVersionedObject
        {
            get
            {
                return this.Entity;
            }

            set
            {
                this.Entity = value;
            }
        }
    }

    /// <summary>
    /// Represents the implementation of interfaces for act version
    /// </summary>
    public partial class ActVersion : IDbVersionedData<Act>
    {
        /// <summary>
        /// Indication whether value is readonly 
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(ActId))]
        public Guid Id
        {
            get
            {
                return this.ActId;
            }

            set
            {
                this.ActId = value;
            }
        }

        /// <summary>
        /// Gets or sets the version id
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid VersionId
        {
            get
            {
                return this.ActVersionId;
            }

            set
            {
                this.ActVersionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the non versioned object
        /// </summary>
        [LinqPropertyMap(nameof(Act))]
        public Act NonVersionedObject
        {
            get
            {
                return this.Act;
            }

            set
            {
                this.Act = value;
            }
        }
    }

    /// <summary>
    /// Concept version 
    /// </summary>
    public partial class ConceptVersion : IDbVersionedData<Concept>
    {
        /// <summary>
        /// Gets readonly status
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                return this.Concept.IsSystemConcept;
            }
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(ConceptId))]
        public Guid Id
        {
            get
            {
                return this.ConceptId;
            }

            set
            {
                this.ConceptId = value;
            }
        }

        /// <summary>
        /// Gets or sets the version key
        /// </summary>
        [LinqPropertyMap(nameof(ConceptVersionId))]
        public Guid VersionId
        {
            get
            {
                return this.ConceptVersionId;
            }

            set
            {
                this.ConceptVersionId = value;
            }
        }

        /// <summary>
        /// Gets or sets the non versioned object
        /// </summary>
        [LinqPropertyMap(nameof(Concept))]
        public Concept NonVersionedObject
        {
            get
            {
                return this.Concept;
            }

            set
            {
                this.Concept = value;
            }
        }

    }

}
