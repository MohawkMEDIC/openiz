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
    /// Represents an XmlRepresentation of an C# Expression
    /// </summary>
    [XmlType(nameof(XmlExpression), Namespace = "http://openiz.org/cdss")]
    public abstract class XmlExpression
    {

        /// <summary>
        /// Initialize the context
        /// </summary>
        public virtual void InitializeContext(XmlExpression context)
        {
            this.Parent = context;
        }

        /// <summary>
        /// Gets or sets the parent
        /// </summary>
        internal XmlExpression Parent { get; private set; }

        /// <summary>
        /// Gets the .net type which represents the return value of this expression
        /// </summary>
        [XmlIgnore]
        public abstract Type Type { get; }

        /// <summary>
        /// Creates an xml expression
        /// </summary>
        public XmlExpression()
        {

        }
        
        /// <summary>
        /// Represents this xml expression as an expression
        /// </summary>
        public abstract Expression ToExpression();

        /// <summary>
        /// Create an XmlExpression tree from a .NET expression tree
        /// </summary>
        public static XmlExpression FromExpression(Expression expr)
        {
            if (expr == null) return null;

            switch(expr.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return new XmlBinaryExpression(expr as BinaryExpression);
                case ExpressionType.TypeIs:
                    return new XmlTypeBinaryExpression(expr as TypeBinaryExpression);
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.Negate:
                case ExpressionType.TypeAs:
                    return new XmlUnaryExpression(expr as UnaryExpression);
                case ExpressionType.MemberAccess:
                    return new XmlMemberExpression(expr as MemberExpression);
                case ExpressionType.Call:
                    return new XmlMethodCallExpression(expr as MethodCallExpression);
                case ExpressionType.Constant:
                    return new XmlConstantExpression(expr as ConstantExpression);
                case ExpressionType.Lambda:
                    return new XmlLambdaExpression(expr as LambdaExpression);
                case ExpressionType.Parameter:
                    return new XmlParameterExpression(expr as ParameterExpression);
                default:
                    throw new ArgumentOutOfRangeException(nameof(expr));
            }
        }
    }
}
