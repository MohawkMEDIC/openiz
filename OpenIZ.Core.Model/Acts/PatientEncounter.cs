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
    /// Represents an encounter a patient has with the health system
    /// </summary>
    
    [XmlType("PatientEncounter",  Namespace = "http://openiz.org/model"), JsonObject("PatientEncounter")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "PatientEncounter")]
    public class PatientEncounter : Act
    {


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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("dischargeDisposition"), JsonProperty("dischargeDisposition")]
        public Guid? DischargeDispositionKey
        {
            get { return this.DischargeDisposition?.Key; }
            set
            {
                if (this.DischargeDisposition?.Key != value)
                    this.DischargeDisposition = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the discharge disposition (how the patient left the encounter
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(DischargeDispositionKey))]
		public Concept DischargeDisposition { get; set; }


    }
}
