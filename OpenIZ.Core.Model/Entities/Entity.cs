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

        /// <summary>
        /// Constructs a new entity
        /// </summary>
        public Entity()
        {
            this.Notes = new List<EntityNote>();
            this.Identifiers = new List<EntityIdentifier>();
            this.Addresses = new List<EntityAddress>();
            this.Extensions = new List<EntityExtension>();
            this.Names = new List<EntityName>();
            this.Participations = new List<ActParticipation>();
            this.Relationships = new List<EntityRelationship>();
            this.Telecoms = new List<EntityTelecomAddress>();
            this.Tags = new List<EntityTag>();
        }

        /// <summary>
        /// Should serialize previous version?
        /// </summary>
        public bool ShouldSerializeCreationAct() { return this.CreationActKey.HasValue; }

        /// <summary>
        /// Class concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("classConcept"), JsonProperty("classConcept")]
        public virtual Guid? ClassConceptKey
        {
            get { return this.ClassConcept?.Key; }
            set
            {
                if (this.ClassConcept?.Key != value)
                    this.ClassConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Determiner concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("determinerConcept"), JsonProperty("determinerConcept")]
        public virtual Guid? DeterminerConceptKey
        {
            get { return this.DeterminerConcept?.Key; }
            set
            {
                if(this.DeterminerConcept?.Key != value)
                    this.DeterminerConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("statusConcept"), JsonProperty("statusConcept")]
        public Guid?  StatusConceptKey
        {
            get { return this.StatusConcept?.Key; }
            set
            {
                if(this.StatusConcept?.Key != value)
                    this.StatusConcept = this.EntityProvider.Get<Concept>(value);
            }
        }
        
        /// <summary>
        /// Creation act reference
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("creationAct"), JsonProperty("creationAct")]
        public Guid?  CreationActKey
        {
            get { return this.CreationAct?.Key; }
            set
            {
                if (this.CreationAct?.Key != value)
                    this.CreationAct = this.EntityProvider.Get<Act>(value);

            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("typeConcept"), JsonProperty("typeConcept")]
        public Guid?  TypeConceptKey
        {
            get { return this.TypeConcept?.Key; }
            set
            {
                if(this.TypeConcept?.Key != value)
                    this.TypeConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Class concept datal load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad()]
        public Concept ClassConcept { get; set; }

        /// <summary>
        /// Determiner concept
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad()]
        public virtual Concept DeterminerConcept { get; set; }

        /// <summary>
        /// Status concept id
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad()]
        public Concept StatusConcept { get; set; }

        /// <summary>
        /// Creation act reference
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(CreationActKey))]
		public Act CreationAct { get; set; }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [AutoLoad()]
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(TypeConceptKey))]
		public Concept TypeConcept { get; set; }

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
        public List<EntityRelationship> Relationships { get; set; }

        /// <summary>
        /// Gets a list of all telecommunications addresses associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("telecom"), JsonProperty("telecom")]
        public List<EntityTelecomAddress> Telecoms { get; set; }

        /// <summary>
        /// Gets a list of all extensions associated with the entity
        /// </summary>
        [AutoLoad()]
        [XmlElement("extension"), JsonProperty("extension")]
        public List<EntityExtension> Extensions { get; set; }

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
        
    }
}
