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
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents an act whereby a substance is administered to the patient
    /// </summary>
    /// <remarks>
    /// <para>The substance administration act is used whenever a clinician administers, plans to administer or should administer to a patient, a substance. 
    /// The substance that is administered is open but should be represented as either a Consumable (something that was consumed in the act of administration 
    /// like a manufactured material (<see cref="ManufacturedMaterial"/>) or a product (if proposing or planning)).</para>
    /// <para>
    /// The type of administration (immunization, drug therapy, treatment, etc.) is classified by the substance administration's type concept (<see cref="Act.TypeConceptKey"/>). In some cases
    /// the dose quantity or dose measure are not required (when giving just "a dose") however it is recommended that implementations accurately track
    /// how much of the substance was administered.
    /// </para>
    /// </remarks>
    [XmlType("SubstanceAdministration",  Namespace = "http://openiz.org/model"), JsonObject("SubstanceAdministration")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SubstanceAdministration")]
    public class SubstanceAdministration : Act
    {
        // Route key
        private Guid? m_routeKey;
        // Dose unit key
        private Guid? m_doseUnitKey;
        // Route
        private Concept m_route;
        // Dose unit
        private Concept m_doseUnit;
        private Concept m_site;
        private Guid? m_siteKey;

        /// <summary>
        /// Substance administration ctor
        /// </summary>
        public SubstanceAdministration()
        {
            base.ClassConceptKey = ActClassKeys.SubstanceAdministration;
        }

        /// <summary>
        /// Gets or sets the key for route
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("route"), JsonProperty("route")]
        public Guid? RouteKey
        {
            get { return this.m_routeKey; }
            set
            {
                if (this.m_routeKey != value)
                {
                    this.m_routeKey = value;
                    this.m_route = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the key for dosing unit
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("doseUnit"), JsonProperty("doseUnit")]
        public Guid? DoseUnitKey
        {
            get { return this.m_doseUnitKey; }
            set
            {
                if (this.m_doseUnitKey != value)
                {
                    this.m_doseUnitKey = value;
                    this.m_doseUnit = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a concept which indicates the route of administration (eg: Oral, Injection, etc.)
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(RouteKey))]
        public Concept Route
        {
            get
            {
                this.m_route = base.DelayLoad(this.m_routeKey, this.m_route);
                return this.m_route;
            }
            set
            {
                this.m_route = value;
                this.m_routeKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets a concept which indicates the unit of measure for the dose (eg: 5 mL, 10 mL, 1 drop, etc.)
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(DoseUnitKey))]
        public Concept DoseUnit
        {
            get
            {
                this.m_doseUnit = base.DelayLoad(this.m_doseUnitKey, this.m_doseUnit);
                return this.m_doseUnit;
            }
            set
            {
                this.m_doseUnit = value;
                this.m_doseUnitKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the amount of substance administered
        /// </summary>
        [XmlElement("doseQuantity"), JsonProperty("doseQuantity")]
        public Decimal DoseQuantity { get; set; }

        /// <summary>
        /// The sequence of the dose (i.e. OPV 0 = 0 , OPV 1 = 1, etc.)
        /// </summary>
        [XmlElement("doseSequence"), JsonProperty("doseSequence")]
        public int SequenceId { get; set; }


        /// <summary>
        /// Gets or sets the site
        /// </summary>
        [XmlElement("site"), JsonProperty("site")]
        public Guid? SiteKey
        {
            get { return this.m_siteKey; }
            set
            {
                if (this.m_siteKey != value)
                {
                    this.m_siteKey = value;
                    this.m_site = null;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a concept which indicates the site of administration
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(SiteKey))]
        public Concept Site
        {
            get
            {
                this.m_site = base.DelayLoad(this.m_siteKey, this.m_site);
                return this.m_site;
            }
            set
            {
                this.m_site = value;
                this.m_siteKey = value?.Key;
            }
        }

        /// <summary>
        /// Force delay loading of properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_doseUnit = this.m_route = null;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as SubstanceAdministration;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.SiteKey == this.SiteKey &&
                other.RouteKey == this.RouteKey &&
                other.DoseUnitKey == this.DoseUnitKey &&
                other.DoseQuantity == this.DoseQuantity &&
                other.SequenceId == this.SequenceId;
        }

        /// <summary>
        /// Should serialize site key
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSiteKey ()
        {
            return this.SiteKey.HasValue;
        }
    }
}
