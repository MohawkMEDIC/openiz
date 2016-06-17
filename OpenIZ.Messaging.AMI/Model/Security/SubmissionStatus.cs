using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// Resubmission outcome
    /// </summary>
    [XmlType(nameof(SubmissionStatus), Namespace = "http://openiz.org/ami")]
    public enum SubmissionStatus
    {
        [XmlEnum("NOT COMPLETE")]
        NotComplete = 0x0,
        [XmlEnum("ERROR")]
        Failed = 0x1,
        [XmlEnum("DENIED")]
        Denied = 0x2,
        [XmlEnum("ISSUED")]
        Issued = 0x3,
        [XmlEnum("ISSUED SEPERATELY")]
        IssuedSeperately = 0x4,
        [XmlEnum("SUBMITTED")]
        Submission = 0x5,
        [XmlEnum("REVOKED")]
        Revoked = 0x6
    }
}
