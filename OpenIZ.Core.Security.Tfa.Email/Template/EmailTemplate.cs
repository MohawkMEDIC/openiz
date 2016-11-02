using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Security.Tfa.Email.Template
{
    /// <summary>
    /// E-mail template
    /// </summary>
    [XmlType(nameof(EmailTemplate), Namespace = "http://openiz.org/tfa/email/template")]
    [XmlRoot(nameof(EmailTemplate), Namespace = "http://openiz.org/tfa/email/template")]
    public class EmailTemplate
    {

        /// <summary>
        /// Gets or sets the from element
        /// </summary>
        [XmlElement("from")]
        public String From { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [XmlElement("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [XmlElement("body")]
        public String Body { get; set; }

        /// <summary>
        /// Load the specified e-mail template
        /// </summary>
        public static EmailTemplate Load(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            {
                XmlSerializer xsz = new XmlSerializer(typeof(EmailTemplate));
                return xsz.Deserialize(fs) as EmailTemplate;
            }
        }
    }
}
