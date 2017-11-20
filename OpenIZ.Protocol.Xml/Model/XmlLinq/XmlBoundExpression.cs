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
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents an XmlExpression bound to another expression
    /// </summary>
    [XmlType(nameof(XmlBoundExpression), Namespace = "http://openiz.org/cdss")]
    public abstract class XmlBoundExpression : XmlExpression
    {
        /// <summary>
        /// Creates the bound expression
        /// </summary>
        public XmlBoundExpression()
        {

        }

        /// <summary>
        /// Initialize context
        /// </summary>
        public override void InitializeContext(XmlExpression context)
        {
            base.InitializeContext(context);
            this.Object?.InitializeContext(this);
        }

        /// <summary>
        /// Creates the bound expression
        /// </summary>
        public XmlBoundExpression(Expression expr) 
        {
            this.Object = XmlExpression.FromExpression(expr);
        }

        /// <summary>
        /// Creates type bound expression
        /// </summary>
        /// <param name="expr"></param>
        public XmlBoundExpression(TypeBinaryExpression expr)
        {
            this.Object = XmlExpression.FromExpression(expr.Expression);
        }

        /// <summary>
        /// Creates the bound expression
        /// </summary>
        public XmlBoundExpression(MemberExpression expr) 
        {
            this.Object = XmlExpression.FromExpression(expr.Expression);
        }

        /// <summary>
        /// Creates the bound expression
        /// </summary>
        public XmlBoundExpression(MethodCallExpression expr) 
        {
            this.Object = XmlExpression.FromExpression(expr.Object);
        }

        /// <summary>
        /// Creates the bound expression
        /// </summary>
        public XmlBoundExpression(UnaryExpression expr) 
        {
            this.Object = XmlExpression.FromExpression(expr.Operand);
        }

        /// <summary>
        /// Gets or sets the explicit type (for unbound methods)
        /// </summary>
        [XmlAttribute("staticClass")]
        public String StaticClassXml { get; set; }


        /// <summary>
        /// Gets the method class
        /// </summary>
        [XmlIgnore]
        public Type StaticClass
        {
            get
            {
                if (this.StaticClassXml == null)
                    return null;
                return Type.GetType(this.StaticClassXml);
            }
        }

        /// <summary>
        /// Gets or sets the expression
        /// </summary>
        [XmlElement("constantExpression", typeof(XmlConstantExpression))]
        [XmlElement("memberExpression", typeof(XmlMemberExpression))]
        [XmlElement("parameterExpression", typeof(XmlParameterExpression))]
        [XmlElement("binaryExpression", typeof(XmlBinaryExpression))]
        [XmlElement("unaryExpression", typeof(XmlUnaryExpression))]
        [XmlElement("methodCallExpression", typeof(XmlMethodCallExpression))]
        [XmlElement("typeBinaryExpression", typeof(XmlTypeBinaryExpression))]
        public XmlExpression Object { get; set; }
    }
}