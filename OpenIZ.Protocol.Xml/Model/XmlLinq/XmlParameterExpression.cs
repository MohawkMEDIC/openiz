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
    /// Represents an expression that accesses a named parameter
    /// </summary>
    [XmlType(nameof(XmlParameterExpression), Namespace = "http://openiz.org/cdss")]
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