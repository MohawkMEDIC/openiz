using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Applet template definition
    /// </summary>
    [XmlType(nameof(AppletTemplateDefinition), Namespace = "http://openiz.org/applet")]
    public class AppletTemplateDefinition
    {
        /// <summary>
        /// Gets or sets the mnemonic
        /// </summary>
        [XmlAttribute("mnemonic")]
        public String Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the form
        /// </summary>
        [XmlElement("form")]
        public String Form { get; set; }

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
