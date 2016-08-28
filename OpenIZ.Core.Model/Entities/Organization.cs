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

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Organization entity
    /// </summary>
    
    [XmlType("Organization",  Namespace = "http://openiz.org/model"), JsonObject("Organization")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Organization")]
    public class Organization : Entity
    {

        // Industry concept
        private Guid? m_industryConceptKey;
        // Industry Concept
        
        private Concept m_industryConcept;

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
        [XmlElement("industryConcept"), JsonProperty("industryConcept")]
        public Guid? IndustryConceptKey
        {
            get { return this.m_industryConceptKey; }
            set
            {
                if (this.m_industryConceptKey != value)
                {
                    this.m_industryConceptKey = value;
                    this.m_industryConcept = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the industry in which the organization operates
        /// </summary>
        [SerializationReference(nameof(IndustryConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept IndustryConcept
        {
            get {
                this.m_industryConcept = base.DelayLoad(this.m_industryConceptKey, this.m_industryConcept);
                return this.m_industryConcept;
            }
            set {
                this.m_industryConcept = value;
                this.m_industryConceptKey = value?.Key;
            }
        }


        /// <summary>
        /// Forces reload of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_industryConcept = null;
        }
    }
}
