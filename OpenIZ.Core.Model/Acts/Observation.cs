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
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents a class which is an observation
    /// </summary>
    [XmlType("Observation",  Namespace = "http://openiz.org/model"), JsonObject("Observation")]
    
    public abstract class Observation : Act
    {

       
        /// <summary>
        /// Observation ctor
        /// </summary>
        public Observation()
        {
            this.ClassConceptKey = ActClassKeys.Observation;
        }

        /// <summary>
        /// Gets or sets the interpretation concept
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("interpretationConcept"), JsonProperty("interpretationConcept")]
        public Guid? InterpretationConceptKey
        {
            get { return this.InterpretationConcept?.Key; }
            set
            {
                if (value != this.InterpretationConcept?.Key)
                    this.InterpretationConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the concept which indicates the interpretation of the observtion
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(InterpretationConceptKey))]
		public Concept InterpretationConcept { get; set; }

    }

    /// <summary>
    /// Represents an observation that contains a quantity
    /// </summary>
    [XmlType("QuantityObservation",  Namespace = "http://openiz.org/model"), JsonObject("QuantityObservation")]
    
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "QuantityObservation" )]
    public class QuantityObservation : Observation
    {

      
        /// <summary>
        /// Gets or sets the observed quantity
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public Decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the key of the uom concept
        /// </summary>
        [XmlElement("unitOfMeasure"), JsonProperty("unitOfMeasure")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? UnitOfMeasureKey
        {
            get { return this.UnitOfMeasure?.Key; }
            set
            {
                if (this.UnitOfMeasure?.Key != value)
                    this.UnitOfMeasure = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the unit of measure
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(UnitOfMeasureKey))]
		public Concept UnitOfMeasure { get; set; }

    }

    /// <summary>
    /// Represents an observation with a text value
    /// </summary>
    [XmlType("TextObservation",  Namespace = "http://openiz.org/model"), JsonObject("TextObservation")]
    
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "TextObservation")]
    public class TextObservation : Observation
    {
        /// <summary>
        /// Gets or sets the textual value
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }
    }

    /// <summary>
    /// Represents an observation with a concept value
    /// </summary>
    [XmlType("CodedObservation",  Namespace = "http://openiz.org/model"), JsonObject("CodedObservation")]
    
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "CodedObservation")]
    public class CodedObservation : Observation
    {

       
        /// <summary>
        /// Gets or sets the key of the uom concept
        /// </summary>
        [DataIgnore, XmlElement("value"), JsonProperty("value")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? ValueKey
        {
            get { return this.Value?.Key; }
            set
            {
                if (this.Value?.Key != value)
                    this.Value = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the coded value of the observation
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ValueKey))]
		public Concept Value { get; set; }

    }

}
