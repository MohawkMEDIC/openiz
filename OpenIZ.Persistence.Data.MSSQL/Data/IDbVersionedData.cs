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
    /// Represents the implementation of interfaces for entity version
    /// </summary>
    public partial class EntityVersion : IDbVersionedData
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
        [Column(Name = "EntityId", AutoSync = AutoSync.Never)]
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
        [Column(Name = "EntityVersionId", AutoSync = AutoSync.Never)]
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
        
    }

    /// <summary>
    /// Represents the implementation of interfaces for act version
    /// </summary>
    public partial class ActVersion : IDbVersionedData
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
        [Column(Name = "ActId", AutoSync = AutoSync.Never)]
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
        [Column(Name = "ActVersionId", AutoSync = AutoSync.Never)]
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

    }

    /// <summary>
    /// Concept version 
    /// </summary>
    public partial class ConceptVersion : IDbVersionedData
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
        [Column(Name = "ConceptId", AutoSync = AutoSync.Never)]
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
        [Column(Name = "ConceptVersionId", AutoSync = AutoSync.Never)]
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

    }

}
