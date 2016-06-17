using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model.Security
{
    /// <summary>
    /// Submission request
    /// </summary>
    [XmlType(nameof(SubmissionRequest), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(SubmissionRequest), Namespace = "http://openiz.org/ami")]
    public class SubmissionRequest
    {

        /// <summary>
        /// Gets or sets the cmc request
        /// </summary>
        [XmlElement("cmc")]
        public String CmcRequest { get; set; }

        /// <summary>
        /// Gets or sets the contact name
        /// </summary>
        [XmlElement("contact")]
        public String AdminContactName { get; set; }

        /// <summary>
        /// Gets or sets the admin address
        /// </summary>
        [XmlElement("address")]
        public String AdminAddress { get; set; }
    }
}
