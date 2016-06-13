using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Attributes;

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
			/// Initializes a new instance of the
			/// <see cref="OpenIZ.Mobile.Core.Interop.Util.HttpQueryExpressionBuilder+HttpQueryExpressionVisitor"/> class.
			/// </summary>
			/// <param name="workingDictionary">Working dictionary.</param>
			public HttpQueryExpressionVisitor (List<KeyValuePair<String, Object>> workingDictionary)
			{
				this.m_query = workingDictionary;
			}

			/// <summary>
			/// Visit a query expression
			/// </summary>
			/// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original
			/// expression list.</returns>
			/// <param name="nodes">The expressions to visit.</param>
			/// <param name="node">Node.</param>
			public override Expression Visit (Expression node)
			{
				if (node == null)
					return node;

				// Convert node type
				switch (node.NodeType) {
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
					return this.VisitBinary ((BinaryExpression)node);
				case ExpressionType.MemberAccess:
					return this.VisitMemberAccess ((MemberExpression)node);
				case ExpressionType.Parameter:
					return this.VisitParameter ((ParameterExpression)node);
				case ExpressionType.Call:
					return this.VisitMethodCall((MethodCallExpression)node);
				case ExpressionType.Lambda:
					return this.VisitLambdaGeneric ((LambdaExpression)node);
				case ExpressionType.Constant:
				case ExpressionType.Convert:
					return node;
				default:
					return this.Visit (node);
				}
			}

            /// <summary>
            /// Visit method call
            /// </summary>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                switch(node.Method.Name)
                {
                    case "Contains":
                        {
                            var parmName = this.ExtractPath(node.Object);
                            object parmValue = this.ExtractValue(node.Arguments[0]);
                            this.m_query.Add(new KeyValuePair<String, Object>(parmName, "~" + parmValue.ToString()));
                            return null;
                        }
                    case "Any":
                        {
                            var parmName = this.ExtractPath(node.Arguments[0]);
                            // Process lambda
                            var result = new List<KeyValuePair<string, object>>();
                            var subQueryExpressionVisitor = new HttpQueryExpressionVisitor(result);
                            subQueryExpressionVisitor.Visit(node.Arguments[1]);

                            // Result
                            foreach (var itm in result)
                                this.m_query.Add(new KeyValuePair<string, object>(String.Format("{0}.{1}", parmName, itm.Key), itm.Value));
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
				this.Visit (node.Expression);
				return node;
			}

			/// <summary>
			/// Visits the lambda generic.
			/// </summary>
			/// <returns>The lambda generic.</returns>
			/// <param name="node">Node.</param>
			protected virtual Expression VisitLambdaGeneric(LambdaExpression node)
			{
				this.Visit (node.Body);
				return node;
			}

			/// <summary>
			/// Visit a binary expression which is in the form of A(operator)B
			/// </summary>
			/// <returns>The binary.</returns>
			/// <param name="node">Node.</param>
			protected override Expression VisitBinary (BinaryExpression node)
			{
				this.Visit (node.Left);
				this.Visit (node.Right);

				String parmName = this.ExtractPath (node.Left);
				Object parmValue = this.ExtractValue (node.Right);

				// Not able to map
				if (!String.IsNullOrEmpty (parmName) && parmValue != null) {
					Object fParmValue = parmValue;
					if (parmValue is DateTime)
						fParmValue = ((DateTime)parmValue).ToString ("o");
					else if (parmValue is DateTimeOffset)
						fParmValue = ((DateTimeOffset)parmValue).ToString ("o");

					// Node type
					switch (node.NodeType) {
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

					this.m_query.Add (new KeyValuePair<string, Object> (parmName, fParmValue));
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
					return this.ExtractValue (((UnaryExpression)access).Operand);
				else if (access.NodeType == ExpressionType.MemberAccess) {
					MemberExpression expr = access as MemberExpression;
					var expressionValue = this.ExtractValue (expr.Expression);
					if (expr.Member is PropertyInfo) {
						return (expr.Member as PropertyInfo).GetValue (expressionValue);
					} else if (expr.Member is FieldInfo) {
						return (expr.Member as FieldInfo).GetValue (expressionValue);
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
				if (access.NodeType == ExpressionType.MemberAccess) {
					MemberExpression memberExpr = access as MemberExpression;
					String path = this.ExtractPath (memberExpr.Expression); // get the chain if required

					// XML property?
					var memberInfo = memberExpr.Expression.Type.GetRuntimeProperty (memberExpr.Member.Name + "Xml") ??
					                 memberExpr.Member;

					// Is this a delay load?
					var delayLoadAttribute= memberExpr.Member.GetCustomAttribute<DelayLoadAttribute>();
                    var xmlIgnoreAttribute = memberExpr.Member.GetCustomAttribute<XmlIgnoreAttribute>();
					if (xmlIgnoreAttribute != null && delayLoadAttribute != null && !String.IsNullOrEmpty(delayLoadAttribute.KeyPropertyName))
						memberInfo = memberExpr.Expression.Type.GetRuntimeProperty (delayLoadAttribute.KeyPropertyName);

					// TODO: Delay and bound properties!!
					var memberXattribute = memberInfo.GetCustomAttribute<XmlElementAttribute>();
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
				return null;
			}
		}

		/// <summary>
		/// Builds the query dictionary .
		/// </summary>
		/// <returns>The query.</returns>
		/// <param name="model">Model.</param>
		/// <typeparam name="TModel">The 1st type parameter.</typeparam>
		public static IEnumerable<KeyValuePair<String, Object>> BuildQuery<TModel>(Expression<Func<TModel, bool>> model) 
		{
			List<KeyValuePair<String, Object>> retVal = new List<KeyValuePair<string, Object>> ();
			var visitor = new HttpQueryExpressionVisitor (retVal);
			visitor.Visit (model);
			return retVal;
		}

	}
}

