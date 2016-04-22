using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.AMI.Model
{
    /// <summary>
    /// AMI collection base
    /// </summary>
    [XmlType(nameof(AmiCollection<T>), Namespace = "http://openiz.org/ami")]
    public class AmiCollection<T>
    {
        /// <summary>
        /// Total collection size
        /// </summary>
        [XmlAttribute("size")]
        public int Size { get; set; }

        /// <summary>
        /// Total offset 
        /// </summary>
        [XmlAttribute("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Collection item
        /// </summary>
        [XmlElement("item")]
        public List<T> CollectionItem { get; set; }

    }
}
