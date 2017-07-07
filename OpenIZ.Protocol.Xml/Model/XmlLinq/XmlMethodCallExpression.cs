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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model.XmlLinq
{
    /// <summary>
    /// Represents a call expression
    /// </summary>
    [XmlType(nameof(XmlMethodCallExpression), Namespace = "http://openiz.org/cdss")]
    public class XmlMethodCallExpression : XmlBoundExpression
    {

        /// <summary>
        /// Call expression ctor
        /// </summary>
        public XmlMethodCallExpression()
        {
            this.Parameters = new XmlExpressionList();
        }

        /// <summary>
        /// Initialize context
        /// </summary>
        public override void InitializeContext(XmlExpression context)
        {
            base.InitializeContext(context);
            this.Parameters.InitializeContext(this);
        }

        /// <summary>
        /// Create call expression from .net call expression
        /// </summary>
        /// <param name="methodCallExpression"></param>
        public XmlMethodCallExpression(MethodCallExpression expr) : base(expr)
        {
            this.MethodName = expr.Method.Name;
            this.Parameters = new XmlExpressionList(expr.Arguments);

            // Static so we need to know where to find the thing
            if (expr.Method.IsStatic)
            {
                this.StaticClassXml = expr.Method.DeclaringType.AssemblyQualifiedName;
                if (expr.Method.IsGenericMethod)
                    this.MethodTypeArgumentXml = expr.Method.GetGenericArguments().Select(o => o.AssemblyQualifiedName).ToArray();
            }
        }

        /// <summary>
        /// Method type argument
        /// </summary>
        [XmlElement("methodTypeArgument")]
        public String[] MethodTypeArgumentXml { get; set; }


        /// <summary>
        /// Gets or sets the method name
        /// </summary>
        [XmlAttribute("methodName")]
        public String MethodName { get; set; }

        /// <summary>
        /// Represents the parameter list to the call
        /// </summary>
        [XmlElement("argument")]
        public XmlExpressionList Parameters { get; set; }

        /// <summary>
        /// Get the type of this method call
        /// </summary>
        public override Type Type
        {
            get
            {
                // Can we just go?
                if(this.MethodTypeArgumentXml == null)
                    return (this.StaticClass ?? this.Object?.Type)?.GetRuntimeMethod(this.MethodName, this.Parameters.Item.Select(o => o.Type).ToArray())?.ReturnType;
                else
                {
                    var mi = this.StaticClass.GetRuntimeMethods().FirstOrDefault(o => o.Name == this.MethodName && o.GetParameters().Length == this.Parameters.Item.Count);
                    var methodInfo = mi.MakeGenericMethod(this.MethodTypeArgumentXml.Select(o => Type.GetType(o)).ToArray());
                    return methodInfo.ReturnType;
                }
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
            else if (String.IsNullOrEmpty(this.MethodName))
                throw new InvalidOperationException("Missing method name");

            var parameters = this.Parameters.Item.Select(o => o.ToExpression());
            var methodInfo = (this.StaticClass ?? this.Object?.Type).GetRuntimeMethod(this.MethodName, parameters.Select(o=>o.Type).ToArray());
            if (methodInfo == null && this.MethodTypeArgumentXml != null)
            {
                var mi = this.StaticClass.GetRuntimeMethods().FirstOrDefault(o => o.Name == this.MethodName && o.GetParameters().Length == this.Parameters.Item.Count);
                methodInfo = mi.MakeGenericMethod(this.MethodTypeArgumentXml.Select(o => Type.GetType(o)).ToArray());
            }
            if (methodInfo == null)
                throw new InvalidOperationException(String.Format("Could not find method {0} in type {1}", this.MethodName, this.Object.Type));

            return Expression.Call(this.Object?.ToExpression(), methodInfo, parameters.ToArray());
        }
    }
}