using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a single preferred communication method for the entity
    /// </summary>
    
    [XmlType("PersonLanguageCommunication",  Namespace ="http://openiz.org/model"), JsonObject("PersonLanguageCommunication")]
    public class PersonLanguageCommunication : VersionedAssociation<Entity>
    {

        /// <summary>
        /// Gets or sets the language code
        /// </summary>
        [XmlElement("languageCode"), JsonProperty("languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or set the user's preference indicator
        /// </summary>
        [XmlElement("isPreferred"), JsonProperty("isPreferred")]
        public bool IsPreferred { get; set; }

    }
}