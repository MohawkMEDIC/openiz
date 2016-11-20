using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Interop
{
    /// <summary>
    /// Service resource options
    /// </summary>
    [XmlType(nameof(ServiceResourceOptions), Namespace = "http://openiz.org/model"), JsonObject(nameof(ServiceResourceOptions))]
    public class ServiceResourceOptions
    {
        /// <summary>
        /// Gets the name of the resource
        /// </summary>
        [XmlAttribute("resource"), JsonProperty("resource")]
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the verbs supported on the specified resource
        /// </summary>
        [XmlElement("verb"), JsonProperty("verb")]
        public List<string> Verbs { get; set; }
    }
}