using OpenIZ.Protocol.Xml.Model.XmlLinq;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents the where clause 
    /// </summary>
    [XmlType(nameof(ProtocolWhereClause), Namespace = "http://openiz.org/protocol")]
    public class ProtocolWhereClause
    {

        /// <summary>
        /// Lambda epression
        /// </summary>
        [XmlElement("lambdaExpression")]
        public XmlLambdaExpression LambdaExpression { get; set; }
    }
}