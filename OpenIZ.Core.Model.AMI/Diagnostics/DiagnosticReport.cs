using Newtonsoft.Json;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
    /// <summary>
    /// Diagnostics report
    /// </summary>
    [JsonObject(nameof(DiagnosticReport)), XmlType(nameof(DiagnosticReport), Namespace = "http://openiz.org/ami/diagnostics")]
    [XmlRoot(nameof(DiagnosticReport), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticReport : BaseEntityData
    {
        /// <summary>
        /// Gets or sets the note
        /// </summary>
        [XmlElement("note"), JsonProperty("note")]
        public String Note { get; set; }

        /// <summary>
        /// Represents the submitter
        /// </summary>
        [XmlElement("submitter"), JsonProperty("submitter")]
        public UserEntity Submitter { get; set; }

        /// <summary>
        /// Represents the most recent logs for the bug report
        /// </summary>
        [XmlElement("attachText", typeof(DiagnosticTextAttachment)), JsonProperty("attach")]
        [XmlElement("attachBin", typeof(DiagnosticBinaryAttachment))]
        public List<DiagnosticAttachmentInfo> Attachments { get; set; }

        /// <summary>
        /// Application configuration information
        /// </summary>
        [XmlElement("appInfo"), JsonProperty("appInfo")]
        public DiagnosticApplicationInfo ApplicationInfo { get; set; }

    }
}
