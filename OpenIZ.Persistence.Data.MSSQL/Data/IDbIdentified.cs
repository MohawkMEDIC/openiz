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
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Gets or sets the identified data
    /// </summary>
    public interface IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key of the object
        /// </summary>
        Guid Id { get; set; }
    }

    /// <summary>
    /// Identified
    /// </summary>
    public partial class ControlAct : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }
    }

    /// <summary>
    /// Identified
    /// </summary>
    public partial class PatientEncounter : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }

    }

    /// <summary>
    /// Identified
    /// </summary>
    public partial class TextObservation : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }

    }

    /// <summary>
    /// Identified
    /// </summary>
    public partial class QuantityObservation : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }

    }
    /// <summary>
    /// Identified
    /// </summary>
    public partial class CodedObservation : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }

    }
    /// <summary>
    /// Identified
    /// </summary>
    public partial class SubstanceAdministration : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(ActVersionId))]
        public Guid Id
        {
            get { return this.ActVersionId; }
            set { this.ActVersionId = value; }
        }

    }
    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class DeviceEntity : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class UserEntity : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class Person : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class Organization : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class Place : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class Material : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Entity identification 
    /// </summary>
    public partial class ManufacturedMaterial : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId; }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Application entity identification 
    /// </summary>
    public partial class ApplicationEntity : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
        {
            get { return this.EntityVersionId;  }
            set { this.EntityVersionId = value; }
        }
    }

    /// <summary>
    /// Represents the interface implementation for concept 
    /// </summary>
    public partial class Concept : IDbIdentified
    {
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
    }

    /// <summary>
    /// Entity implementation of IDbIdentified
    /// </summary>
    public partial class Entity : IDbIdentified
    {
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
    }

    /// <summary>
    /// Act implementation of IDbIdentified
    /// </summary>
    public partial class Act : IDbIdentified
    {
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
    }

    /// <summary>
    /// Interface implementation for patient
    /// </summary>
    public partial class Patient : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
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
    }

    /// <summary>
    /// Interface implementation for provider
    /// </summary>
    public partial class Provider : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(EntityVersionId))]
        public Guid Id
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
    }

    /// <summary>
    /// Interface implementation
    /// </summary>
    public partial class IdentifierType : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap(nameof(IdentifierTypeId))]
        public Guid Id
        {
            get
            {
                return this.IdentifierTypeId;
            }

            set
            {
                this.IdentifierTypeId = value;
            }
        }
    }

}
