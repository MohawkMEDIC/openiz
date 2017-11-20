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

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents a act (<see cref="Act"/>) which is an observation
    /// </summary>
    /// <remarks>
    /// <para>
    /// The observation class itself is an abstract class which is generically used to represent something that is observed about a patient.
    /// </para>
    /// <para>
    /// It is not recommended to use this class directly, rather one of its sub classes based on the type of observation being made such as:
    /// </para>
    /// <list type="bullet">
    ///     <item>Coded observation (<see cref="CodedObservation"/>) for observations whose values are codified (example: blood type, presentation, etc.), </item>
    ///     <item>Quantity observations (<see cref="QuantityObservation"/>) for observations whose values are quantified values (example: weight, height, etc.), </item>
    ///     <item>Text observations (<see cref="TextObservation"/>) for observations whose values are textual in nature.</item>
    /// </list>
    /// <para>
    /// No matter what type of value an observation carries (coded, quantity, text) it is always classified by the type concept (<see cref="Act.TypeConceptKey"/>).
    /// </para>
    /// </remarks>
    [XmlType("Observation",  Namespace = "http://openiz.org/model"), JsonObject("Observation")]
    
    public class Observation : Act
    {

        // Interpreation concept key
        private Guid? m_interpretationConceptKey;
        // Interpretation concept
        private Concept m_interpretationConcept;

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
            get { return this.m_interpretationConceptKey; }
            set
            {
                if (this.m_interpretationConceptKey != value)
                {
                    this.m_interpretationConceptKey = value;
                    this.m_interpretationConcept = null;
                }
            }
        }


        /// <summary>
        /// Value type
        /// </summary>
        [XmlElement("valueType"), JsonProperty("valueType")]
        public virtual String ValueType { get { return "NA"; } set { } }

        /// <summary>
        /// Gets or sets the concept which indicates the interpretation of the observtion
        /// </summary>
        [AutoLoad, SerializationReference(nameof(InterpretationConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept InterpretationConcept
        {
            get {
                this.m_interpretationConcept = base.DelayLoad(this.m_interpretationConceptKey, this.m_interpretationConcept);
                return this.m_interpretationConcept;
            }
            set
            {
                this.m_interpretationConcept = value;
                this.m_interpretationConceptKey = value?.Key;
            }
        }

        /// <summary>
        /// Refresh the object forcing delay load 
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_interpretationConcept = null;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as Observation;
            if (other == null) return false;
            return base.SemanticEquals(obj) && this.InterpretationConceptKey == other.InterpretationConceptKey;
        }

        /// <summary>
        /// Should serialize value type?
        /// </summary>
        public bool ShouldSerializeValueType() => false;
    }

    /// <summary>
    /// Represents an observation that contains a quantity
    /// </summary>
    /// <remarks>
    /// The quantity observation class should be used whenever you wish to store an observation which carries a numerical value 
    /// and an optional unit of measure (example: length = 3.2 ft, weight = 1.2 kg, etc.)
    /// </remarks>
    [XmlType("QuantityObservation",  Namespace = "http://openiz.org/model"), JsonObject("QuantityObservation")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "QuantityObservation" )]
    public class QuantityObservation : Observation
    {

        // UOM key
        private Guid? m_unitOfMeasureKey;
        // UOM
        private Concept m_unitOfMeasure;

        /// <summary>
        /// Gets or sets the observed quantity
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public Decimal Value { get; set; }

        /// <summary>
        /// Value type
        /// </summary>
        [XmlElement("valueType"), JsonProperty("valueType")]
        public override string ValueType
        {
            get
            {
                return "PQ";
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the key of the uom concept
        /// </summary>
        [XmlElement("unitOfMeasure"), JsonProperty("unitOfMeasure")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? UnitOfMeasureKey
        {
            get { return this.m_unitOfMeasureKey; }
            set
            {
                if (this.m_unitOfMeasureKey != value)
                {
                    this.m_unitOfMeasureKey = value;
                    this.m_unitOfMeasure = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the unit of measure
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(UnitOfMeasureKey))]
        public Concept UnitOfMeasure
        {
            get
            {
                this.m_unitOfMeasure = base.DelayLoad(this.m_unitOfMeasureKey, this.m_unitOfMeasure);
                return this.m_unitOfMeasure;
            }
            set
            {
                this.m_unitOfMeasure = value;
                this.m_unitOfMeasureKey = value?.Key;
            }
        }

        /// <summary>
        /// Forces a refresh of the object
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_unitOfMeasure = null;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as QuantityObservation;
            if (other == null) return false;
            return base.SemanticEquals(obj) && this.Value == other.Value && this.UnitOfMeasureKey == other.UnitOfMeasureKey;
        }
    }

    /// <summary>
    /// Represents an observation with a text value
    /// </summary>
    /// <remarks>
    /// The text observation type represents an observation made with a textual value. This is done whenever an observation type 
    /// cannot be quantified or classified using either a coded or observed value. Please note that this type should not be used
    /// for taking notes, rather it is a specific type of thing observed about a patient. For example: Interpretation of patient's mood
    /// </remarks>
    [XmlType("TextObservation",  Namespace = "http://openiz.org/model"), JsonObject("TextObservation")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "TextObservation")]
    public class TextObservation : Observation
    {

        /// <summary>
        /// Value type
        /// </summary>
        [XmlElement("valueType"), JsonProperty("valueType")]
        public override string ValueType
        {
            get
            {
                return "ED";
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the textual value
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as TextObservation;
            if (other == null) return false;
            return base.SemanticEquals(obj) && this.Value == other.Value;
        }
    }

    /// <summary>
    /// Represents an observation with a concept value
    /// </summary>
    /// <remarks>
    /// A coded observation represents an observation whose value is classified using a coded concept. For example: fetal presentation, 
    /// stage of pregnancy, etc.
    /// </remarks>
    [XmlType("CodedObservation",  Namespace = "http://openiz.org/model"), JsonObject("CodedObservation")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "CodedObservation")]
    public class CodedObservation : Observation
    {

        // Value key
        private Guid? m_valueKey;
        // Value
        private Concept m_value;

        /// <summary>
        /// Value type
        /// </summary>
        [XmlElement("valueType"), JsonProperty("valueType")]
        public override string ValueType
        {
            get
            {
                return "CD";
            }
            set { }
        }

        /// <summary>
        /// Gets or sets the key of the uom concept
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? ValueKey
        {
            get { return this.m_valueKey; }
            set
            {
                if (this.m_valueKey != value)
                {
                    this.m_valueKey = value;
                    this.m_value = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the coded value of the observation
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(ValueKey))]
        public Concept Value
        {
            get
            {
                this.m_value = base.DelayLoad(this.m_valueKey, this.m_value);
                return this.m_value;
            }
            set
            {
                this.m_value = value;
                this.m_valueKey = value?.Key;
            }
        }

        /// <summary>
        /// Forces a refresh of underlying data
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_value = null;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as CodedObservation;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.ValueKey == this.ValueKey;
        }
    }

}
