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
    /// Entity identification 
    /// </summary>
    public partial class DeviceEntity : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key for the interface
        /// </summary>
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("ConceptId")]
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
        [LinqPropertyMap("EntityId")]
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
    /// Interface implementation for patient
    /// </summary>
    public partial class Patient : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [LinqPropertyMap("EntityVersionId")]
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
        [LinqPropertyMap("IdentifierTypeId")]
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
