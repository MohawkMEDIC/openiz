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

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a material 
    /// </summary>
    
    [XmlType("Material",  Namespace = "http://openiz.org/model"), JsonObject("Material")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Material")]
    public class Material : Entity
    {
       
        /// <summary>
        /// Material ctor
        /// </summary>
        public Material()
        {
            this.ClassConceptKey = EntityClassKeys.Material;
        }

        /// <summary>
        /// The base quantity of the object in the units. This differs from quantity on the relationship
        /// which is a /per ... 
        /// </summary>
        [XmlElement("quantity"), JsonProperty("quantity")]
        public Decimal? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the form concept's key
        /// </summary>
        [DataIgnore, XmlElement("formConcept"), JsonProperty("formConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? FormConceptKey
        {
            get { return this.FormConcept?.Key; }
            set
            {
                if (this.FormConcept?.Key != value)
                    this.FormConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the quantity concept ref
        /// </summary>
        [DataIgnore, XmlElement("quantityConcept"), JsonProperty("quantityConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? QuantityConceptKey
        {
            get { return this.QuantityConcept?.Key; }
            set
            {
                if (this.QuantityConcept?.Key != value)
                    this.QuantityConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(FormConceptKey))]
		public Concept FormConcept { get; set; }

        /// <summary>
        /// Gets or sets the concept which dictates the unit of measure for a single instance of this entity
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(QuantityConceptKey))]
		public Concept QuantityConcept { get; set; }


        /// <summary>
        /// Gets or sets the expiry date of the material
        /// </summary>
        [XmlElement("expiryDate"), JsonProperty("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// True if the material is simply administrative
        /// </summary>
        [XmlElement("isAdministrative"), JsonProperty("isAdministrative")]
        public Boolean IsAdministrative { get; set; }


    }
}
