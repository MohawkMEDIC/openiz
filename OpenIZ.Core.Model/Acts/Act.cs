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

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents the base class for an act
    /// </summary>
    [XmlType(Namespace ="http://openiz.org/model", TypeName ="Act")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Act")]
    [Classifier(nameof(ClassConcept))]
    public class Act : VersionedEntityData<Act>
    {

        /// <summary>
        /// Creates the act
        /// </summary>
        public Act()
        {
            this.Participations = new List<ActParticipation>();
            this.Identifiers = new List<ActIdentifier>();
            this.Extensions = new List<ActExtension>();
            this.Relationships = new List<ActRelationship>();
            this.Notes = new List<ActNote>();
            this.Tags = new List<ActTag>();
            
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
        [DataIgnore, XmlElement("classConcept"), JsonProperty("classConcept")]
        public virtual Guid ClassConceptKey
        {
            get { return this.ClassConcept.Key.Value; }
            set
            {
                if (value != this.ClassConcept?.Key)
                    this.ClassConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Mood concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("moodConcept"), JsonProperty("moodConcept")]
        public virtual Guid? MoodConceptKey
        {
            get { return this.MoodConcept?.Key; }
            set
            {
                if (this.MoodConcept?.Key != value)
                    this.MoodConcept = this.EntityProvider.Get<Concept>(value);
            }
        }


        /// <summary>
        /// Reason concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("reasonConcept"), JsonProperty("reasonConcept")]
        public virtual  Guid? ReasonConceptKey
        {
            get { return this.ReasonConcept?.Key; }
            set
            {
                if (this.ReasonConcept?.Key != value)
                    this.ReasonConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Status concept id
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("statusConcept"), JsonProperty("statusConcept")]
        public Guid? StatusConceptKey
        {
            get { return this.StatusConcept?.Key; }
            set
            {
                if (this.StatusConcept?.Key != value)
                    this.StatusConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("typeConcept"), JsonProperty("typeConcept")]
        public Guid? TypeConceptKey
        {
            get { return this.TypeConcept?.Key; }
            set
            {
                if (this.TypeConcept?.Key != value)
                    this.TypeConcept = this.EntityProvider.Get<Concept>(value);
            }
        }


        /// <summary>
        /// Class concept datal load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(ClassConceptKey))]
        public Concept ClassConcept { get; set; }

        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(MoodConceptKey))]
        public Concept MoodConcept { get; set; }

        /// <summary>
        /// Mood concept data load property
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ReasonConceptKey))]
	    public Concept ReasonConcept { get; set; }

        /// <summary>
        /// Status concept id
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(StatusConceptKey))]
		public Concept StatusConcept { get; set; }

        /// <summary>
        /// Type concept identifier
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(TypeConceptKey))]
		public Concept TypeConcept { get; set; }

        /// <summary>
        /// Gets the identifiers associated with this act
        /// </summary>
        [XmlElement("identifier"), JsonProperty("identifier")]
        public List<ActIdentifier> Identifiers { get; set; }

        /// <summary>
        /// Gets a list of all associated acts for this act
        /// </summary>
        [XmlElement("relationship"), JsonProperty("relationship")]
        public List<ActRelationship> Relationships { get; set; }

        /// <summary>
        /// Gets a list of all extensions associated with the act
        /// </summary>
        [XmlElement("extension"), JsonProperty("extension")]
        public List<ActExtension> Extensions { get; set; }

        /// <summary>
        /// Gets a list of all notes associated with the act
        /// </summary>
        [XmlElement("note"), JsonProperty("note")]
        public List<ActNote> Notes { get; set; }

        /// <summary>
        /// Gets a list of all tags associated with the act
        /// </summary>
        [XmlElement("tag"), JsonProperty("tag")]
        public List<ActTag> Tags { get; set; }

        /// <summary>
        /// Participations
        /// </summary>
        [XmlElement("participation"), JsonProperty("participation")]
        public List<ActParticipation> Participations { get; set; }


    }
}
