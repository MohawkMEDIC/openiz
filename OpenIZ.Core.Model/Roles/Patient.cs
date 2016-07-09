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
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Roles
{
    /// <summary>
    /// Represents an entity which is a patient
    /// </summary>
    
    [XmlType("Patient",  Namespace = "http://openiz.org/model"), JsonObject("Patient")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Patient")]
    public class Patient : Person
    {

        // Gender concept key
        private Guid? m_genderConceptKey;
        // Gender concept
        
        private Concept m_genderConcept;

        /// <summary>
        /// Represents a patient
        /// </summary>
        public Patient()
        {
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            base.ClassConceptKey = EntityClassKeys.Patient;
        }

        /// <summary>
        /// Gets or sets the date the patient was deceased
        /// </summary>
        [XmlElement("deceasedDate"), JsonProperty("deceasedDate")]
        public DateTime? DeceasedDate { get; set; }
        /// <summary>
        /// Gets or sets the precision of the date of deceased
        /// </summary>
        [XmlElement("deceasedDatePrecision"), JsonProperty("deceasedDatePrecision")]
        public DatePrecision? DeceasedDatePrecision { get; set; }
        /// <summary>
        /// Gets or sets the multiple birth order of the patient 
        /// </summary>
        [XmlElement("multipleBirthOrder"), JsonProperty("multipleBirthOrder")]
        public int? MultipleBirthOrder { get; set; }

        /// <summary>
        /// Gets or sets the gender concept key
        /// </summary>
        [XmlElement("genderConcept"), JsonProperty("genderConcept")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? GenderConceptKey
        {
            get { return this.m_genderConceptKey; }
            set
            {
                this.m_genderConceptKey = value;
                this.m_genderConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the gender concept
        /// </summary>
        [DelayLoad(nameof(GenderConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept GenderConcept
        {
            get
            {
                this.m_genderConcept = base.DelayLoad(this.m_genderConceptKey, this.m_genderConcept);
                return this.m_genderConcept;
            }
            set
            {
                this.m_genderConcept = value;
                this.m_genderConceptKey = value?.Key;
            }
        }
        
        /// <summary>
        /// Force a refresh of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_genderConcept = null;
            
        }
    }
}
