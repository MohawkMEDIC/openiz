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
    /// Represents meta-data about a particular log
    /// </summary>
    [JsonObject(nameof(DiagnosticTextAttachment)), XmlType(nameof(DiagnosticTextAttachment), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticTextAttachment : DiagnosticAttachmentInfo
    {


        /// <summary>
        /// The content of the log file
        /// </summary>
        [XmlText, JsonProperty("text")]
        public String Content { get; set; }
        
    }

    /// <summary>
    /// Represents meta-data about a particular log
    /// </summary>
    [JsonObject(nameof(DiagnosticBinaryAttachment)), XmlType(nameof(DiagnosticBinaryAttachment), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticBinaryAttachment : DiagnosticAttachmentInfo
    {

        /// <summary>
        /// The content of the log file
        /// </summary>
        [XmlElement("data"), JsonProperty("data")]
        public byte[] Content { get; set; }

    }
}
