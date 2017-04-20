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
