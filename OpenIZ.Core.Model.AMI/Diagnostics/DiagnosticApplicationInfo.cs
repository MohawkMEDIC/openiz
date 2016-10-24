using Newtonsoft.Json;
using OpenIZ.Core.Applets.Model;
using OpenIZ.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Diagnostics
{
   
    /// <summary>
    /// Application information
    /// </summary>
    [JsonObject(nameof(DiagnosticApplicationInfo)), XmlType(nameof(DiagnosticApplicationInfo), Namespace = "http://openiz.org/ami/diagnostics")]
    public class DiagnosticApplicationInfo : DiagnosticVersionInfo
    {
        private Tracer m_tracer = Tracer.GetTracer(typeof(DiagnosticApplicationInfo));

        /// <summary>
        /// Diagnostic application information
        /// </summary>
        public DiagnosticApplicationInfo()
        {

        }

        /// <summary>
        /// Creates new diagnostic application information
        /// </summary>
        /// <param name="versionInfo"></param>
        public DiagnosticApplicationInfo(Assembly versionInfo) : base(versionInfo)
        {

        }
        
        /// <summary>
        /// Environment information
        /// </summary>
        [JsonProperty("environment"), XmlElement("environment")]
        public DiagnosticEnvironmentInfo EnvironmentInfo { get; set; }

        /// <summary>
        /// Open IZ information
        /// </summary>
        [JsonProperty("openiz"), XmlElement("openiz")]
        public DiagnosticVersionInfo OpenIZ { get; set; }

        /// <summary>
        /// Gets or sets the assemblies
        /// </summary>
        [JsonProperty("assembly"), XmlElement("assembly")]
        public List<DiagnosticVersionInfo> Assemblies { get; set; }

        /// <summary>
        /// Gets or sets the applets
        /// </summary>
        [JsonProperty("applet"), XmlElement("applet")]
        public List<AppletInfo> Applets { get; set; }

        /// <summary>
        /// Gets or sets file info
        /// </summary>
        [JsonProperty("fileInfo"), XmlElement("fileInfo")]
        public List<DiagnosticAttachmentInfo> FileInfo { get; set; }

        /// <summary>
        /// Gets the sync info
        /// </summary>
        [JsonProperty("syncInfo"), XmlElement("syncInfo")]
        public List<DiagnosticSyncInfo> SyncInfo { get; set; }

    }
    
   
   

}
