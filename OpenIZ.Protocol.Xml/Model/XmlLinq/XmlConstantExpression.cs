using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents an expression that is a constant
    /// </summary>
    [XmlInclude(typeof(int))]
    [XmlInclude(typeof(decimal))]
    [XmlInclude(typeof(Guid))]
    [XmlInclude(typeof(DateTime))]
    [XmlInclude(typeof(String))]
    [XmlInclude(typeof(bool))]
    [XmlType(nameof(XmlConstantExpression), Namespace = "http://openiz.org/protocol")]
    public class XmlConstantExpression : XmlExpression
    {
        /// <summary>
        /// Create constant expression
        /// </summary>
        public XmlConstantExpression()
        {

        }

        /// <summary>
        /// Create constant expression from the specitifed expression
        /// </summary>
        /// <param name="constantExpression"></param>
        public XmlConstantExpression(ConstantExpression expr)
        {
            this.Value = expr.Value;
        }

        /// <summary>
        /// Get the type of this expression
        /// </summary>
        public override Type Type
        {
            get
            {
                return this.Value?.GetType();
            }
        }

        /// <summary>
        /// Value of the constant expression
        /// </summary>
        [XmlElement("value")]
        public Object Value { get; set; }

        /// <summary>
        /// Create the specified constant expression
        /// </summary>
        public override Expression ToExpression()
        {
            return Expression.Constant(this.Value);
        }
    }
}
