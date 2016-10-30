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
    /// Represents two-factor authentication mechanism information
    /// </summary>
    [XmlType(nameof(TfaMechanismInfo), Namespace = "http://openiz.org/ami")]
    [XmlRoot(nameof(TfaMechanismInfo), Namespace = "http://openiz.org/ami")]
    [JsonObject(nameof(TfaMechanismInfo))]
    public class TfaMechanismInfo
    {

        /// <summary>
        /// Default serialization ctor
        /// </summary>
        public TfaMechanismInfo()
        {

        }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the challenge text
        /// </summary>
        [XmlElement("challengeText"), JsonProperty("challengeText")]
        public String ChallengeText { get; set; }

    }
}
