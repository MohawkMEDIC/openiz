using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// Certificate information
    /// </summary>
    [XmlRoot(nameof(X509Certificate2Info), Namespace = "http://openiz.org/ami")]
    [XmlType(nameof(X509Certificate2Info), Namespace = "http://openiz.org/ami")]
    public class X509Certificate2Info
    {

        public X509Certificate2Info()
        {

        }

        /// <summary>
        /// Constructs a certificate info
        /// </summary>
        /// <param name="cert"></param>
        public X509Certificate2Info(X509Certificate2 cert)
        {
            this.Issuer = cert.Issuer;
            this.NotBefore = cert.NotBefore;
            this.NotAfter = cert.NotAfter;
            this.Subject = cert.Subject;
            this.Thumbprint = cert.Thumbprint;
        }

        /// <summary>
        /// Create from a CA attribute set
        /// </summary>
        /// <param name="attributes"></param>
        public X509Certificate2Info(List<KeyValuePair<String, String>> attributes)
        {
            this.Id = Int32.Parse(attributes.First(o => o.Key == "RequestID").Value);
            this.Thumbprint = attributes.First(o => o.Key == "SerialNumber").Value;
            this.Subject = attributes.First(o => o.Key == "DistinguishedName").Value;
            this.NotAfter = DateTime.Parse(attributes.First(o => o.Key == "NotAfter").Value);
            this.NotBefore = DateTime.Parse(attributes.First(o => o.Key == "NotBefore").Value);
            this.Issuer = attributes.First(o => o.Key == "ccm").Value;
        }

        /// <summary>
        /// The identifier of the certificate
        /// </summary>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the issuers
        /// </summary>
        [XmlElement("iss")]
        public String Issuer { get; set; }

        /// <summary>
        /// Distinguished name
        /// </summary>
        [XmlElement("sub")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the issue date
        /// </summary>
        [XmlElement("nbf")]
        public DateTime NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the expiry date
        /// </summary>
        [XmlElement("exp")]
        public DateTime NotAfter { get; set; }

        /// <summary>
        /// Gets or sets the thumbprint
        /// </summary>
        [XmlElement("thumbprint")]
        public String Thumbprint { get; set; }


    }
}
