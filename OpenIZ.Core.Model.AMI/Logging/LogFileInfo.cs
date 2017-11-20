using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Logging
{
    /// <summary>
    /// Log file information
    /// </summary>
    [XmlRoot(nameof(LogFileInfo), Namespace = "http://openiz.org/ami")]
    [JsonObject(nameof(LogFileInfo))]
    public class LogFileInfo 
    {

        /// <summary>
        /// The key of the logfile
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the size of the file
        /// </summary>
        [XmlElement("size"), JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the last write time
        /// </summary>
        [XmlElement("modified"), JsonProperty("modified")]
        public DateTime LastWrite { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        [XmlElement("text"), JsonProperty("text")]
        public byte[] Contents { get; set; }
    }
}
