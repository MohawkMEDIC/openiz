using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
    /// <summary>
    /// Application version information
    /// </summary>
    [JsonObject(nameof(DiagnosticVersionInfo)), XmlType(nameof(DiagnosticVersionInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticVersionInfo
    {
        /// <summary>
        /// Diagnostic version information
        /// </summary>
        public DiagnosticVersionInfo()
        {

        }
        /// <summary>
        /// Version information
        /// </summary>
        public DiagnosticVersionInfo(Assembly asm)
        {
            if (asm == null) return;
            this.Version = asm.GetName().Version.ToString();
            this.InformationalVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            this.Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            this.Product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            this.Name = asm.GetName().Name;
            this.Info = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        }


        [JsonProperty("version"), XmlAttribute("version")]
        public String Version { get; set; }
        [JsonProperty("infoVersion"), XmlAttribute("infoVersion")]
        public String InformationalVersion { get; set; }
        [JsonProperty("copyright"), XmlAttribute("copyright")]
        public String Copyright { get; set; }
        [JsonProperty("product"), XmlAttribute("product")]
        public String Product { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name"), XmlAttribute("name")]
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the informational value
        /// </summary>
        [JsonProperty("info"), XmlElement("description")]
        public String Info { get; set; }
    }

}
