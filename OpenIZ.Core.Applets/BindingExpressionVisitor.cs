using OpenIZ.Core.Model.Reflection;
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
                                    
                                    Expression leftExpression = (lambda.Body as BinaryExpression).Left as MemberExpression;
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
                                            bind = Expression.Call(bind, (expr as MethodCallExpression).Method, (expr as MethodCallExpression).Arguments);
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