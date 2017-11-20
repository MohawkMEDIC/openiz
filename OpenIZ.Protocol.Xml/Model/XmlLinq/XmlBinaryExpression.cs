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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{

    /// <summary>
    /// Gets or sets the binary expression operator
    /// </summary>
    [XmlType(nameof(BinaryOperatorType), Namespace = "http://openiz.org/cdss")]
    public enum BinaryOperatorType
    {
        [XmlEnum("eq")]
        Equal,
        [XmlEnum("lt")]
        LessThan,
        [XmlEnum("lte")]
        LessThanOrEqual,
        [XmlEnum("gt")]
        GreaterThan,
        [XmlEnum("gte")]
        GreaterThanOrEqual,
        [XmlEnum("ne")]
        NotEqual,
        [XmlEnum("and")]
        AndAlso,
        [XmlEnum("or")]
        OrElse,
        [XmlEnum("add")]
        Add,
        [XmlEnum("sub")]
        Subtract,
        [XmlEnum("is")]
        TypeIs
    }

    /// <summary>
    /// Represents an XML binary expression
    /// </summary>
    [XmlType(nameof(XmlBinaryExpression), Namespace = "http://openiz.org/cdss")]
    public class XmlBinaryExpression : XmlExpression
    {

        /// <summary>
        /// Represents a binary expression
        /// </summary>
        public XmlBinaryExpression()
        {
            this.Parts = new List<XmlExpression>();
        }

        /// <summary>
        /// Initialize context
        /// </summary>
        public override void InitializeContext(XmlExpression context)
        {
            base.InitializeContext(context);
            foreach (var itm in this.Parts)
                itm.InitializeContext(this);
        }

        /// <summary>
        /// Creates an XmlBinaryExpression from the specified binary expression
        /// </summary>
        public XmlBinaryExpression(BinaryExpression expr) 
        {
            BinaryOperatorType opType = BinaryOperatorType.AndAlso;
            if (!Enum.TryParse<BinaryOperatorType>(expr.NodeType.ToString(), out opType))
                throw new ArgumentOutOfRangeException(nameof(Expression.NodeType));
            this.Operator = opType;

            this.Parts = new List<XmlExpression>() {
                XmlExpression.FromExpression(expr.Left),
                XmlExpression.FromExpression(expr.Right)
            };
        }

        /// <summary>
        /// Gets or sets the operator of the binary expression
        /// </summary>
        [XmlAttribute("operator")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the left side of the expression
        /// </summary>
        [XmlElement("constantExpression", typeof(XmlConstantExpression))]
        [XmlElement("memberExpression", typeof(XmlMemberExpression))]
        [XmlElement("parameterExpression", typeof(XmlParameterExpression))]
        [XmlElement("binaryExpression", typeof(XmlBinaryExpression))]
        [XmlElement("unaryExpression", typeof(XmlUnaryExpression))]
        [XmlElement("methodCallExpression", typeof(XmlMethodCallExpression))]
        [XmlElement("typeBinaryExpression", typeof(XmlTypeBinaryExpression))]
        public List<XmlExpression> Parts { get; set; }

        /// <summary>
        /// Get the type of the binary
        /// </summary>
        public override Type Type
        {
            get
            {
                switch(this.Operator)
                {
                    case BinaryOperatorType.Add:
                    case BinaryOperatorType.Subtract:
                        return this.Parts[0].Type;
                    default:
                        return typeof(bool);
                }
            }
        }

        /// <summary>
        /// Converts the object to an expression
        /// </summary>
        /// <returns></returns>
        public override Expression ToExpression()
        {
            if (this.Parts.Count < 2)
                throw new InvalidOperationException("At least two parts must be in a expression tree");

            // We basically take the two parts and construct those :) 
            ExpressionType type = ExpressionType.Add;
            if (!Enum.TryParse<ExpressionType>(this.Operator.ToString(), out type))
                throw new ArgumentOutOfRangeException(nameof(ExpressionType));
            Queue<XmlExpression> parts = new Queue<XmlExpression>(this.Parts);
            var retVal = Expression.MakeBinary(type, parts.Dequeue().ToExpression(), parts.Dequeue().ToExpression());

            while (parts.Count > 0)
                retVal = Expression.MakeBinary(type, retVal, parts.Dequeue().ToExpression());

            return retVal;
        }
    }
}
