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
    /// Represents an encounter a patient has with the health system
    /// </summary>
    ///<remarks>
    ///<para>An encounter is a special type of act which represents an episode of care which a patient experiences with the health system. 
    ///An encounter is used to document things like hospital visits, inpatient care encounters, or any longer running series of actions which 
    ///are linked by the admit -&gt; discharge workflow.</para>
    /// </remarks>
    [XmlType("PatientEncounter",  Namespace = "http://openiz.org/model"), JsonObject("PatientEncounter")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "PatientEncounter")]
    public class PatientEncounter : Act
    {

        // Disposition key
        private Guid? m_dischargeDispositionKey;
        // Disposition
        private Concept m_dischargeDisposition;

        /// <summary>
        /// Patient encounter ctor
        /// </summary>
        public PatientEncounter()
        {
            base.ClassConceptKey = ActClassKeys.Encounter;
        }

        /// <summary>
        /// Gets or sets the key of discharge disposition
        /// </summary>
        [AutoLoad, EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("dischargeDisposition"), JsonProperty("dischargeDisposition")]
        public Guid? DischargeDispositionKey
        {
            get { return this.m_dischargeDispositionKey; }
            set
            {
                if (this.m_dischargeDispositionKey != value)
                {
                    this.m_dischargeDispositionKey = value;
                    this.m_dischargeDisposition = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the discharge disposition (how the patient left the encounter
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(DischargeDispositionKey))]
        public Concept DischargeDisposition
        {
            get
            {
                this.m_dischargeDisposition = base.DelayLoad(this.m_dischargeDispositionKey, this.m_dischargeDisposition);
                return this.m_dischargeDisposition;
            }
            set
            {
                this.m_dischargeDisposition = value;
                this.m_dischargeDispositionKey = value?.Key;
            }
        }

        /// <summary>
        /// Refresh forcing delay load
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_dischargeDisposition = null;
        }

        /// <summary>
        /// Semantic equality function
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as PatientEncounter;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.DischargeDispositionKey == this.DischargeDispositionKey;
        }
    }
}
