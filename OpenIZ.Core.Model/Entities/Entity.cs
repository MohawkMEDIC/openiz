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
 * Date: 2016-7-16
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
        private Guid? m_classConceptId;
        // Determiner concept id
        private Guid? m_determinerConceptId;
        // Status 
        private Guid? m_statusConceptId;
        // Control act which created this
        private Guid? m_creationActId;
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

        /// <summary>
        /// Creates a new instance of the entity class
        /// </summary>
        public Entity()
        {
            this.Identifiers = new List<EntityIdentifier>();
            this.Addresses = new List<EntityAddress>();
            this.Extensions = new List<EntityExtension>();
            this.Names = new List<EntityName>();
            this.Notes = new List<EntityNote>();
            this.Participations = new List<ActParticipation>();
            this.Relationships = new List<EntityRelationship>();
            this.Telecoms = new List<EntityTelecomAddress>();
            this.Tags = new List<EntityTag>();
        }

        /// <summary>
        /// Class concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("classConcept"), JsonProperty("classConcept")]
        public virtual Guid? ClassConceptKey
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
        public virtual Guid? DeterminerConceptKey
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
        public Guid?  StatusConceptKey
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
        public Guid?  CreationActKey
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
        [AutoLoad()]
        [SerializationReference(nameof(ClassConceptKey))]
        public Concept ClassConcept
        {
            get {
                this.m_classConcept = base.DelayLoad(this.m_classConceptId, this.m_classConcept);
                return this.m_classConcept;
            }
            set
            {
                this.m_classConcept = value;
                this.m_classConceptId = value?.Key;
            }
        }

        /// <summary>
        /// Determiner concept
        /// </summary>
        [SerializationReference(nameof(DeterminerConceptKey))]
        [XmlIgnore, JsonIgnore]
        [AutoLoad()]   
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
        [SerializationReference(nameof(StatusConceptKey))]
        [XmlIgnore, JsonIgnore]
        [AutoLoad()]
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
        [SerializationReference(nameof(CreationActKey))]
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
                this.m_creationActId = value?.Key;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [SerializationReference(nameof(TypeConceptKey))]
        [AutoLoad()]
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
        [AutoLoad()]
        [XmlElement("identifier"), JsonProperty("identifier")]
        public List<EntityIdentifier> Identifiers { get; set; }

        /// <summary>
        /// Gets a list of all associated entities for this entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("relationship"), JsonProperty("relationship")]
        public List<EntityRelationship> Relationships {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all telecommunications addresses associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("telecom"), JsonProperty("telecom")]
        public List<EntityTelecomAddress> Telecoms {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all extensions associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("extension"), JsonProperty("extension")]
        public List<EntityExtension> Extensions {
            get;
            set;
        }


        /// <summary>
        /// Gets a list of all names associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("name"), JsonProperty("name")]
        public List<EntityName> Names { get; set; }

        /// <summary>
        /// Gets a list of all addresses associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("address"), JsonProperty("address")]
        public List<EntityAddress> Addresses { get; set; }

        /// <summary>
        /// Gets a list of all notes associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("note"), JsonProperty("note")]
        public List<EntityNote> Notes { get; set; }

        /// <summary>
        /// Gets a list of all tags associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("tag"), JsonProperty("tag")]
        public List<EntityTag> Tags { get; set; }
        
        /// <summary>
        /// Gets the acts in which this entity participates
        /// </summary>
        [AutoLoad()]
        [XmlElement("participation"), JsonProperty("participation")]
        public List<ActParticipation> Participations { get; set; }

        /// <summary>
        /// Clean the patient of any empty "noise" elements
        /// </summary>
        /// <returns></returns>
        public override IdentifiedData Clean()
        {
            this.Addresses.RemoveAll(o => o.Clean().IsEmpty());
            this.Names.RemoveAll(o => o.Clean().IsEmpty());
            this.Telecoms.RemoveAll(o => o.Clean().IsEmpty());
            this.Tags.RemoveAll(o => o.Clean().IsEmpty());
            this.Notes.RemoveAll(o => o.Clean().IsEmpty());
            this.Extensions.RemoveAll(o => o.Clean().IsEmpty());
            this.Identifiers.RemoveAll(o => o.Clean().IsEmpty());

            return this;
        }
    }
}
