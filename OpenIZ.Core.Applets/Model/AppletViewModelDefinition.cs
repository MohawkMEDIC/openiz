using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents a view model definition
    /// </summary>
    [XmlType(nameof(AppletViewModelDefinition), Namespace = "http://openiz.org/applet")]
    public class AppletViewModelDefinition
    {

        /// <summary>
        /// Gets or sets the mnemonic
        /// </summary>
        [XmlAttribute("key")]
        public String ViewModelId { get; set; }

        /// <summary>
        /// Gets or sets the definition
        /// </summary>
        [XmlElement("definition")]
        public String Definition { get; set; }

        /// <summary>
        /// The content loaded
        /// </summary>
        [XmlIgnore]
        public byte[] DefinitionContent { get; internal set; }

    }
}