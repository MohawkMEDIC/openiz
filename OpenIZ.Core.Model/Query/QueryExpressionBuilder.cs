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
 * Date: 2016-8-2
 */
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Attributes;
using System.Linq;
using System.Collections;

namespace OpenIZ.Core.Model.Query
{
    /// <summary>
    /// Expression visitor which turns a LINQ expression against a query type to an HTTP header
    /// </summary>
    public class QueryExpressionBuilder
    {
        /// <summary>
        /// Http query expression visitor.
        /// </summary>
        private class HttpQueryExpressionVisitor : ExpressionVisitor
        {
            // The dictionary
            private List<KeyValuePair<String, Object>> m_query;
			/// <summary>
			/// Initializes a new instance of the <see cref="HttpQueryExpressionVisitor"/> class.
			/// </summary>
			/// <param name="workingDictionary">The working dictionary.</param>
			public HttpQueryExpressionVisitor(List<KeyValuePair<String, Object>> workingDictionary)
            {
                this.m_query = workingDictionary;
            }

            /// <summary>
            /// Add a condition if not already present
            /// </summary>
            private void AddCondition(String key, Object value)
            {
                var cvalue = this.m_query.FirstOrDefault(o=>o.Key == key);
                if (cvalue.Value == null)
                    this.m_query.Add(new KeyValuePair<string, object>(key, value));
                else if (cvalue.Value is IList)
                    (cvalue.Value as IList).Add(value);
                else
                {
                    this.m_query.Remove(cvalue);
                    this.m_query.Add(new KeyValuePair<String, Object>(key, new List<Object>() { cvalue.Value, value }));
                }
            }

            /// <summary>
            /// Visit a query expression
            /// </summary>
            /// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original
            /// expression list.</returns>
            /// <param name="nodes">The expressions to visit.</param>
            /// <param name="node">Node.</param>
            public override Expression Visit(Expression node)
            {
                if (node == null)
                    return node;

                // Convert node type
                switch (node.NodeType)
                {
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        return this.VisitBinary((BinaryExpression)node);
                    case ExpressionType.MemberAccess:
                        return this.VisitMemberAccess((MemberExpression)node);
                    case ExpressionType.Parameter:
                        return this.VisitParameter((ParameterExpression)node);
                    case ExpressionType.Call:
                        return this.VisitMethodCall((MethodCallExpression)node);
                    case ExpressionType.Lambda:
                        return this.VisitLambdaGeneric((LambdaExpression)node);
                    case ExpressionType.Constant:
                    case ExpressionType.Convert:
                    case ExpressionType.TypeAs:
                        return node;
                    default:
                        return this.Visit(node);
                }
            }

            /// <summary>
            /// Visit method call
            /// </summary>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                switch (node.Method.Name)
                {
                    case "Contains":
                        {
                            var parmName = this.ExtractPath(node.Object);
                            object parmValue = this.ExtractValue(node.Arguments[0]);
                            this.AddCondition(parmName, "~" + parmValue.ToString());
                            return null;
                        }
                    case "StartsWith":
                        {
                            var parmName = this.ExtractPath(node.Object);
                            object parmValue = this.ExtractValue(node.Arguments[0]);
                            this.AddCondition(parmName, "^" + parmValue.ToString());
                            return null;
                        }
                    case "Any":
                        {
                            var parmName = this.ExtractPath(node.Arguments[0]);
                            // Process lambda
                            var result = new List<KeyValuePair<string, object>>();
                            var subQueryExpressionVisitor = new HttpQueryExpressionVisitor(result);
                            if (node.Arguments.Count == 2)
                            {
                                subQueryExpressionVisitor.Visit(node.Arguments[1]);

                                // Result
                                foreach (var itm in result)
                                    this.AddCondition(String.Format("{0}.{1}", parmName, itm.Key), itm.Value);
                                return null;
                            }
                            else
                                return null;
                           
                        }
                    default:
                        return base.VisitMethodCall(node);

                }

            }

            /// <summary>
            /// Visits the member access.
            /// </summary>
            /// <returns>The member access.</returns>
            /// <param name="expr">Expr.</param>
            protected virtual Expression VisitMemberAccess(MemberExpression node)
            {
                this.Visit(node.Expression);
                return node;
            }

            /// <summary>
            /// Visits the lambda generic.
            /// </summary>
            /// <returns>The lambda generic.</returns>
            /// <param name="node">Node.</param>
            protected virtual Expression VisitLambdaGeneric(LambdaExpression node)
            {
                this.Visit(node.Body);
                return node;
            }

            /// <summary>
            /// Visit a binary expression which is in the form of A(operator)B
            /// </summary>
            /// <returns>The binary.</returns>
            /// <param name="node">Node.</param>
            protected override Expression VisitBinary(BinaryExpression node)
            {
                this.Visit(node.Left);
                this.Visit(node.Right);

                String parmName = this.ExtractPath(node.Left);
                Object parmValue = this.ExtractValue(node.Right);

                // Not able to map
                if ((node.Left as MethodCallExpression)?.Method.Name == "Any" &&
                    (node.Left as MethodCallExpression)?.Arguments.Count == 1)
                {
                    // Special exists method call - i.e. HAS X
                    var mci = (node.Left as MethodCallExpression);
                    parmName = this.ExtractPath(mci.Arguments[0]);
                    var cci = node.Right is ConstantExpression;
                    this.AddCondition(parmName, cci ? "null" : "!null");
                }
                else if (!String.IsNullOrEmpty(parmName))
                {
                    Object fParmValue = parmValue;
                    if (parmValue is DateTime)
                        fParmValue = ((DateTime)parmValue).ToString("o");
                    else if (parmValue is DateTimeOffset)
                        fParmValue = ((DateTimeOffset)parmValue).ToString("o");
                    else if (parmValue == null)
                        fParmValue = "null";
                    // Node type
                    switch (node.NodeType)
                    {
                        case ExpressionType.GreaterThan:
                            fParmValue = ">" + fParmValue;
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            fParmValue = ">=" + fParmValue;
                            break;
                        case ExpressionType.LessThan:
                            fParmValue = "<" + fParmValue;
                            break;
                        case ExpressionType.LessThanOrEqual:
                            fParmValue = "<=" + fParmValue;
                            break;
                        case ExpressionType.NotEqual:
                            fParmValue = "!" + fParmValue;
                            break;
                    }

                    this.AddCondition(parmName, fParmValue);
                }
                
                return node;
            }

            /// <summary>
            /// Extract a value
            /// </summary>
            /// <returns>The value.</returns>
            /// <param name="access">Access.</param>
            protected Object ExtractValue(Expression access)
            {
                if (access == null)
                    return null;
                if (access.NodeType == ExpressionType.Parameter)
                    return ((ParameterExpression)access).Name;
                else if (access.NodeType == ExpressionType.Constant)
                    return ((ConstantExpression)access).Value;
                else if (access.NodeType == ExpressionType.Convert)
                    return this.ExtractValue(((UnaryExpression)access).Operand);
                else if (access.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression expr = access as MemberExpression;
                    var expressionValue = this.ExtractValue(expr.Expression);
                    if (expr.Member is PropertyInfo)
                    {
	                    try
	                    {
							return (expr.Member as PropertyInfo).GetValue(expressionValue);
						}
	                    catch
	                    {
		                    return null;
	                    }
                    }
                    else if (expr.Member is FieldInfo)
                    {
                        return (expr.Member as FieldInfo).GetValue(expressionValue);
                    }
                }
                return null;
            }

            /// <summary>
            /// Extract the path
            /// </summary>
            /// <returns>The path.</returns>
            /// <param name="access">Access.</param>
            protected String ExtractPath(Expression access)
            {
                if (access.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression memberExpr = access as MemberExpression;
                    String path = this.ExtractPath(memberExpr.Expression); // get the chain if required
                    if (memberExpr.Expression.Type.GetTypeInfo().IsGenericType && memberExpr.Expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        return path;

                    // XML property?
                    var memberInfo = memberExpr.Expression.Type.GetRuntimeProperty(memberExpr.Member.Name + "Xml") ??
                                     memberExpr.Member;

                    // Is this a delay load?
                    var SerializationReferenceAttribute = memberExpr.Member.GetCustomAttribute<SerializationReferenceAttribute>();
                    var xmlIgnoreAttribute = memberExpr.Member.GetCustomAttribute<XmlIgnoreAttribute>();
                    if (xmlIgnoreAttribute != null && SerializationReferenceAttribute != null && !String.IsNullOrEmpty(SerializationReferenceAttribute.RedirectProperty))
                        memberInfo = memberExpr.Expression.Type.GetRuntimeProperty(SerializationReferenceAttribute.RedirectProperty);

                    // TODO: Delay and bound properties!!
                    var memberXattribute = memberInfo.GetCustomAttributes<XmlElementAttribute>().FirstOrDefault();
                    if (memberXattribute == null)
                        return null; // TODO: When this occurs?

                    // Return path
                    if (String.IsNullOrEmpty(path))
                        return memberXattribute.ElementName;
                    else if (memberXattribute.ElementName == "id") // ID can be ignored
                        return path;
                    else
                        return String.Format("{0}.{1}", path, memberXattribute.ElementName);
                }
                else if (access.NodeType == ExpressionType.Call)
                {
                    //CallExpression callExpr = access as MemberExpression;
                    MethodCallExpression callExpr = access as MethodCallExpression;

                    if (callExpr.Method.Name == "Where")
                    {
                        String path = this.ExtractPath(callExpr.Arguments[0]); // get the chain if required
                        var guardExpression = callExpr.Arguments[1] as LambdaExpression;
                        // Where should be a guard so we just grab the unary equals only!
                        var binaryExpression = guardExpression.Body as BinaryExpression;
                        if (binaryExpression == null)
                            throw new InvalidOperationException("Cannot translate non-binary expression guards");

                        // Is the expression the guard?
                        String guardString = this.BuildGuardExpression(binaryExpression); 
                        return String.Format("{0}[{1}]", path, guardString);

                    }
                }
                else if(access.NodeType == ExpressionType.Convert ||
                    access.NodeType == ExpressionType.ConvertChecked)
                {
                    UnaryExpression ua = (UnaryExpression)access;
                    return this.ExtractPath(ua.Operand);
                } 
                else if(access.NodeType == ExpressionType.TypeAs)
                {
                    UnaryExpression ua = (UnaryExpression)access;
                    return String.Format("{0}@{1}", this.ExtractPath(ua.Operand), ua.Type.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName);
                }
                return null;
            }

            /// <summary>
            /// Build a guard expression
            /// </summary>
            private string BuildGuardExpression(BinaryExpression binaryExpression)
            {

                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.Or:
                        return $"{this.BuildGuardExpression(binaryExpression.Left as BinaryExpression)}|{this.BuildGuardExpression(binaryExpression.Right as BinaryExpression)}";
                    case ExpressionType.Equal:
                        var expressionMember = binaryExpression.Left as MemberExpression;
                        var valueExpression = binaryExpression.Right as ConstantExpression;
                        if (expressionMember.Member.DeclaringType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>()?.ClassifierProperty != expressionMember.Member.Name)
                            throw new InvalidOperationException("Guards must be on classifier");
                        if (valueExpression == null)
                            throw new InvalidOperationException("Only constant expressions are supported on guards");
                        return valueExpression.Value.ToString();
                    default:
                        throw new InvalidOperationException($"Binary expressions of {binaryExpression.NodeType} are not permitted");

                }

            }
        }

        /// <summary>
        /// Builds the query dictionary .
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="model">Model.</param>
        /// <typeparam name="TModel">The 1st type parameter.</typeparam>
        public static IEnumerable<KeyValuePair<String, Object>> BuildQuery<TModel>(Expression<Func<TModel, bool>> model, bool stripNullChecks = false)
        {
            List<KeyValuePair<String, Object>> retVal = new List<KeyValuePair<string, Object>>();
            var visitor = new HttpQueryExpressionVisitor(retVal);
            visitor.Visit(model);
            if(stripNullChecks)
                retVal.RemoveAll(o => retVal.Any(c => c.Key == o.Key && c.Value != o.Value) && o.Value.Equals("!null"));
            return retVal;
        }

    }
}

