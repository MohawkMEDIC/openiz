/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.Map;
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Model conversion visitor is used to convert a lambda expression based on the business model 
    /// into a domain model lamda expression
    /// </summary>
    public class ModelExpressionVisitor : ExpressionVisitor
    {


        /// <summary>
        /// A small visitor which corrects lambda expressions to skip over associative
        /// classes
        /// </summary>
        private class LambdaCorrectionVisitor : ExpressionVisitor
        {

            // Original Parameter
            private readonly ParameterExpression m_originalParameter;
            // Member access
            private readonly Expression m_memberAccess;

            /// <summary>
            /// Creates a new instance of the lambda correction visitor
            /// </summary>
            public LambdaCorrectionVisitor(Expression correctedMemberAccess, ParameterExpression lambdaExpressionParameter)
            {
                this.m_originalParameter = lambdaExpressionParameter;
                this.m_memberAccess = correctedMemberAccess;

            }

            /// <summary>
            /// Visit the node
            /// </summary>
            public override Expression Visit(Expression node)
            {

                if (node == null)
                    return node;

                switch (node.NodeType)
                {
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Equal:
                        return this.VisitBinary((BinaryExpression)node);
                    case ExpressionType.MemberAccess:
                        {
                            MemberExpression memberExpression = node as MemberExpression;
                            if ((memberExpression.Expression as ParameterExpression)?.Name == this.m_originalParameter.Name)
                            {
                                var memInfo = this.m_memberAccess.Type.GetMember(memberExpression.Member.Name, memberExpression.Member.MemberType, BindingFlags.Public | BindingFlags.Instance);
                                if(memInfo.Length == 0)
                                    return memberExpression;
                                else
                                    return Expression.MakeMemberAccess(this.m_memberAccess, memInfo.FirstOrDefault() ?? memberExpression.Member);
                            }
                            else
                                return base.Visit(node);
                        }
                    default:
                        return base.Visit(node);
                }

            }

            /// <summary>
            /// Visit a binary method
            /// </summary>
            protected override Expression VisitBinary(BinaryExpression node)
            {
                Expression right = this.Visit(node.Right),
                    left = this.Visit(node.Left);
                if (right != node.Right || left != node.Left)
                    return Expression.MakeBinary(node.NodeType, left, right);
                return node;
            }
        }

        // The mapper to be used
        private readonly ModelMapper m_mapper;

        // Parameters
        private readonly ParameterExpression[] m_parameters;

        /// <summary>
        /// Model conversion visitor 
        /// </summary>
        public ModelExpressionVisitor(ModelMapper mapData, params ParameterExpression[] parameters)
        {
            this.m_mapper = mapData;
            this.m_parameters = parameters;
        }

        /// <summary>
        /// Visit an expression
        /// </summary>
        public override Expression Visit(Expression node)
        {

            if (node == null)
                return node;

            switch (node.NodeType)
            {
                // TODO: Unary
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return this.VisitBinary((BinaryExpression)node);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)node);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)node);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)node);
                case ExpressionType.Lambda:
                    return this.VisitLambdaGeneric((LambdaExpression)node);
                case ExpressionType.Convert:
                    return this.VisitConvert((UnaryExpression)node);
                default:
                    return base.Visit(node);
            }

        }

        /// <summary>
        /// Remove unnecessary convert statement
        /// </summary>
        public virtual Expression VisitConvert(UnaryExpression convert)
        {
            Expression newOperand = this.Visit(convert.Operand);
            if (newOperand != convert.Operand)
            {
                Type targetType = m_mapper.MapModelType(convert.Type);
                if (targetType == convert.Type) // No map
                    return newOperand;
                return Expression.Convert(newOperand, targetType);
            }
            return convert;
        }

        /// <summary>
        /// Visit a lambda expression
        /// </summary>
        protected virtual Expression VisitLambdaGeneric(LambdaExpression node)
        {
            Expression newBody = this.Visit(node.Body);
            if (newBody != node.Body)
            {
                var parameters = this.VisitExpressionList(node.Parameters.OfType<Expression>().ToList().AsReadOnly()).OfType<ParameterExpression>().ToArray();
                var lambdaType = node.Type;
                if (lambdaType.GetGenericTypeDefinition() == typeof(Func<,>))
                    lambdaType = typeof(Func<,>).MakeGenericType(parameters.Select(p => p.Type).Union(new Type[] { newBody.Type }).ToArray());
                return Expression.Lambda(lambdaType, newBody, parameters);
            }
            return node;

        }

        /// <summary>
        /// Visit method call
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression newExpression = this.Visit(node.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(node.Arguments);
            if (newExpression != node.Object || args != node.Arguments)
            {
                // Re-bind the parameter types
                MethodInfo methodInfo = node.Method;
                if (methodInfo.IsGenericMethod) // Generic re-bind
                {
                    // HACK: Find a more appropriate way of doing this
                    Type bindType = this.m_mapper.ExtractDomainType(args.First().Type);
                    methodInfo = methodInfo.GetGenericMethodDefinition().MakeGenericMethod(new Type[] { bindType });
                }

                return Expression.Call(newExpression, methodInfo, args);
            }
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Visit each expression in the args
        /// </summary>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> args)
        {
            List<Expression> retVal = new List<Expression>();
            bool isDifferent = false;
            foreach (var exp in args)
            {
                Expression argExpression = this.Visit(exp);

                // Is there a VIA expression to be corrected?
                if (argExpression is LambdaExpression)
                {
                    var lambdaExpression = argExpression as LambdaExpression;
                    // Ok, we need to find the traversal expression
                    //this.m_mapper.
                    ParameterExpression newParameter = Expression.Parameter(this.m_mapper.ExtractDomainType(retVal[0].Type), lambdaExpression.Parameters[0].Name);
                    Expression accessExpression = this.m_mapper.CreateLambdaMemberAdjustmentExpression(args.First() as MemberExpression, newParameter);
                    Expression newBody = new LambdaCorrectionVisitor(accessExpression, lambdaExpression.Parameters[0]).Visit(lambdaExpression.Body);
                    Type lambdaType = typeof(Func<,>).MakeGenericType(new Type[] { newParameter.Type, newBody.Type });
                    argExpression = Expression.Lambda(lambdaType, newBody, newParameter);

                }
                // Add the expression
                if (argExpression != exp)
                {
                    isDifferent = true;
                    retVal.Add(argExpression);
                }
                else
                    retVal.Add(exp);
            }
            if (isDifferent)
                return retVal.AsReadOnly();
            else
                return args;
        }

        /// <summary>
        /// Visit a binary method
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Expression right = this.Visit(node.Right),
                left = this.Visit(node.Left);
            // Are the types compatible?
            if (!right.Type.IsAssignableFrom(left.Type))
            {
                // Convert
                return Expression.MakeBinary(node.NodeType, left, Expression.Convert(right, left.Type));
            }
            else if (right != node.Right || left != node.Left)
            {
                return Expression.MakeBinary(node.NodeType, left, right);
            }
            return node;
        }

        /// <summary>
        /// Visit parameter
        /// </summary>
        protected override Expression VisitParameter(ParameterExpression node)
        {

            Type mappedType = this.m_mapper.MapModelType(node.Type);
            var parameterRef = this.m_parameters.FirstOrDefault(p => p.Name == node.Name && p.Type == mappedType);

            if (parameterRef != null)
                return parameterRef;

            if (mappedType != null && mappedType != node.Type)
                return Expression.Parameter(mappedType, node.Name);

            return node;
        }

        /// <summary>
        /// Visit member access, converts member expression type and name
        /// </summary>
        /// <param name="node">The node to be converted</param>
        protected virtual Expression VisitMemberAccess(MemberExpression node)
        {
            // Convert the expression
            Expression newExpression = this.Visit(node.Expression);
            if (newExpression != node.Expression)
            {
                // Is the node member access a useless convert function?
                if (node.Expression.NodeType == ExpressionType.Convert)
                {
                    UnaryExpression convertExpression = node.Expression as UnaryExpression;
                    if (convertExpression.Type.IsAssignableFrom(convertExpression.Operand.Type))
                        node = Expression.MakeMemberAccess(convertExpression.Operand, node.Member);
                }
                return this.m_mapper.MapModelMember(node, newExpression);
            }
            return node;
        }


    }
}