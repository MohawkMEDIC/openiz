/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-12-1
 */
using System;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{

    /// <summary>
    /// Gets or sets the binary expression operator
    /// </summary>
    [XmlType(nameof(UnaryOperatorType), Namespace = "http://openiz.org/cdss")]
    public enum UnaryOperatorType
    {
        [XmlEnum("not")]
        Not,
        [XmlEnum("convert")]
        Convert,
        [XmlEnum("neg")]
        Negate,
        [XmlEnum("as")]
        TypeAs
    }

    /// <summary>
    /// Represents a unary expression
    /// </summary>
    [XmlType(nameof(XmlUnaryExpression), Namespace = "http://openiz.org/cdss")]
    public class XmlUnaryExpression : XmlBoundExpression
    {
        /// <summary>
        /// Creates a new unary expression
        /// </summary>
        public XmlUnaryExpression()
        {

        }

        /// <summary>
        /// Create the unary expression from a .net expression
        /// </summary>
        public XmlUnaryExpression(UnaryExpression expr) : base(expr)
        {
            UnaryOperatorType uop = UnaryOperatorType.Negate;
            if (!Enum.TryParse<UnaryOperatorType>(expr.NodeType.ToString(), out uop))
                throw new ArgumentOutOfRangeException(nameof(UnaryExpression.NodeType));
            this.Operator = uop;
            this.TypeXml = expr.Type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets or sets the unary operator
        /// </summary>
        [XmlAttribute("operator")]
        public UnaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        [XmlAttribute("type")]
        public String TypeXml { get; set; }

        /// <summary>
        /// Gets the type of expression
        /// </summary>
        public override Type Type
        {
            get
            {
                switch (this.Operator)
                {
                    case UnaryOperatorType.Convert:
                    case UnaryOperatorType.TypeAs:
                        return Type.GetType(this.TypeXml);
                    default:
                        return this.Object?.Type;
                }
            }
        }

        /// <summary>
        /// Convert this to a .NET expression
        /// </summary>
        /// <returns></returns>
        public override Expression ToExpression()
        {
            ExpressionType uop = ExpressionType.Parameter;
            if (!Enum.TryParse<ExpressionType>(this.Operator.ToString(), out uop))
                throw new ArgumentOutOfRangeException(nameof(ExpressionType));
            return Expression.MakeUnary(uop, this.Object.ToExpression(), this.Type);
        }
    }
}