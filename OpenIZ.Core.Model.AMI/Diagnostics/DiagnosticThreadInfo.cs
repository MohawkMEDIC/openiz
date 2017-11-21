using Newtonsoft.Json;
using System.Xml.Serialization;
using System;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{

    /// <summary>
    /// Diagnostic thread information
    /// </summary>
    [JsonObject(nameof(DiagnosticEnvironmentInfo)), XmlType(nameof(DiagnosticEnvironmentInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticThreadInfo
    {
        /// <summary>
        /// Gets or sets the time the CPU has been running
        /// </summary>
        [XmlElement("cpuTime"), JsonProperty("cpuTime")]
        public TimeSpan CpuTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Get or set the state
        /// </summary>
        [XmlElement("state"), JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets the task information
        /// </summary>
        [XmlAttribute("taskInfo"), JsonProperty("taskInfo")]
        public string TaskInfo { get; set; }

        /// <summary>
        /// Get or sets the wait reason
        /// </summary>
        [XmlAttribute("waitReason"), JsonProperty("waitReason")]
        public string WaitReason { get; set; }
    }
}