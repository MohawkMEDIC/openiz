using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// The reason something is revoked
    /// </summary>
    [XmlType(nameof(RevokeReason), Namespace = "http://openiz.org/ami")]
    public enum RevokeReason : uint
    {
        [XmlEnum("UNSPECIFIED")]
        Unspecified = 0x0,
        [XmlEnum("KEY COMPROMISED")]
        KeyCompromise = 0x1,
        [XmlEnum("CA COMPROMISED")]
        CaCompromise = 0x2,
        [XmlEnum("AFFILIATION CHANGED")]
        AffiliationChange = 0x3,
        [XmlEnum("SUPERSEDED")]
        Superseded = 0x4,
        [XmlEnum("CESSATION OF OPERATION")]
        CessationOfOperation = 0x5,
        [XmlEnum("CERTIFICATE ON HOLD")]
        CertificateHold = 0x6,
        [XmlEnum("REINSTANTIATE")]
        Reinstate = 0xFFFFFFFF
    }
}
