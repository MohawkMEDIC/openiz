using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents an expression collection
    /// </summary>
    [XmlType(nameof(XmlExpressionList), Namespace = "http://openiz.org/protocol")]
    public class XmlExpressionList
    {

        /// <summary>
        /// Represents an expression list
        /// </summary>
        public XmlExpressionList()
        {

        }
        /// <summary>
        /// Initialize context
        /// </summary>
        public virtual void InitializeContext(XmlExpression context)
        {
            foreach (var itm in this.Item)
                itm.InitializeContext(context);
        }

        /// <summary>
        /// Creates a new xml expression list
        /// </summary>
        public XmlExpressionList(IEnumerable<Expression> expr)
        {
            this.Item = new List<XmlExpression>(expr.Select(o=>XmlExpression.FromExpression(o)));
        }

        /// <summary>
        /// Represents the list of items
        /// </summary>
        [XmlElement("constantExpression", typeof(XmlConstantExpression))]
        [XmlElement("memberExpression", typeof(XmlMemberExpression))]
        [XmlElement("parameterExpression", typeof(XmlParameterExpression))]
        [XmlElement("binaryExpression", typeof(XmlBinaryExpression))]
        [XmlElement("unaryExpression", typeof(XmlUnaryExpression))]
        [XmlElement("methodCallExpression", typeof(XmlMethodCallExpression))]
        [XmlElement("lambdaExpression", typeof(XmlLambdaExpression))]
        [XmlElement("typeBinaryExpression", typeof(XmlTypeBinaryExpression))]
        public List<XmlExpression> Item { get; set; }

 
    }
}