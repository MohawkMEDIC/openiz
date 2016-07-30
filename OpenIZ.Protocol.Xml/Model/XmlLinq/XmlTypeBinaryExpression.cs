using System;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// XmlType binary expression
    /// </summary>
    [XmlType(nameof(XmlTypeBinaryExpression), Namespace = "http://openiz.org/protocol")]
    public class XmlTypeBinaryExpression : XmlBoundExpression
    {

        /// <summary>
        /// Serializer ctor
        /// </summary>
        public XmlTypeBinaryExpression()
        {

        }

        /// <summary>
        /// Constructs the type operand
        /// </summary>
        public XmlTypeBinaryExpression(TypeBinaryExpression expr) : base(expr)
        {
            this.QueryTypeXml = expr.TypeOperand.AssemblyQualifiedName;
        }

        /// <summary>
        /// Query the type of Xml
        /// </summary>
        [XmlAttribute("operandType")]
        public string QueryTypeXml { get; set; }

        /// <summary>
        /// Gets the type this expression is querying for
        /// </summary>
        public Type QueryType
        {
            get
            {
                return Type.GetType(this.QueryTypeXml);
            }
        }

        /// <summary>
        /// Gets the type check binary
        /// </summary>
        public override Type Type
        {
            get
            {
                return typeof(bool);
            }
        }

        /// <summary>
        /// Convert to an expression
        /// </summary>
        public override Expression ToExpression()
        {
            return Expression.TypeIs(this.Object?.ToExpression(), this.QueryType);
        }
    }
}