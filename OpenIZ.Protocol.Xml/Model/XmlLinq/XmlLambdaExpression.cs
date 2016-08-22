using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents a lambda expression
    /// </summary>
    [XmlType(nameof(XmlLambdaExpression), Namespace = "http://openiz.org/cdss")]
    public class XmlLambdaExpression : XmlBoundExpression
    {

        // Function types
        private static List<Type> s_funcTypes = new List<Type>()
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>)
        };

        /// <summary>
        /// Serialization ctor
        /// </summary>
        public XmlLambdaExpression()
        {
            
        }

        /// <summary>
        /// Creates a new lambda expression
        /// </summary>
        public XmlLambdaExpression(LambdaExpression expr) : base(expr.Body)
        {
            this.Parameters = expr.Parameters.Select(o => new XmlParameterExpression(o)).ToList();
        }

        /// <summary>
        /// Initialize context
        /// </summary>
        public override void InitializeContext(XmlExpression context)
        {
            base.InitializeContext(context);
            //foreach (var itm in this.Parameters)
            //    itm.InitializeContext(this);
        }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        [XmlElement("argument")]
        public List<XmlParameterExpression> Parameters { get; set; }

        /// <summary>
        /// Get the type of this item
        /// </summary>
        public override Type Type
        {
            get
            {
                return this.Object?.Type;
            }
        }

        /// <summary>
        /// Create the specified expression
        /// </summary>
        public override Expression ToExpression()
        {
            if (this.Type != null)
            {
                var lamdaType = s_funcTypes[this.Parameters.Count];
                var typeParameters = this.Parameters.Select(o => o.Type).ToList();
                typeParameters.Add(this.Type);
                return Expression.Lambda(lamdaType.MakeGenericType(typeParameters.ToArray()), this.Object.ToExpression(), this.Parameters.Select(o => o.ToExpression()).OfType<ParameterExpression>().ToArray());
            }
            else
                return Expression.Lambda(this.Object.ToExpression(), this.Parameters.Select(o => o.ToExpression()).OfType<ParameterExpression>().ToArray());
        }
    }
}