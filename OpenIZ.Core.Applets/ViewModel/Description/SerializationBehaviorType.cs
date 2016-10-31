using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Serialization behavior type
    /// </summary>
    [XmlType(nameof(SerializationBehaviorType), Namespace = "http://openiz.org/model/view")]
    [Flags]
    public enum SerializationBehaviorType
    {
        [XmlEnum("default")]
        Default = 0,
        [XmlEnum("always")]
        Always = 1,
        [XmlEnum("never")]
        Never = 2
    }
}