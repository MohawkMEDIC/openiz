using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// Enrollment information
    /// </summary>
    [XmlType(nameof(SubmissionInfo), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(SubmissionInfo), Namespace = "http://openiz.org/ami")]

    public class SubmissionInfo
    {
        /// <summary>
        /// RequestId
        /// </summary>
        [XmlAttribute("id")]
        public string RequestID { get; set; }
        /// <summary>
        /// Status code
        /// </summary>
        [XmlAttribute("status")]
        public SubmissionStatus XmlStatusCode { get; set; }
        /// <summary>
        /// Disposition message
        /// </summary>
        [XmlElement("message")]
        public string DispositionMessage { get; set; }
        /// <summary>
        /// Submitted on
        /// </summary>
        [XmlElement("submitted")]
        public string SubmittedWhen { get; set; }
        /// <summary>
        /// Resolved on
        /// </summary>
        [XmlElement("resolved")]
        public string ResolvedWhen { get; set; }
        /// <summary>
        /// Revoked on
        /// </summary>
        [XmlElement("revoked")]
        public string RevokedWhen { get; set; }
        /// <summary>
        /// Revokation reason
        /// </summary>
        [XmlElement("revokationReason")]
        public RevokeReason XmlRevokeReason { get; set; }
        /// <summary>
        /// Email address of user
        /// </summary>
        [XmlElement("email")]
        public string EMail { get; set; }
        /// <summary>
        /// Before date
        /// </summary>
        [XmlElement("notBefore")]
        public string NotBefore { get; set; }
        /// <summary>
        /// Expiry
        /// </summary>
        [XmlElement("notAfter")]
        public string NotAfter { get; set; }
        /// <summary>
        /// Administration contact
        /// </summary>
        [XmlElement("adminContact")]
        public string AdminContact { get; set; }
        /// <summary>
        /// Administration
        /// </summary>
        public String AdminSiteAddress { get; set; }
        /// <summary>
        /// Revoke reason from Keystore
        /// </summary>
        [XmlIgnore]
        public String RevokedReason
        {
            get { return ((int)this.XmlRevokeReason).ToString(); }
            set
            {
                if (value != null)
                    this.XmlRevokeReason = (RevokeReason)Int32.Parse(value);
            }
        }
        /// <summary>
        /// DN
        /// </summary>
        [XmlElement("dn")]
        public string DistinguishedName { get; set; }
    }
}
