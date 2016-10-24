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
    /// Remote sync info
    /// </summary>
    [JsonObject("RemoteSyncInfo"), XmlType(nameof(DiagnosticSyncInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticSyncInfo
    {
        [JsonProperty("resource"), XmlAttribute("resource")]
        public String ResourceName { get; set; }

        [JsonProperty("etag"), XmlAttribute("etag")]
        public String Etag { get; set; }

        [JsonProperty("lastSync"), XmlAttribute("lastSync")]
        public DateTime LastSync { get; set; }
        /// <summary>
        /// Filter used to sync
        /// </summary>
        [JsonProperty("filter"), XmlText]
        public String Filter { get; set; }
    }

}
