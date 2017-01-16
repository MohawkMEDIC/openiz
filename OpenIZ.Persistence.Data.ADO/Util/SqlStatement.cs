using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OpenIZ.Persistence.Data.ADO.Util
{
    /// <summary>
    /// Represents a SQL statement builder tool
    /// </summary>
    public class SqlStatement
    {
        /// <summary>
        /// The SQL statement
        /// </summary>
        private string m_sql = null;

        /// <summary>
        /// RHS of the SQL statement
        /// </summary>
        protected SqlStatement m_rhs = null;

        /// <summary>
        /// Arguments for the SQL statement
        /// </summary>
        protected List<object> m_arguments = null;

        /// <summary>
        /// True if the sql statement is finalized
        /// </summary>
        public bool IsFinalized { get; private set; }

        /// <summary>
        /// Arguments for the SQL Statement
        /// </summary>
        public IEnumerable<Object> Arguments
        {
            get
            {
                return this.m_arguments;
            }
        }

        /// <summary>
        /// Gets the constructed or set SQL
        /// </summary>
        public string SQL { get { return this.m_sql; } }

        /// <summary>
        /// Creates a new empty SQL statement
        /// </summary>
        public SqlStatement()
        {
        }

        /// <summary>
        /// Create a new sql statement from the specified sql
        /// </summary>
        public SqlStatement(string sql, params object[] parms)
        {
            this.m_sql = sql;
            this.m_arguments = new List<object>(parms);
        }

        /// <summary>
        /// Append the SQL statement
        /// </summary>
        public SqlStatement Append(SqlStatement sql)
        {
            if (this.IsFinalized) throw new InvalidOperationException();

            if (this.m_rhs != null)
                this.m_rhs.Append(sql);
            else
                this.m_rhs = sql;
            return this;
        }

        /// <summary>
        /// Append the specified SQL
        /// </summary>
        public SqlStatement Append(string sql, params object[] parms)
        {
            return this.Append(new SqlStatement(sql, parms));
        }

        /// <summary>
        /// Build the special SQL statement
        /// </summary>
        /// <returns></returns>
        public SqlStatement Build()
        {
            StringBuilder sb = new StringBuilder();

            // Parameters
            List<object> parameters = new List<object>();

            var focus = this;
            do
            {
                sb.Append(focus.m_sql);
                // Add parms
                if(focus.Arguments != null)
                    parameters.AddRange(focus.Arguments);
                focus = focus.m_rhs;
            } while (focus != null);

            return new SqlStatement(sb.ToString(), parameters.ToArray());
        }

        /// <summary>
        /// Where clause
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public SqlStatement Where(SqlStatement clause)
        {
            if (String.IsNullOrEmpty(clause.SQL) && clause.m_rhs == null)
                return this;
            return this.Append(new SqlStatement("WHERE ").Append(clause));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where(String whereClause, params object[] args)
        {
            return this.Where(new SqlStatement(whereClause, args));
        }

        /// <summary>
        /// Append an AND condition
        /// </summary>
        public SqlStatement And(SqlStatement clause)
        {
            if (String.IsNullOrEmpty(this.m_sql) && this.m_rhs == null)
                return this.Append(clause);
            else
                return this.Append( new SqlStatement("AND (").Append(clause).Append(") "));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement And(String clause, params object[] args)
        {
            return this.And(new SqlStatement(clause, args));
        }

        /// <summary>
        /// Append an AND condition
        /// </summary>
        public SqlStatement Or(SqlStatement clause)
        {
            return this.Append(new SqlStatement("OR (").Append(clause).Append(") "));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Or(String clause, params object[] args)
        {
            return this.And(new SqlStatement(clause, args));
        }

        /// <summary>
        /// Inner join
        /// </summary>
        public SqlStatement InnerJoin<TLeft, TRight>(Expression<Func<TLeft, dynamic>> leftColumn, Expression<Func<TRight, dynamic>> rightColumn)
        {
            var leftMap = TableMapping.Get(typeof(TLeft));
            var rightMap = TableMapping.Get(typeof(TRight));
            var joinStatement = this.Append($"INNER JOIN {rightMap.TableName} ON");
            var rhsPk = rightMap.GetColumn(this.GetMember(rightColumn.Body));
            var lhsPk = leftMap.GetColumn(this.GetMember(leftColumn.Body));
            return joinStatement.Append($"({lhsPk.Table.TableName}.{lhsPk.Name} = {rhsPk.Table.TableName}.{rhsPk.Name}) ");
        }

        /// <summary>
        /// Get member information from lambda
        /// </summary>
        protected MemberInfo GetMember(Expression expression)
        {
            if (expression is MemberExpression) return (expression as MemberExpression).Member;
            else if (expression is UnaryExpression) return this.GetMember((expression as UnaryExpression).Operand);
            else throw new InvalidOperationException($"{expression} not supported, please use a member access expression");
        }

        /// <summary>
        /// Removes the last statement from the list
        /// </summary>
        internal void RemoveLast()
        {
            var t = this;
            while (t.m_rhs?.m_rhs != null)
                t = t.m_rhs;
            if(t != null)
                t.m_rhs = null;
        }
    }

    /// <summary>
    /// Represents a strongly typed SQL Statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlStatement<T> : SqlStatement
    {

        // The alias of the table if needed
        private String m_alias;

        /// <summary>
        /// Gets the table type
        /// </summary>
        public Type TableType { get { return typeof(T); } }

        /// <summary>
        /// Creates a new empty SQL statement
        /// </summary>
        public SqlStatement()
        {
        }

        /// <summary>
        /// Create a new sql statement from the specified sql
        /// </summary>
        public SqlStatement(string sql, params object[] parms) : base(sql, parms)
        {
        }

        /// <summary>
        /// Append the SQL statement
        /// </summary>
        public SqlStatement<T> Append(SqlStatement<T> sql)
        {
            if (this.IsFinalized) throw new InvalidOperationException();

            if (this.m_rhs != null)
                this.m_rhs.Append(sql);
            else
                this.m_rhs = sql;
            return this;
        }

        /// <summary>
        /// Inner join
        /// </summary>
        public SqlStatement<T> InnerJoin<TRight>(Expression<Func<T, dynamic>> leftColumn, Expression<Func<TRight, dynamic>> rightColumn)
        {
            var leftMap = TableMapping.Get(typeof(T));
            var rightMap = TableMapping.Get(typeof(TRight));
            var joinStatement = this.Append($"INNER JOIN {rightMap.TableName} ON ");
            var rhsPk = rightMap.GetColumn(this.GetMember(rightColumn.Body));
            var lhsPk = leftMap.GetColumn(this.GetMember(leftColumn.Body));
            var retVal = new SqlStatement<T>();
            retVal.Append(joinStatement).Append($"({lhsPk.Table.TableName}.{lhsPk.Name} = {rhsPk.Table.TableName}.{rhsPk.Name}) ");
            return retVal;
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where(Expression<Func<T, bool>> expression)
        {
            var tableMap = TableMapping.Get(typeof(T));
            var queryBuilder = new SqlQueryExpressionBuilder(this.m_alias ?? tableMap.TableName);
            queryBuilder.Visit(expression.Body);
            return this.Append(new SqlStatement("WHERE ").Append(queryBuilder.SqlStatement));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where<TExpression>(Expression<Func<TExpression, bool>> expression)
        {
            var tableMap = TableMapping.Get(typeof(TExpression));
            var queryBuilder = new SqlQueryExpressionBuilder(this.m_alias ?? tableMap.TableName);
            queryBuilder.Visit(expression.Body);
            return this.Append(new SqlStatement("WHERE ").Append(queryBuilder.SqlStatement));
        }

        /// <summary>
        /// Appends an inner join
        /// </summary>
        public SqlStatement<TReturn> InnerJoin<TJoinTable, TReturn>()
        {
            var tableMap = TableMapping.Get(typeof(TJoinTable));
            var joinStatement = this.Append($"INNER JOIN {tableMap.TableName} ON ");
            var rhsPk = tableMap.Columns.SingleOrDefault(o => o.IsPrimaryKey);
            var lhsPk = TableMapping.Get(typeof(T)).Columns.SingleOrDefault(o => o.ForeignKey?.Column == rhsPk.SourceProperty.Name && o.ForeignKey?.Table == tableMap.OrmType);
            if (lhsPk == null || rhsPk == null) throw new InvalidOperationException("PK not found");
            joinStatement.Append($"({lhsPk.Table.TableName}.{lhsPk.Name} = {rhsPk.Table.TableName}.{rhsPk.Name}) ");
            var retVal = new SqlStatement<TReturn>();
            retVal.Append(joinStatement);
            return retVal;
        }

        /// <summary>
        /// Create a delete from
        /// </summary>
        public SqlStatement<T> DeleteFrom()
        {
            var tableMap = TableMapping.Get(typeof(T));
            return this.Append(new SqlStatement<T>($"DELETE FROM {tableMap.TableName} "));
        }

        /// <summary>
        /// Construct a SELECT FROM statement
        /// </summary>
        public SqlStatement<T> SelectFrom(String tableAlias = null)
        {
            var tableMap = TableMapping.Get(typeof(T));
            return this.Append(new SqlStatement<T>($"SELECT * FROM {tableMap.TableName} AS {tableAlias ?? tableMap.TableName} ")
            {
                m_alias = tableAlias ?? tableMap.TableName
            });
        }

        /// <summary>
        /// Generate an update statement
        /// </summary>
        public SqlStatement<T> UpdateSet()
        {
            var tableMap = TableMapping.Get(typeof(T));
            return this.Append(new SqlStatement<T>($"UPDATE {tableMap.TableName} SET "));
        }
    }
}