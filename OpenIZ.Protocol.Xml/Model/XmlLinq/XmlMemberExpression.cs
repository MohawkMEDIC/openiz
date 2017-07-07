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
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents a expression that access an expression
    /// </summary>
    [XmlType(nameof(XmlMemberExpression), Namespace = "http://openiz.org/cdss")]
    public class XmlMemberExpression : XmlBoundExpression
    {

        /// <summary>
        /// Creates a new member expression
        /// </summary>
        public XmlMemberExpression()
        {

        }

        /// <summary>
        /// Creates a new member expression from a .net expression
        /// </summary>
        /// <param name="expr"></param>
        public XmlMemberExpression(MemberExpression expr) : base(expr)
        {
            this.MemberName = expr.Member.Name;
            if (expr.Expression == null)
                this.StaticClassXml = expr.Member.DeclaringType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets or sets the member name
        /// </summary>
        [XmlAttribute("memberName")]
        public String MemberName { get; set; }

        /// <summary>
        /// Get the type of this expression
        /// </summary>
        public override Type Type
        {
            get
            {
                return (this.StaticClass ?? this.Object?.Type).GetRuntimeProperty(this.MemberName)?.PropertyType ?? this.Object.Type.GetRuntimeField(this.MemberName)?.FieldType; 
            }
        }

        /// <summary>
        /// Convert to expression
        /// </summary>
        public override Expression ToExpression()
        {
            // validate
            if (this.Object == null && this.StaticClass == null)
                throw new InvalidOperationException("Bound object is required");
            else if (String.IsNullOrEmpty(this.MemberName))
                throw new InvalidOperationException("Missing method name");

            MemberInfo memberInfo = (MemberInfo)(this.StaticClass ?? this.Object?.Type).GetRuntimeProperty(this.MemberName) ??
                (this.StaticClass ?? this.Object?.Type).GetRuntimeField(this.MemberName);
            if (memberInfo == null)
                throw new InvalidOperationException(String.Format("Could not find member {0} in type {1}", this.MemberName, this.Object.Type));

            return Expression.MakeMemberAccess(this.Object?.ToExpression(), memberInfo);
        }
    }
}
