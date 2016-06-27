using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Represents sort order
    /// </summary>
    [XmlType(nameof(SortOrderType), Namespace = "http://openiz.org/model/map")]
    public enum SortOrderType
    {
        [XmlEnum("asc")]
        OrderBy,
        [XmlEnum("desc")]
        OrderByDescending
    }
}