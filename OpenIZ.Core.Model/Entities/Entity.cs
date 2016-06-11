/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-2-1
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents the base of all entities
    /// </summary>
    [XmlType("Entity",  Namespace = "http://openiz.org/model"), JsonObject("Entity")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Entity")]
    [Classifier(nameof(ClassConcept))]
    public class Entity : VersionedEntityData<Entity>
    {

        // Classe concept
        private Guid m_classConceptId;
        // Determiner concept id
        private Guid m_determinerConceptId;
        // Status 
        private Guid m_statusConceptId;
        // Control act which created this
        private Guid m_creationActId;
        // Type concept
        private Guid? m_typeConceptId;

        // Class concept
        private Concept m_classConcept;
        // Determiner concept
        
        private Concept m_determinerConcept;
        // TODO: Change this to Act
        private Act m_creationAct;
        // Status concept
        private Concept m_statusConcept;
        // Type concept
        private Concept m_typeConcept;

        // Identifiers 
        private List<EntityIdentifier> m_identifiers;
        // Associations
        private List<EntityRelationship> m_relationships;
        // Telecom addresses
        private List<EntityTelecomAddress> m_telecomAddresses;
        // Extensions
        private List<EntityExtension> m_extensions;
        // Names
        private List<EntityName> m_names;
        // Addresses
        private List<EntityAddress> m_addresses;
        // Notes
        private List<EntityNote> m_notes;
        // Tags
        private List<EntityTag> m_tags;
        // Participations
        private List<ActParticipation> m_participations;

        /// <summary>
        /// Class concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("classConcept"), JsonProperty("classConcept")]
        public virtual Guid ClassConceptKey
        {
            get { return this.m_classConceptId; }
            set
            {
                this.m_classConceptId = value;
                this.m_classConcept = null;
            }
        }

        /// <summary>
        /// Determiner concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("determinerConcept"), JsonProperty("determinerConcept")]
        public virtual Guid DeterminerConceptKey
        {
            get { return this.m_determinerConceptId; }
            set
            {
                this.m_determinerConceptId = value;
                this.m_determinerConcept = null;
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("statusConcept"), JsonProperty("statusConcept")]
        public Guid  StatusConceptKey
        {
            get { return this.m_statusConceptId; }
            set
            {
                this.m_statusConceptId = value;
                this.m_statusConcept = null;
            }
        }
        
        /// <summary>
        /// Creation act reference
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("creationAct"), JsonProperty("creationAct")]
        public Guid  CreationActKey
        {
            get { return this.m_creationActId; }
            set
            {
                this.m_creationActId = value;
                this.m_creationAct = null;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("typeConcept"), JsonProperty("typeConcept")]
        public Guid?  TypeConceptKey
        {
            get { return this.m_typeConceptId; }
            set
            {
                this.m_typeConceptId = value;
                this.m_typeConcept = null;
            }
        }

        /// <summary>
        /// Class concept datal load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [DelayLoad(nameof(ClassConceptKey))]
        public Concept ClassConcept
        {
            get {
                this.m_classConcept = base.DelayLoad(this.m_classConceptId, this.m_classConcept);
                return this.m_classConcept;
            }
            set
            {
                this.m_classConcept = value;
                if (value == null)
                    this.m_classConceptId = Guid.Empty;
                else
                    this.m_classConceptId = value.Key;
            }
        }

        /// <summary>
        /// Determiner concept
        /// </summary>
        [DelayLoad(nameof(DeterminerConceptKey))]
        [XmlIgnore, JsonIgnore]
        public virtual Concept DeterminerConcept
        {
            get
            {
                this.m_determinerConcept = base.DelayLoad(this.m_determinerConceptId, this.m_determinerConcept);
                return this.m_determinerConcept;
            }
            set
            {
                this.m_determinerConcept = value;
                if (value == null)
                    this.m_determinerConceptId = Guid.Empty;
                else
                    this.m_determinerConceptId = value.Key;
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [DelayLoad(nameof(StatusConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept StatusConcept
        {
            get
            {
                this.m_statusConcept = base.DelayLoad(this.m_statusConceptId, this.m_statusConcept);
                return this.m_statusConcept;
            }
            set
            {
                this.m_statusConcept = value;
                if (value == null)
                    this.m_statusConceptId = Guid.Empty;
                else
                    this.m_statusConceptId = value.Key;
            }
        }

        /// <summary>
        /// Creation act reference
        /// </summary>
        [DelayLoad(nameof(CreationActKey))]
        [XmlIgnore, JsonIgnore]
        public Act CreationAct
        {
            get {
                this.m_creationAct = base.DelayLoad(this.m_creationActId, this.m_creationAct);
                return this.m_creationAct;
            }
            set
            {
                this.m_creationAct = value;
                if (value != null)
                    this.m_creationActId = value.Key;
                else
                    this.m_creationActId = Guid.Empty;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [DelayLoad(nameof(TypeConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept TypeConcept
        {
            get {
                this.m_typeConcept = base.DelayLoad(this.m_typeConceptId, this.m_typeConcept);
                return this.m_typeConcept;
            }
            set
            {
                this.m_typeConcept = value;
                this.m_typeConceptId = value?.Key;
            }
        }

        /// <summary>
        /// Gets the identifiers associated with this entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("identifier"), JsonProperty("identifier")]
        public List<EntityIdentifier> Identifiers
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_identifiers = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_identifiers);
                return this.m_identifiers;
            }
            set
            {
                this.m_identifiers = value;
            }
        }

        /// <summary>
        /// Gets a list of all associated entities for this entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("relationship"), JsonProperty("relationship")]
        public List<EntityRelationship> Relationships
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_relationships = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_relationships);

                return this.m_relationships;
            }
            set
            {
                this.m_relationships = value;
            }
        }

        /// <summary>
        /// Gets a list of all telecommunications addresses associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("telecom"), JsonProperty("telecom")]
        public List<EntityTelecomAddress> Telecoms
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_telecomAddresses = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_telecomAddresses);

                return this.m_telecomAddresses;
            }
            set
            {
                this.m_telecomAddresses = value;
            }
        }

        /// <summary>
        /// Gets a list of all extensions associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("extension"), JsonProperty("extension")]
        public List<EntityExtension> Extensions
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_extensions = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_extensions);

                return this.m_extensions;
            }
            set
            {
                this.m_extensions = value;
            }
        }

        /// <summary>
        /// Gets a list of all names associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("name"), JsonProperty("name")]
        public List<EntityName> Names
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_names = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_names);

                return this.m_names;
            }
            set
            {
                this.m_names = value;
            }
        }

        /// <summary>
        /// Gets a list of all addresses associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("address"), JsonProperty("address")]
        public List<EntityAddress> Addresses
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_addresses = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_addresses);

                return this.m_addresses;
            }
            set
            {
                this.m_addresses = value;
            }
        }

        /// <summary>
        /// Gets a list of all notes associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("note"), JsonProperty("note")]
        public List<EntityNote> Notes
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_notes = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_notes);

                return this.m_notes;
            }
            set
            {
                this.m_notes = value;
            }
        }

        /// <summary>
        /// Gets a list of all tags associated with the entity
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("tag"), JsonProperty("tag")]
        public List<EntityTag> Tags
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_tags = EntitySource.Current.GetRelations(this.Key, this.m_tags);

                return this.m_tags;
            }
            set
            {
                this.m_tags = value;
            }
        }

        /// <summary>
        /// Gets the acts in which this entity participates
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("participation"), JsonProperty("participation")]
        public List<ActParticipation> Participations
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_participations = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_participations);

                return this.m_participations;
            }
            set
            {
                this.m_participations = value;
            }
        }

        /// <summary>
        /// Set common delay load properties
        /// </summary>
        public void SetDelayLoadProperties(List<EntityName> names,
            List<EntityAddress> address,
            List<EntityIdentifier> identifiers,
            List<EntityTelecomAddress> telecoms)
        {
            this.m_names = names;
            this.m_addresses = address;
            this.m_identifiers = identifiers;
            this.m_telecomAddresses = telecoms;
        }

        /// <summary>
        /// Forces the delay load properties in this type to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_classConcept = this.m_determinerConcept = this.m_statusConcept = this.m_typeConcept = null;
            this.m_addresses = null;
            this.m_relationships = null;
            this.m_creationAct = null;
            this.m_extensions = null;
            this.m_identifiers = null;
            this.m_names = null;
            this.m_notes = null;
            this.m_tags = null;
            this.m_telecomAddresses = null;
        }

    }
}
