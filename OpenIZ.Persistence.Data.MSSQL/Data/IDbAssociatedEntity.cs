using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Represents the databased associated entity
    /// </summary>
    public interface IDbAssociation : IDbIdentified
    {
        /// <summary>
        /// Gets or sets the key of the item associated with this object
        /// </summary>
        Guid AssociatedItemKey { get; set; }
    }

    /// <summary>
    /// Represents the versioned copy of an association
    /// </summary>
    public interface IDbVersionedAssociation : IDbAssociation
    { 
        /// <summary>
        /// Gets or sets the version when the relationship is effective
        /// </summary>
        Decimal EffectiveVersionSequenceId { get; set; }
        
        /// <summary>
        /// Gets or sets the verson when the relationship is not effecitve
        /// </summary>
        Decimal? ObsoleteVersionSequenceId { get; set; }
    }

    /// <summary>
    /// Entity address component
    /// </summary>
    public partial class EntityAddressComponent : IDbAssociation
    {
        /// <summary>
        /// Get the associated item key
        /// </summary>
        [LinqPropertyMap("EntityAddressId")]
        public Guid AssociatedItemKey
        {
            get
            {
                return this.EntityAddressId;
            }

            set
            {
                this.EntityAddressId = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityAddressComponentId;
            }

            set
            {
                this.EntityAddressComponentId = value;

            }
        }
    }

    /// <summary>
    /// Represents the interface implementations for concept name
    /// </summary>
    public partial class ConceptName : IDbVersionedAssociation
    {
        /// <summary>
        /// Represents the associated item key
        /// </summary>
        [LinqPropertyMap(nameof(ConceptId))]
        public Guid AssociatedItemKey
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
        /// Gets or sets the unique identifier
        /// </summary>
        [LinqPropertyMap(nameof(ConceptNameId))]
        public Guid Id
        {
            get
            {
                return this.ConceptNameId;
            }

            set
            {
                ConceptNameId = value;
            }
        }
    }

    /// <summary>
    /// Entity name component
    /// </summary>
    public partial class EntityNameComponent : IDbAssociation
    {
        /// <summary>
        /// Get the associated item key
        /// </summary>
        public Guid AssociatedItemKey
        {
            get
            {
                return this.EntityNameId;
            }

            set
            {
                this.EntityNameId = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityNameComponentId;
            }

            set
            {
                this.EntityNameComponentId = value;

            }
        }
    }

    /// <summary>
    /// Partial implementing entity address
    /// </summary>
    public partial class EntityAddress : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityAddressId;
            }

            set
            {
                this.EntityAddressId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity name
    /// </summary>
    public partial class EntityName : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityNameId;
            }

            set
            {
                this.EntityNameId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity Association
    /// </summary>
    public partial class EntityAssociation : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
        {
            get
            {
                return this.SourceEntityId;
            }

            set
            {
                this.SourceEntityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityAssociationId;
            }

            set
            {
                this.EntityAssociationId = value;
            }
        }
    }
    /// <summary>
    /// Partial implementing entity Note
    /// </summary>
    public partial class EntityNote : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityNoteId;
            }

            set
            {
                this.EntityNoteId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity Identifier
    /// </summary>
    public partial class EntityIdentifier : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityIdentifierId;
            }

            set
            {
                this.EntityIdentifierId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity Extension
    /// </summary>
    public partial class EntityExtension : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityExtensionId;
            }

            set
            {
                this.EntityExtensionId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity TelecomAddress
    /// </summary>
    public partial class EntityTelecomAddress : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityTelecomAddressId;
            }

            set
            {
                this.EntityTelecomAddressId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity Tag
    /// </summary>
    public partial class EntityTag : IDbAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
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
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.EntityTagId;
            }

            set
            {
                this.EntityTagId = value;
            }
        }
    }

    /// <summary>
    /// Partial implementing entity language
    /// </summary>
    public partial class PersonLanguageCommunication : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets the associated item key
        /// </summary>
        public Guid AssociatedItemKey
        {
            get
            {
                return this.PersonEntityId;
            }

            set
            {
                this.PersonEntityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.PersonLanguageCommunicationId;
            }

            set
            {
                this.PersonLanguageCommunicationId = value;
            }
        }
    }

    /// <summary>
    /// Implementation of interfaces for place service
    /// </summary>
    public partial class PlaceService : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the item key
        /// </summary>
        public Guid AssociatedItemKey
        {
            get
            {
                return this.PlaceEntityId;
            }

            set
            {
                this.PlaceEntityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.PlaceServiceId;
            }

            set
            {
                this.PlaceServiceId = value;
            }
        }
    }

    /// <summary>
    /// Implementation of interfaces for place service
    /// </summary>
    public partial class ConceptReferenceTerm : IDbVersionedAssociation
    {
        /// <summary>
        /// Gets or sets the item key
        /// </summary>
        [LinqPropertyMap("ConceptId")]
        public Guid AssociatedItemKey
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
        /// Gets or sets the unique identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.ConceptReferenceTermId;
            }

            set
            {
                this.ConceptReferenceTermId = value;
            }
        }
    }

}
