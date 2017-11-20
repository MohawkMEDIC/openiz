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
 * Date: 2017-4-22
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents a care plan
    /// </summary>
    /// <remarks>
    /// The care plan object is used to represent a collection of clinical protocols which the care planning
    /// engine proposes should be done as part of the patient's course of care.
    /// </remarks>
    [XmlType(nameof(CarePlan), Namespace = "http://openiz.org/model")]
    [XmlRoot(nameof(CarePlan), Namespace = "http://openiz.org/model")]
    [JsonObject(nameof(CarePlan))]
    public class CarePlan : BaseEntityData
    {
                
        /// <summary>
        /// Target of the careplan
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public Patient Target { get; set; }

        /// <summary>
        /// Action to take
        /// </summary>
        [XmlElement("act", typeof(Act))]
        [XmlElement("substanceAdministration", typeof(SubstanceAdministration))]
        [XmlElement("quantityObservation", typeof(QuantityObservation))]
        [XmlElement("codedObservation", typeof(CodedObservation))]
        [XmlElement("textObservation", typeof(TextObservation))]
        [XmlElement("patientEncounter", typeof(PatientEncounter))]
        [JsonProperty("act")]
        public List<Act> Action { get; set; }

        /// <summary>
        /// Default ctor
        /// </summary>
        public CarePlan()
        {
            this.Action = new List<Act>();
            this.Key = Guid.NewGuid();
            this.CreationTime = DateTime.Now;
        }

        /// <summary>
        /// Create care plan with acts
        /// </summary>
        public CarePlan(Patient p, IEnumerable<Act> acts) : this()
        {
            this.Action = acts.ToList();
            this.Target = p;
        }

        /// <summary>
        /// Create a care plan request
        /// </summary>
        public static CarePlan CreateCarePlanRequest(Patient p)
        {
            return new CarePlan() { Target= p  };
        }
    }
}
