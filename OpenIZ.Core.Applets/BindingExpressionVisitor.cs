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
using OpenIZ.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenIZ.Core.Applets
{
    /// <summary>
    /// Binding expression visitor
    /// </summary>
    internal class BindingExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        public LambdaExpression RewriteLambda(LambdaExpression node)
        {
            return Expression.Lambda(this.Visit(node.Body), node.Parameters[0]);
        }

        /// <summary>
        /// Visit specific types of expressions
        /// </summary>
        /// <param name="node"></param>
        public override Expression Visit(Expression node)
        {
            if (node == null) return node;

            switch(node.NodeType)
            {
                case ExpressionType.Equal:
                    {
                        var binary = node as BinaryExpression;
                        if ((binary.Right as ConstantExpression)?.Value.ToString() == "RemoveMe")
                            return binary.Left;
                        else
                            return binary;
                    }
                case ExpressionType.Call:
                    {
                        var call = node as MethodCallExpression;

                        var obj = this.Visit(call.Object);
                        var methodInfo = call.Method;
                        switch (methodInfo.Name)
                        {
                            case "Where": // Where becomes FirstOrDefault()
                                //obj = call.Arguments[0];
                                return Expression.Call(null, (MethodInfo)typeof(Enumerable).GetGenericMethod("FirstOrDefault", methodInfo.GetGenericArguments(), methodInfo.GetParameters().Select(o => o.ParameterType).ToArray()),
                                    call.Arguments);
                            case "Any": // Any gets stripped
                                {
                                    Expression bind = this.Visit(call.Arguments[0]);
                                    var lambda = call.Arguments[1] as LambdaExpression;

                                    Expression leftExpression = null;
                                    if (lambda.Body is MethodCallExpression) // Embedded ANY?
                                        leftExpression = (lambda.Body as MethodCallExpression).Object ?? (lambda.Body as MethodCallExpression).Arguments[0];
                                    else
                                        leftExpression = (lambda.Body as BinaryExpression).Left as MemberExpression;

                                    Stack<Expression> accessExpression = new Stack<Expression>();
                                    while (leftExpression != lambda.Parameters[0])
                                    {
                                        accessExpression.Push(leftExpression);
                                        if (leftExpression is MemberExpression)
                                            leftExpression = (leftExpression as MemberExpression).Expression;
                                        else if (leftExpression is MethodCallExpression)
                                            leftExpression = (leftExpression as MethodCallExpression).Object ?? (leftExpression as MethodCallExpression).Arguments[0];
                                    }


                                    // Now we re-write
                                    while (accessExpression.Count > 0)
                                    {
                                        var expr = accessExpression.Pop();
                                        if (expr is MemberExpression)
                                            bind = Expression.MakeMemberAccess(bind, (expr as MemberExpression).Member);
                                        else
                                        {
                                            var method = (expr as MethodCallExpression).Method;
                                            if (method.IsStatic)
                                            {
                                                bind = this.Visit(Expression.Call(null, (expr as MethodCallExpression).Method, bind, (expr as MethodCallExpression).Arguments[1]));
                                            }
                                            else
                                                bind = Expression.Call(bind, (expr as MethodCallExpression).Method, (expr as MethodCallExpression).Arguments);
                                        }
                                    }
                                    return bind;
                                }
                            default:
                                return base.Visit(node);
                        }
                    }
                default:
                    return base.Visit(node);
            }
        }

        public BindingExpressionVisitor()
        {
        }
    }
}