using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a single preferred communication method for the entity
    /// </summary>
    [Serializable]
    [XmlType("PersonLanguageCommunication", Namespace ="http://openiz.org/model")]
    public class PersonLanguageCommunication : VersionBoundRelationData<Entity>
    {

        /// <summary>
        /// Gets or sets the language code
        /// </summary>
        [XmlElement("languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or set the user's preference indicator
        /// </summary>
        [XmlElement("isPreferred")]
        public bool IsPreferred { get; set; }

    }
}