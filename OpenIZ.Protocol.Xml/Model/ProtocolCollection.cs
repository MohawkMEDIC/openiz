using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Protocol collection
    /// </summary>
    [XmlType(nameof(ProtocolCollection), Namespace = "http://openiz.org/protocol")]
    public class ProtocolCollection : BaseProtocolElement
    {

        /// <summary>
        /// Loads the protocol collection from a stream
        /// </summary>
        public static ProtocolCollection Load(Stream s)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ProtocolCollection));
            return xsz.Deserialize(s) as ProtocolCollection;
        }

        /// <summary>
        /// Gets or sets the protocol definitions
        /// </summary>
        [XmlElement("protocol")]
        public List<ProtocolDefinition> Protocols{ get; set; }
    }
}
