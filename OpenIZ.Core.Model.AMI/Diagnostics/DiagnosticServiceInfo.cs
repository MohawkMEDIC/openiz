using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
    /// <summary>
    /// Represents diagnostic service info
    /// </summary>
	[JsonObject(nameof(DiagnosticServiceInfo)), XmlType(nameof(DiagnosticServiceInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticServiceInfo
    {

        public DiagnosticServiceInfo()
        {

        }

        /// <summary>
        /// Create the service info
        /// </summary>
        public DiagnosticServiceInfo(object daemon)
        {
            var dat = daemon?.GetType().GetTypeInfo().GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "DescriptionAttribute");
            this.Description = dat?.GetType().GetRuntimeProperty("Description")?.GetValue(dat)?.ToString();
            this.IsRunning = (bool)daemon.GetType().GetRuntimeProperty("IsRunning")?.GetValue(daemon);
            this.Type = daemon.GetType().FullName;
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [XmlElement("description"), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether the service is running
        /// </summary>
        [XmlElement("running"), JsonProperty("running")]
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [XmlElement("type"), JsonProperty("type")]
        public string Type { get; set; }

    }
}