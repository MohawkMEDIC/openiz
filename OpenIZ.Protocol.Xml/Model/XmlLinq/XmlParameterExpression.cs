using System;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents an expression that accesses a named parameter
    /// </summary>
    [XmlType(nameof(XmlParameterExpression), Namespace = "http://openiz.org/protocol")]
    public class XmlParameterExpression : XmlExpression
    {
        // Expression
        private ParameterExpression m_expression;

        /// <summary>
        /// Creates the parameter expression
        /// </summary>
        public XmlParameterExpression()
        {

        }

        /// <summary>
        /// Represents a parameter expression
        /// </summary>
        public XmlParameterExpression(ParameterExpression expr) 
        {
            this.m_expression = expr;
            this.ParameterName = expr.Name;
            this.TypeXml = expr.Type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets or sets the type 
        /// </summary>
        [XmlAttribute("type")]
        public String TypeXml { get; set; }

        /// <summary>
        /// Gets or sets the parameter name
        /// </summary>
        [XmlAttribute("parameterName")]
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets the type of object
        /// </summary>
        public override Type Type
        {
            get
            {
                return Type.GetType(this.TypeXml);
            }
        }

        /// <summary>
        /// Create a parameter expression
        /// </summary>
        public override Expression ToExpression()
        {
            if (this.m_expression != null)
                return this.m_expression;

            if (this.Type == null)
                throw new InvalidOperationException("Type not set");

            // Is there some parameter in the parent context?
            XmlExpression xe = this.Parent;
            while (xe != null && this.m_expression == null)
            {
                this.m_expression = (xe as XmlLambdaExpression)?.Parameters.Find(o => o.ParameterName == this.ParameterName)?.ToExpression() as ParameterExpression;
                xe = xe.Parent;
            }

            if(this.m_expression == null)
                this.m_expression = Expression.Parameter(this.Type, this.ParameterName);
            return this.m_expression;
        }
    }
}