using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// A single address component
    /// </summary>
    [Serializable]
    [XmlType("AddressComponent", Namespace = "http://openiz.org/model")]
    public class EntityAddressComponent : GenericComponentValues<EntityAddress>
    {
       
        /// <summary>
        /// Gets or sets the value of the component
        /// </summary>
        [XmlElement("value")]
        public String Value { get; set; }

    }
}