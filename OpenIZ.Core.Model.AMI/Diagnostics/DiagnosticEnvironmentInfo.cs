using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
    /// <summary>
    /// Environment information
    /// </summary>
    [JsonObject(nameof(DiagnosticEnvironmentInfo)), XmlType(nameof(DiagnosticEnvironmentInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticEnvironmentInfo
    {
        /// <summary>
        /// Is platform 64 bit
        /// </summary>
        [JsonProperty("is64bit"), XmlAttribute("is64Bit")]
        public bool Is64Bit { get; set; }
        /// <summary>
        /// OS Version
        /// </summary>
        [JsonProperty("osVersion"), XmlAttribute("osVersion")]
        public String OSVersion { get; set; }
        /// <summary>
        /// CPU count
        /// </summary>
        [JsonProperty("processorCount"), XmlAttribute("processorCount")]
        public int ProcessorCount { get; set; }
        /// <summary>
        /// Used memory
        /// </summary>
        [JsonProperty("usedMem"), XmlElement("mem")]
        public long UsedMemory { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        [JsonProperty("version"), XmlElement("version")]
        public String Version { get; set; }
    }

}
