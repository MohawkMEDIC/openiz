using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Persistence.Data.MSSQL.Linq
{
    /// <summary>
    /// Model conversion visitor is used to convert a lambda expression based on the business model 
    /// into a domain model lamda expression
    /// </summary>
    public class ModelConversionVisitor : ExpressionVisitor
    {

        /// <summary>
        /// Visit an expression
        /// </summary>
        public override Expression Visit(Expression node)
        {

            if (node == null)
                return node;

            switch(node.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)node);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)node);
                default:
                    return base.Visit(node);
            }
            return base.Visit(node);

        }

        /// <summary>
        /// Visit parameter
        /// </summary>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            IModelConverter converter = ModelConverterUtil.Current.GetModelConverter(node.Type);
            if (converter == null)
                throw new InvalidOperationException(String.Format("Cannot find type converter for {0}", node.Type.FullName));
            Type mappedType = converter.DomainType;
            if (mappedType != node.Type)
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
                IModelConverter converter = ModelConverterUtil.Current.GetModelConverter(newExpression.Type);
                if (converter == null)
                    throw new InvalidOperationException(String.Format("Cannot find type converter for {0}", newExpression.Type.FullName));
                return Expression.MakeMemberAccess(newExpression, converter.MapMember(node.Member));
            }
            return node;
        }
        
        /// <summary>
        /// Convert the specified lambda expression from model into query
        /// </summary>
        /// <param name="expression">The expression to be converted</param>
        public Expression<Func<TTo, bool>> Convert<TFrom, TTo>(Expression<Func<TFrom, bool>> expression)
        {
            Expression expr = this.Visit(expression.Body);
            return Expression.Lambda<Func<TTo, bool>>(expr, Expression.Parameter(typeof(TTo), expression.Parameters[0].Name));
        }
    }
}