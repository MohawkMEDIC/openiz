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
    /// Runtime file inforamtion
    /// </summary>
    [JsonObject(nameof(DiagnosticAttachmentInfo)), XmlType(nameof(DiagnosticAttachmentInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticAttachmentInfo
    {


        /// <summary>
        /// Gets or sets the identiifer
        /// </summary>
        [XmlAttribute("id"), JsonProperty("id")]
        public String Id { get; set; }

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        [JsonProperty("file"), XmlAttribute("file")]
        public String FileName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description"), XmlElement("description")]
        public String FileDescription { get; set; }

        /// <summary>
        /// Size of the file
        /// </summary>
        [JsonProperty("size"), XmlAttribute("size")]
        public long FileSize { get; set; }

        /// <summary>
        /// Last write date
        /// </summary>
        [JsonProperty("lastWrite"), XmlAttribute("lastWrite")]
        public DateTime LastWriteDate { get; set; }
    }
}
