using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Patch
{
    /// <summary>
    /// Represents a patch operation type
    /// </summary>
    [XmlType(nameof(PatchOperationType), Namespace = "http://openiz.org/model")]
    public enum PatchOperationType
    {
        /// <summary>
        /// Patch operation adds the specified value to the array
        /// </summary>
        [XmlEnum("add")]
        Add, 
        /// <summary>
        /// Patch operation removes the specified value from the array
        /// </summary>
        [XmlEnum("remove")]
        Remove,
        /// <summary>
        /// Patch operation replaces the specified item at the path 
        /// </summary>
        [XmlEnum("replace")]
        Replace, 
        /// <summary>
        /// Patch should test value before proceeding
        /// </summary>
        [XmlEnum("test")]
        Test
    }
}