using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    [XmlType(nameof(AggregationFunctionType), Namespace = "http://openiz.org/model/map")]
    public enum AggregationFunctionType
    {
        None,
        [XmlEnum("last")]
        LastOrDefault,
        [XmlEnum("first")]
        FirstOrDefault,
        [XmlEnum("single")]
        SingleOrDefault,
        [XmlEnum("count")]
        Count,
        [XmlEnum("sum")]
        Sum
    }
}