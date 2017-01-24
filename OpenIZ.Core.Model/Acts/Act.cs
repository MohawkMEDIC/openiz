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
using System.Globalization;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents the base class for an act
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model", TypeName = "Act")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Act")]
    [JsonObject("Act")]
    [Classifier(nameof(ClassConcept))]
    public class Act : VersionedEntityData<Act>
    {

        private Guid? m_classConceptKey;
        private Guid? m_typeConceptKey;
        private Guid? m_statusConceptKey;
        private Guid? m_moodConceptKey;
        private Guid? m_reasonConceptKey;

        private Concept m_classConcept;
        private Concept m_typeConcept;
        private Concept m_statusConcept;
        private Concept m_moodConcept;
        private Concept m_reasonConcept;

        /// <summary>
        /// Constructor for ACT
        /// </summary>
        public Act()
        {
            this.Relationships = new VersionedAssociationCollection<ActRelationship>(this);
            this.Identifiers = new VersionedAssociationCollection<ActIdentifier>(this);
            this.Extensions = new VersionedAssociationCollection<ActExtension>(this);
            this.Notes = new VersionedAssociationCollection<ActNote>(this);
            this.Participations = new VersionedAssociationCollection<ActParticipation>(this);
            this.Tags = new SimpleAssociationCollection<ActTag>(this);
            this.Protocols = new VersionedAssociationCollection<ActProtocol>(this);
            this.Policies = new SimpleAssociationCollection<SecurityPolicyInstance>();
            
        }
        /// <summary>
        /// Gets or sets an indicator which identifies whether the object is negated
        /// </summary>
        [XmlElement("isNegated"), JsonProperty("isNegated")]
        public Boolean IsNegated { get; set; }

        /// <summary>
        /// Gets or sets the stop time of the act
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTimeOffset ActTime { get; set; }

        /// <summary>
        /// Gets the template key
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Guid? TemplateKey { get { return this.Template?.Key; } set { } }


        /// <summary>
        /// Gets or sets the template identifier 
        /// </summary>
        [AutoLoad, XmlElement("template"), JsonProperty("template")]
        public TemplateDefinition Template { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [DataIgnore, XmlElement("actTime"), JsonProperty("actTime")]
        public String ActTimeXml
        {
            get { return this.ActTime.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.ActTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.ActTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Gets or sets the start time of the act
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [DataIgnore, XmlElement("startTime"), JsonProperty("startTime")]
        public String StartTimeXml
        {
            get { return this.StartTime?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.StartTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.StartTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Gets or sets the stop time of the act
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public DateTimeOffset? StopTime { get; set; }


        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [DataIgnore, XmlElement("stopTime"), JsonProperty("stopTime")]
        public String StopTimeXml
        {
            get { return this.StopTime?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.StopTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.StopTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Class concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("classConcept"), JsonProperty("classConcept")]
        [Binding(typeof(ActClassKeys))]
        public virtual Guid? ClassConceptKey
        {
            get { return this.m_classConceptKey; }
            set
            {
                if (this.m_classConceptKey != value)
                {
                    this.m_classConceptKey = value;
                    this.m_classConcept = null;
                }
            }
        }

        /// <summary>
        /// Mood concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("moodConcept"), JsonProperty("moodConcept")]
        [Binding(typeof(ActMoodKeys))]
        public virtual Guid? MoodConceptKey
        {
            get { return this.m_moodConceptKey; }
            set
            {
                if (this.m_moodConceptKey != value)
                {
                    this.m_moodConceptKey = value;
                    this.m_moodConcept = null;
                }
            }
        }


        /// <summary>
        /// Reason concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("reasonConcept"), JsonProperty("reasonConcept")]
        [Binding(typeof(ActReasonKeys))]
        public Guid? ReasonConceptKey
        {
            get { return this.m_reasonConceptKey; }
            set
            {
                if (this.m_reasonConceptKey != value)
                {
                    this.m_reasonConceptKey = value;
                    this.m_reasonConcept = null;
                }
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
            get { return this.m_statusConceptKey; }
            set
            {
                if (this.m_statusConceptKey != value)
                {
                    this.m_statusConceptKey = value;
                    this.m_statusConcept = null;
                }
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("typeConcept"), JsonProperty("typeConcept")]
        public Guid? TypeConceptKey
        {
            get { return this.m_typeConceptKey; }
            set
            {
                if (this.m_typeConceptKey != value)
                {
                    this.m_typeConceptKey = value;
                    this.m_typeConcept = null;
                }
            }
        }


        /// <summary>
        /// Class concept datal load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(ClassConceptKey))]
        public Concept ClassConcept
        {
            get
            {
                this.m_classConcept = base.DelayLoad(this.m_classConceptKey, this.m_classConcept);
                return this.m_classConcept;
            }
            set
            {
                this.m_classConcept = value;
                this.m_classConceptKey = value?.Key;
            }
        }

        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(MoodConceptKey))]
        public Concept MoodConcept
        {
            get
            {
                this.m_moodConcept = base.DelayLoad(this.m_moodConceptKey, this.m_moodConcept);
                return this.m_moodConcept;
            }
            set
            {
                this.m_moodConcept = value;
                this.m_moodConceptKey = value?.Key;
            }
        }


        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(ReasonConceptKey))]
        public Concept ReasonConcept
        {
            get
            {
                this.m_reasonConcept = base.DelayLoad(this.m_reasonConceptKey, this.m_reasonConcept);
                return this.m_reasonConcept;
            }
            set
            {
                this.m_reasonConcept = value;
                this.m_reasonConceptKey = value?.Key;
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [AutoLoad, SerializationReference(nameof(StatusConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept StatusConcept
        {
            get
            {
                this.m_statusConcept = base.DelayLoad(this.m_statusConceptKey, this.m_statusConcept);
                return this.m_statusConcept;
            }
            set
            {
                this.m_statusConcept = value;
                if (value == null)
                    this.m_statusConceptKey = Guid.Empty;
                else
                    this.m_statusConceptKey = value.Key;
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [AutoLoad, SerializationReference(nameof(TypeConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept TypeConcept
        {
            get
            {
                this.m_typeConcept = base.DelayLoad(this.m_typeConceptKey, this.m_typeConcept);
                return this.m_typeConcept;
            }
            set
            {
	            this.m_typeConceptKey = value?.Key;
	            this.m_typeConcept = value;
            }
        }

        /// <summary>
        /// Gets the identifiers associated with this act
        /// </summary>
        [AutoLoad, XmlElement("identifier"), JsonProperty("identifier")]
        public VersionedAssociationCollection<ActIdentifier> Identifiers { get; set; }

        /// <summary>
        /// Gets a list of all associated acts for this act
        /// </summary>
        [AutoLoad, XmlElement("relationship"), JsonProperty("relationship")]
        public VersionedAssociationCollection<ActRelationship> Relationships { get; set; }

        /// <summary>
        /// Gets or sets the policy instances
        /// </summary>
        [XmlElement("policy"), JsonProperty("policy")]
        public SimpleAssociationCollection<SecurityPolicyInstance> Policies { get; set; }

        /// <summary>
        /// Gets a list of all extensions associated with the act
        /// </summary>

        [AutoLoad, XmlElement("extension"), JsonProperty("extension")]
        public VersionedAssociationCollection<ActExtension> Extensions { get; set; }

        /// <summary>
        /// Gets a list of all notes associated with the act
        /// </summary>

        [AutoLoad, XmlElement("note"), JsonProperty("note")]
        public VersionedAssociationCollection<ActNote> Notes { get; set; }

        /// <summary>
        /// Gets a list of all tags associated with the act
        /// </summary>

        [AutoLoad, XmlElement("tag"), JsonProperty("tag")]
        public SimpleAssociationCollection<ActTag> Tags { get; set; }

        /// <summary>
        /// Identifies protocols attached to the act
        /// </summary>
        public VersionedAssociationCollection<ActProtocol> Protocols { get; set; }

        /// <summary>
        /// Participations
        /// </summary>
        [XmlElement("participation"), JsonProperty("participation")]
        [AutoLoad]
        public VersionedAssociationCollection<ActParticipation> Participations { get; set; }

        /// <summary>
        /// Forces the delay load properties in this type to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_moodConcept = this.m_reasonConcept = this.m_classConcept = this.m_statusConcept = this.m_typeConcept = null;
        }


        /// <summary>
        /// Clean the patient of any empty "noise" elements
        /// </summary>
        /// <returns></returns>
        public override IdentifiedData Clean()
        {
            this.Tags.RemoveAll(o => o.Clean().IsEmpty());
            this.Notes.RemoveAll(o => o.Clean().IsEmpty());
            this.Extensions.RemoveAll(o => o.Clean().IsEmpty());
            this.Identifiers.RemoveAll(o => o.Clean().IsEmpty());
            return this;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as Act;
            if (other == null) return false;
            return base.SemanticEquals(obj) &&
                this.ActTime == other.ActTime &&
                this.ClassConceptKey == other.ClassConceptKey &&
                this.Extensions?.SemanticEquals(other.Extensions) == true &&
                this.Identifiers?.SemanticEquals(other.Identifiers) == true &&
                this.IsNegated == other.IsNegated &&
                this.MoodConceptKey == other.MoodConceptKey &&
                this.Notes?.SemanticEquals(other.Notes) == true &&
                this.Participations?.SemanticEquals(other.Participations) == true &&
                this.Policies?.SemanticEquals(other.Policies) == true  &&
                this.Protocols?.SemanticEquals(other.Protocols) == true &&
                this.ReasonConceptKey == other.ReasonConceptKey &&
                this.Relationships?.SemanticEquals(other.Relationships) == true &&
                this.StartTime == other.StartTime &&
                this.StatusConceptKey == other.StatusConceptKey &&
                this.StopTime == other.StopTime &&
                this.Tags?.SemanticEquals(other.Tags) == true &&
                this.Template?.SemanticEquals(other.Template) == true &&
                this.TypeConceptKey == other.TypeConceptKey;
        }
    }
}
