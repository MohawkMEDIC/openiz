using System;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a single preferred communication method for the entity
    /// </summary>
    [Serializable]
    [DataContract(Name = "PersonLanguageCommunication", IsReference = false, Namespace ="http://openiz.org/model")]
    public class PersonLanguageCommunication : VersionBoundRelationData<Entity>
    {

        /// <summary>
        /// Gets or sets the language code
        /// </summary>
        [DataMember(Name = "languageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or set the user's preference indicator
        /// </summary>
        [DataMember(Name = "isPreferred")]
        public bool IsPreferred { get; set; }

    }
}