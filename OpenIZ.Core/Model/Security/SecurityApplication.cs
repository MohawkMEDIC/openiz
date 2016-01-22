using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Policy;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Security
{
    /// <summary>
    /// Represents a security application
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "SecurityApplication")]
    [XmlType("SecurityApplication", Namespace = "http://openiz.org/model")]
    public class SecurityApplication : SecurityEntity
    {
        // Backing field
        private List<SecurityPolicyInstance> m_policies;

        /// <summary>
        /// Gets or sets the application secret used for authenticating the application
        /// </summary>
        [XmlElement("applicationSecret")]
        public String ApplicationSecret { get; set; }

    }
}
