using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.AMI.Auth
{
    /// <summary>
    /// Security ticket information
    /// </summary>
    [XmlType(nameof(TfaRequestInfo), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(TfaRequestInfo), Namespace = "http://openiz.org/ami")]
    [JsonObject(nameof(TfaRequestInfo))]
    public class TfaRequestInfo
    {
        /// <summary>
        /// Gets or sets the user key
        /// </summary>
        [XmlElement("mechanism"), JsonProperty("mechanism")]
        public Guid ResetMechanism { get; set; }

        /// <summary>
        /// The verification (usually the phone number or e-mail address of the user)
        /// </summary>
        [XmlElement("verification"), JsonProperty("verification")]
        public String Verification { get; set; }


        /// <summary>
        /// The verification (usually the phone number or e-mail address of the user)
        /// </summary>
        [XmlElement("username"), JsonProperty("username")]
        public String UserName { get; set; }

        /// <summary>
        /// The scope or purpose of the TFA secret
        /// </summary>
        [XmlElement("purpose"), JsonProperty("purpose")]
        public String Purpose { get; set; }
    }
}
