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
    /// Organization entity
    /// </summary>
    
    [XmlType("Organization",  Namespace = "http://openiz.org/model"), JsonObject("Organization")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Organization")]
    public class Organization : Entity
    {

       
        /// <summary>
        /// Organization ctor
        /// </summary>
        public Organization()
        {
            this.DeterminerConceptKey = DeterminerKeys.Specific;
            this.ClassConceptKey = EntityClassKeys.Organization;
        }

        /// <summary>
        /// Gets or sets the industry concept key
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("industryConcept"), JsonProperty("industryConcept")]
        public Guid? IndustryConceptKey
        {
            get { return this.IndustryConcept?.Key; }
            set
            {
                if (this.IndustryConcept?.Key != value)
                    this.IndustryConcept = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the industry in which the organization operates
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(IndustryConceptKey))]
		public Concept IndustryConcept { get; set; }

    }
}
