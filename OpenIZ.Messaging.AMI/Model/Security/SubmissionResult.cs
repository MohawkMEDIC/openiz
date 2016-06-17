using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// Represents the submission result
    /// </summary>
    [XmlType(nameof(SubmissionResult), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(SubmissionResult), Namespace = "http://openiz.org/ami")]

    public class SubmissionResult
    {


        public SubmissionResult()
        {

        }
        /// <summary>
        /// Creates a new client certificate request result based on the internal request response
        /// </summary>
        /// <param name="certificateRequestResponse"></param>
        public SubmissionResult(MARC.Util.CertificateTools.CertificateRequestResponse certificateRequestResponse)
        {
            this.Message = certificateRequestResponse.Message;
            this.RequestId = certificateRequestResponse.RequestId;
            this.Status = (SubmissionStatus)certificateRequestResponse.Outcome;
            this.Certificate = certificateRequestResponse.AuthorityResponse;
        }

        /// <summary>
        /// Gets or sets the message from the server
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the certificate content
        /// </summary>
        [XmlElement("pkcs")]
        public string Certificate { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        [XmlAttribute("status")]
        public SubmissionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the request id
        /// </summary>
        [XmlAttribute("id")]
        public int RequestId { get; set; }

    }
}
