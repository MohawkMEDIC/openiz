using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Util
{
    /// <summary>
    /// Postgresql query expression builder
    /// </summary>
    public class SqlQueryExpressionBuilder : ExpressionVisitor
    {
        private string m_tableAlias = null;
        private SqlStatement m_sqlStatement = null;

        /// <summary>
        /// Gets the constructed SQL statement
        /// </summary>
        public SqlStatement SqlStatement { get { return this.m_sqlStatement.Build(); } }

        /// <summary>
        /// Creates a new postgresql query expression builder
        /// </summary>
        public SqlQueryExpressionBuilder(String alias)
        {
            this.m_tableAlias = alias;
            this.m_sqlStatement = new SqlStatement();
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
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)node);
                case ExpressionType.Convert:
                case ExpressionType.Not:
                case ExpressionType.Negate:
                    return this.VisitUnary((UnaryExpression)node);
                default:
                    return this.Visit(node);
            }
        }

        /// <summary>
        /// Visit constant
        /// </summary>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            this.m_sqlStatement.Append(" ? ", node.Value);
            return node;
        }

        /// <summary>
        /// Visits a unary member expression
        /// </summary>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Negate:
                    this.m_sqlStatement.Append(" -");
                    break;
                case ExpressionType.Not:
                    this.m_sqlStatement.Append(" NOT ");
                    break;
                case ExpressionType.Convert:
                    break;
                default:
                    return null;
            }

            this.Visit(node.Operand);
            return node;
        }

        /// <summary>
        /// Visit binary expression
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            this.m_sqlStatement.Append("(");
            this.Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    this.m_sqlStatement.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    this.m_sqlStatement.Append(" <> ");
                    break;
                case ExpressionType.GreaterThan:
                    this.m_sqlStatement.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.m_sqlStatement.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    this.m_sqlStatement.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.m_sqlStatement.Append(" <= ");
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.m_sqlStatement.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.m_sqlStatement.Append(" OR ");
                    break;
            }

            this.Visit(node.Right);
            this.m_sqlStatement.Append(")");
            return node;
        }

        /// <summary>
        /// Visit a parameter reference
        /// </summary>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            var tableMap = TableMapping.Get(node.Type);
            this.m_sqlStatement.Append($"{this.m_tableAlias ?? tableMap.TableName}");
            return node;
        }

        /// <summary>
        /// Visit method call
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {

            // Method names
            switch (node.Method.Name)
            {
                case "Contains":
                    this.Visit(node.Object);
                    this.m_sqlStatement.Append(" LIKE '%' || ");
                    this.Visit(node.Arguments[0]);
                    this.m_sqlStatement.Append(" || '%' ");
                    break;
                case "ToLower":
                    this.m_sqlStatement.Append("LOWER(");
                    this.Visit(node.Object);
                    this.m_sqlStatement.Append(")");
                    break;
                case "ToUpper":
                    this.m_sqlStatement.Append("LOWER(");
                    this.Visit(node.Object);
                    this.m_sqlStatement.Append(")");
                    break;
                case "NewGuid":
                    this.m_sqlStatement.Append("uuid_generate_v4()");
                    break;
                default:
                    throw new NotSupportedException(node.Method.Name);
            }
            return node;
        }


        /// <summary>
        /// Visit member access
        /// </summary>
        private Expression VisitMemberAccess(MemberExpression node)
        {
            // Member expression is node... This has the limitation of only going one deep :/
            if(node.Expression != null)
                switch (node.Expression.NodeType)
                {
                    case ExpressionType.Parameter:

                        // Translate
                        var tableMap = TableMapping.Get(node.Expression.Type);
                        var columnMap = tableMap.GetColumn(node.Member);
                        this.Visit(node.Expression);
                        // Now write out the expression
                        this.m_sqlStatement.Append($".{columnMap.Name}");
                        break;
                    case ExpressionType.Constant:

                        // Ok, this is a constant member access.. so ets get the value
                        var cons = (node.Expression as ConstantExpression).Value;
                        if (node.Member is PropertyInfo)
                            this.m_sqlStatement.Append(" ? ", (node.Member as PropertyInfo).GetValue(cons));
                        else if (node.Member is FieldInfo)
                            this.m_sqlStatement.Append(" ? ", (node.Member as FieldInfo).GetValue(cons));
                        else
                            throw new NotSupportedException();
                        break;
                }
            else // constant expression
            {
                switch(node.Member.Name)
                {
                    case "Now":
                        this.m_sqlStatement.Append("CURRENT_TIMESTAMP");
                        break;
                    case "NewGuid":
                        this.m_sqlStatement.Append("uuid_generate_v4()");
                        break;
                    default:
                        if (node.Member is PropertyInfo)
                            this.m_sqlStatement.Append(" ? ", (node.Member as PropertyInfo).GetValue(null));
                        else if (node.Member is FieldInfo)
                            this.m_sqlStatement.Append(" ? ", (node.Member as FieldInfo).GetValue(null));
                        break;
                }
            }
            return node;
        }
    }

}
