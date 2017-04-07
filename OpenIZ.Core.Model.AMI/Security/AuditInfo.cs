using MARC.HI.EHRS.SVC.Auditing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Security
{
    /// <summary>
    /// Audit information which wraps audit data from service core
    /// </summary>
    [XmlRoot("audit", Namespace = "http://openiz.org/ami")]
    [XmlType(nameof(AuditInfo), Namespace = "http://openiz.org/ami")]
    public class AuditInfo : IdentifiedData
    {

        /// <summary>
        /// Audit information
        /// </summary>
        public AuditInfo()
        {

        }

        /// <summary>
        /// Creates a new instance of audit information
        /// </summary>
        public AuditInfo(AuditData audit)
        {
            this.Audit = audit;
        }

        /// <summary>
        /// Gets the process identifier of the mobile application
        /// </summary>
        [XmlElement("pid")]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the device identifier which sent the audit
        /// </summary>
        [XmlElement("deviceId")]
        public Guid SecurityDeviceId { get; set; }

        /// <summary>
        /// Gets or sets the audit
        /// </summary>
        [XmlElement("audit")]
        public AuditData Audit { get; set; }

        /// <summary>
        /// When was the audit modified
        /// </summary>
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return this.Audit.Timestamp;
            }
        }
    }
}
