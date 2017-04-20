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
    public class CarePlan : Bundle
    {

        /// <summary>
        /// Create a care plan request
        /// </summary>
        public static CarePlan CreateCarePlanRequest(Patient p)
        {
            return new CarePlan() { Item = new List<IdentifiedData>() { p });
        }
    }
}
