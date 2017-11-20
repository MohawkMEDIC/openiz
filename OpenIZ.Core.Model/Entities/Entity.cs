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
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents the base of all entities
	/// </summary>
	[XmlType("Entity", Namespace = "http://openiz.org/model"), JsonObject("Entity")]
	[XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Entity")]
	[Classifier(nameof(ClassConcept))]
	public class Entity : VersionedEntityData<Entity>, ITaggable, IExtendable
	{

        private TemplateDefinition m_template;
        private Guid? m_templateKey;

		// Class concept
		private Concept m_classConcept;

		// Classe concept
		private Guid? m_classConceptId;

		// TODO: Change this to Act
		private Act m_creationAct;

		// Control act which created this
		private Guid? m_creationActId;

		// Determiner concept
		private Concept m_determinerConcept;

		// Determiner concept id
		private Guid? m_determinerConceptId;

		// Status concept
		private Concept m_statusConcept;

		// Status
		private Guid? m_statusConceptId;

		// Type concept
		private Concept m_typeConcept;

		// Type concept
		private Guid? m_typeConceptId;

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
		/// Gets a list of all addresses associated with the entity
		/// </summary>
		[AutoLoad()]
		[AutoLoad(EntityClassKeyStrings.Patient)]
		[AutoLoad(EntityClassKeyStrings.ServiceDeliveryLocation)]
		[AutoLoad(EntityClassKeyStrings.Provider)]
		[AutoLoad(EntityClassKeyStrings.Person)]
		[AutoLoad(EntityClassKeyStrings.Place)]
		[AutoLoad(EntityClassKeyStrings.State)]
		[AutoLoad(EntityClassKeyStrings.Country)]
		[AutoLoad(EntityClassKeyStrings.CountyOrParish)]
		[AutoLoad(EntityClassKeyStrings.CityOrTown)]
		[AutoLoad(EntityClassKeyStrings.Organization)]
		[XmlElement("address"), JsonProperty("address")]
		public List<EntityAddress> Addresses { get; set; }

		/// <summary>
		/// Class concept datal load property
		/// </summary>
		[XmlIgnore, JsonIgnore]
		[AutoLoad()]
		[SerializationReference(nameof(ClassConceptKey))]
		public Concept ClassConcept
		{
			get
			{
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
		/// Class concept
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("classConcept"), JsonProperty("classConcept")]
		[Binding(typeof(EntityClassKeys))]
		public virtual Guid? ClassConceptKey
		{
			get { return this.m_classConceptId; }
			set
			{
				if (this.m_classConceptId != value)
				{
					this.m_classConceptId = value;
					this.m_classConcept = null;
				}
			}
		}

		/// <summary>
		/// Creation act reference
		/// </summary>
		[SerializationReference(nameof(CreationActKey))]
		[XmlIgnore, JsonIgnore]
		public Act CreationAct
		{
			get
			{
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
		/// Creation act reference
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("creationAct"), JsonProperty("creationAct")]
		public Guid? CreationActKey
		{
			get { return this.m_creationActId; }
			set
			{
				if (this.m_creationActId != value)
				{
					this.m_creationActId = value;
					this.m_creationAct = null;
				}
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
		/// Determiner concept
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("determinerConcept"), JsonProperty("determinerConcept")]
		[Binding(typeof(DeterminerKeys))]
		public virtual Guid? DeterminerConceptKey
		{
			get { return this.m_determinerConceptId; }
			set
			{
				if (this.m_determinerConceptId != value)
				{
					this.m_determinerConceptId = value;
					this.m_determinerConcept = null;
				}
			}
		}

		/// <summary>
		/// Gets a list of all extensions associated with the entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("extension"), JsonProperty("extension")]
		public List<EntityExtension> Extensions
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the identifiers associated with this entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("identifier"), JsonProperty("identifier")]
		public List<EntityIdentifier> Identifiers { get; set; }

		/// <summary>
		/// Gets a list of all names associated with the entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("name"), JsonProperty("name")]
		public List<EntityName> Names { get; set; }

		/// <summary>
		/// Gets a list of all notes associated with the entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("note"), JsonProperty("note")]
		public List<EntityNote> Notes { get; set; }

		/// <summary>
		/// Gets the acts in which this entity participates
		/// </summary>
		[XmlElement("participation"), JsonProperty("participation")]
		public List<ActParticipation> Participations { get; set; }

		/// <summary>
		/// Gets a list of all associated entities for this entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("relationship"), JsonProperty("relationship")]
		public List<EntityRelationship> Relationships
		{
			get;
			set;
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
		/// Status concept id
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("statusConcept"), JsonProperty("statusConcept")]
		[Binding(typeof(StatusKeys))]
		public Guid? StatusConceptKey
		{
			get { return this.m_statusConceptId; }
			set
			{
				if (this.m_statusConceptId != value)
				{
					this.m_statusConceptId = value;
					this.m_statusConcept = null;
				}
			}
		}

		/// <summary>
		/// Gets a list of all tags associated with the entity
		/// </summary>
		[AutoLoad()]
		[XmlElement("tag"), JsonProperty("tag")]
		public List<EntityTag> Tags { get; set; }

		/// <summary>
		/// Gets a list of all telecommunications addresses associated with the entity
		/// </summary>
		[AutoLoad(EntityClassKeyStrings.Patient)]
		[AutoLoad(EntityClassKeyStrings.ServiceDeliveryLocation)]
		[AutoLoad(EntityClassKeyStrings.Provider)]
		[AutoLoad(EntityClassKeyStrings.Organization)]
		[AutoLoad(EntityClassKeyStrings.Person)]
		[XmlElement("telecom"), JsonProperty("telecom")]
		public List<EntityTelecomAddress> Telecoms
		{
			get;
			set;
		}

        /// <summary>
        /// Gets the template key
        /// </summary>
        [XmlElement("template"), JsonProperty("template")]
        public Guid? TemplateKey
        {
            get
            {
                return this.m_templateKey;
            }
            set
            {
                this.m_templateKey = value;
                if (value.HasValue && value != this.m_template?.Key)
                    this.m_template = null;
            }
        }

        /// <summary>
        /// Gets or sets the template definition
        /// </summary>
        [AutoLoad, SerializationReference(nameof(TemplateKey)), XmlIgnore, JsonIgnore]
        public TemplateDefinition Template
        {
            get
            {
                this.m_template = base.DelayLoad(this.m_templateKey, this.m_template);
                return this.m_template;
            }
            set
            {
                this.m_template = value;
                this.m_templateKey = value?.Key;
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
			get
			{
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
		/// Type concept identifier
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("typeConcept"), JsonProperty("typeConcept")]
		public Guid? TypeConceptKey
		{
			get { return this.m_typeConceptId; }
			set
			{
				if (this.m_typeConceptId != value)
				{
					this.m_typeConceptId = value;
					this.m_typeConcept = null;
				}
			}
		}

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
			//this.Relationships.RemoveAll(o => o.Clean().IsEmpty());
			//this.Participations.RemoveAll(o => o.Clean().IsEmpty());
			return this;
		}

		/// <summary>
		/// Semantic equality function
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as Entity;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.Addresses?.SemanticEquals(other.Addresses) == true &&
				this.ClassConceptKey == other.ClassConceptKey &&
				this.CreationActKey == other.CreationActKey &&
				this.DeterminerConceptKey == other.DeterminerConceptKey &&
				this.Extensions?.SemanticEquals(other.Extensions) == true &&
				this.Identifiers?.SemanticEquals(other.Identifiers) == true &&
				this.Names?.SemanticEquals(other.Names) == true &&
				this.Notes?.SemanticEquals(other.Notes) == true &&
				this.Participations?.SemanticEquals(other.Participations) == true &&
				this.Relationships?.SemanticEquals(other.Relationships) == true &&
				this.StatusConceptKey == other.StatusConceptKey &&
				this.Tags?.SemanticEquals(other.Tags) == true &&
				this.Telecoms?.SemanticEquals(other.Telecoms) == true &&
				this.Template?.SemanticEquals(other.Template) == true &&
				this.TypeConceptKey == other.TypeConceptKey;
		}

		/// <summary>
		/// Should serialize creation act
		/// </summary>
		public bool ShouldSerializeCreationActKey() => this.CreationActKey.HasValue;

		/// <summary>
		/// Should serialize type concept
		/// </summary>
		public bool ShouldSerializeTypeConceptKey() => this.TypeConceptKey.HasValue;

        /// <summary>
        /// Should serialize identifiers
        /// </summary>
        public bool ShouldSerializeIdentifiers() => this.Identifiers?.Count > 0;

        /// <summary>
        /// Should serialize Names
        /// </summary>
        public bool ShouldSerializeNames() => this.Names?.Count > 0;

        /// <summary>
        /// Should serialize addresses
        /// </summary>
        public bool ShouldSerializeAddresses () => this.Addresses?.Count > 0;

        /// <summary>
        /// Should serialize participations
        /// </summary>
        public bool ShouldSerializeParticipations() => this.Participations?.Count > 0;

        /// <summary>
        /// Should serialize tags
        /// </summary>
        public bool ShouldSerializeTags () => this.Tags?.Count > 0;

        /// <summary>
        /// Shoudl serialize extensions
        /// </summary>
        public bool ShouldSerializeExtensions() =>this.Extensions?.Count > 0;

        /// <summary>
        /// Should serialize notes
        /// </summary>
        public bool ShouldSerializeNotes() => this.Notes?.Count > 0;

        /// <summary>
        /// Should serialize telecoms
        /// </summary>
        public bool ShouldSerializeTelecoms() => this.Telecoms?.Count > 0;

        [XmlIgnore, JsonIgnore]
        IEnumerable<ITag> ITaggable.Tags { get { return this.Tags.OfType<ITag>(); } }

        [XmlIgnore, JsonIgnore]
        IEnumerable<IModelExtension> IExtendable.Extensions { get { return this.Extensions.OfType<IModelExtension>(); } }

        /// <summary>
        /// Copies the entity
        /// </summary>
        /// <returns></returns>
        public IdentifiedData Copy()
        {
            var retVal = base.Clone() as Entity;
            retVal.Relationships = new List<EntityRelationship>(this.Relationships.ToArray());
            retVal.Identifiers = new List<EntityIdentifier>(this.Identifiers.ToArray());
            retVal.Names = new List<EntityName>(this.Names.ToArray());
            retVal.Notes = new List<EntityNote>(this.Notes.ToArray());
            retVal.Participations = new List<ActParticipation>(this.Participations.ToArray());
            retVal.Addresses = new List<EntityAddress>(this.Addresses.ToArray());
            retVal.Tags = new List<EntityTag>(this.Tags.ToArray());
            retVal.Extensions = new List<EntityExtension>(this.Extensions.ToArray());
            return retVal;
        }

        /// <summary>
        /// Should serialize template key
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeTemplateKey() => this.TemplateKey.GetValueOrDefault() != Guid.Empty;

    }
}