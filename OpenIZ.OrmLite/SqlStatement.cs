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
 * Date: 2017-1-21
 */
using OpenIZ.Core.Model.Map;
using OpenIZ.OrmLite.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Represents a SQL statement builder tool
    /// </summary>
    public class SqlStatement
    {

        // Provider
        protected IDbProvider m_provider = null;

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
        public SqlStatement(IDbProvider provider)
        {
            this.m_provider = provider;
        }

        /// <summary>
        /// Create a new sql statement from the specified sql
        /// </summary>
        public SqlStatement(IDbProvider provider, string sql, params object[] parms) : this(provider)
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
            return this.Append(new SqlStatement(this.m_provider, sql, parms));
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

            return new SqlStatement(this.m_provider, sb.ToString(), parameters.ToArray());
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
            return this.Append(new SqlStatement(this.m_provider, "WHERE ").Append(clause));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where(String whereClause, params object[] args)
        {
            return this.Where(new SqlStatement(this.m_provider, whereClause, args));
        }

        /// <summary>
        /// Append an AND condition
        /// </summary>
        public SqlStatement And(SqlStatement clause)
        {
            if (String.IsNullOrEmpty(this.m_sql) && this.m_rhs == null)
                return this.Append(clause);
            else
                return this.Append(" AND ").Append(clause.Build());
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement And(String clause, params object[] args)
        {
            return this.And(new SqlStatement(this.m_provider, clause, args));
        }

        /// <summary>
        /// Append an AND condition
        /// </summary>
        public SqlStatement Or(SqlStatement clause)
        {
            if (String.IsNullOrEmpty(this.m_sql) && this.m_rhs == null)
                return this.Append(clause);
            else
                return this.Append(new SqlStatement(this.m_provider, " OR ")).Append(clause.Build());
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Or(String clause, params object[] args)
        {
            return this.And(new SqlStatement(this.m_provider, clause, args));
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
        /// Inner join left and right
        /// </summary>
        public SqlStatement InnerJoin(Type tLeft, Type tRight)
        {
            var tableMap = TableMapping.Get(tRight);
            var joinStatement = this.Append($"INNER JOIN {tableMap.TableName} ON ");

            // For RHS we need to find a column which references the tLEFT table ... 
            var rhsPk = tableMap.Columns.SingleOrDefault(o => o.ForeignKey?.Table == tLeft);
            ColumnMapping lhsPk = null;
            if (rhsPk == null) // look for primary key instead
            {
                rhsPk = tableMap.Columns.SingleOrDefault(o => o.IsPrimaryKey);
                lhsPk = TableMapping.Get(tLeft).Columns.SingleOrDefault(o => o.ForeignKey?.Table == rhsPk.Table.OrmType && o.ForeignKey?.Column == rhsPk.SourceProperty.Name);
            }
            else
                lhsPk = TableMapping.Get(rhsPk.ForeignKey.Table).GetColumn(rhsPk.ForeignKey.Column);

            if (lhsPk == null || rhsPk == null) // Try a natural join
            {
                rhsPk = tableMap.Columns.SingleOrDefault(o => TableMapping.Get(tLeft).Columns.Any(l=>o.Name == l.Name));
                lhsPk = TableMapping.Get(tLeft).Columns.SingleOrDefault(o => o.Name == rhsPk?.Name);
                if(rhsPk == null || lhsPk == null)
                    throw new InvalidOperationException("Unambiguous linked keys not found");
            }
            joinStatement.Append($"({lhsPk.Table.TableName}.{lhsPk.Name} = {rhsPk.Table.TableName}.{rhsPk.Name}) ");
            return joinStatement;
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
        /// Return a select from
        /// </summary>
        public SqlStatement SelectFrom(Type dataType)
        {
            var tableMap = TableMapping.Get(dataType);
            return this.Append(new SqlStatement(this.m_provider, $"SELECT * FROM {tableMap.TableName} AS {tableMap.TableName} "));
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where<TExpression>(Expression<Func<TExpression, bool>> expression)
        {
            var tableMap = TableMapping.Get(typeof(TExpression));
            var queryBuilder = new SqlQueryExpressionBuilder(tableMap.TableName, this.m_provider);
            queryBuilder.Visit(expression.Body);
            return this.Append(new SqlStatement(this.m_provider, "WHERE ").Append(queryBuilder.SqlStatement));
        }

        internal void Append(object like)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Expression
        /// </summary>
        public SqlStatement And<TExpression>(Expression<Func<TExpression, bool>> expression)
        {
            var tableMap = TableMapping.Get(typeof(TExpression));
            var queryBuilder = new SqlQueryExpressionBuilder(tableMap.TableName, this.m_provider);
            queryBuilder.Visit(expression.Body);
            return this.And(queryBuilder.SqlStatement);
        }

        /// <summary>
        /// Append an offset statement
        /// </summary>
        public SqlStatement Offset(int offset)
        {
            return this.Append($" OFFSET {offset} ");
        }

        /// <summary>
        /// Limit of the 
        /// </summary>
        /// <param name="value"></param>
        public SqlStatement Limit(int limit)
        {
            return this.Append($" LIMIT {limit} ");
        }

        /// <summary>
        /// Construct an order by 
        /// </summary>
        public SqlStatement OrderBy<TExpression>(Expression<Func<TExpression, dynamic>> orderField, SortOrderType sortOperation = SortOrderType.OrderBy)
        {
            var orderMap = TableMapping.Get(typeof(TExpression));
            var orderCol = orderMap.GetColumn(this.GetMember(orderField.Body));
            return this.Append($"ORDER BY {orderMap.TableName}.{orderCol.Name} ").Append(sortOperation == SortOrderType.OrderBy ? " ASC " : " DESC ");
        }


        /// <summary>
        /// Removes the last statement from the list
        /// </summary>
        public void RemoveLast()
        {
            var t = this;
            while (t.m_rhs?.m_rhs != null)
                t = t.m_rhs;
            if(t != null)
                t.m_rhs = null;
        }

        /// <summary>
        /// Represent as string
        /// </summary>
        public override string ToString()
        {
            return this.Build().SQL;
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
        public SqlStatement(IDbProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Create a new sql statement from the specified sql
        /// </summary>
        public SqlStatement(IDbProvider provider, string sql, params object[] parms) : base(provider, sql, parms)
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
            var retVal = new SqlStatement<T>(this.m_provider);
            retVal.Append(joinStatement).Append($"({lhsPk.Table.TableName}.{lhsPk.Name} = {rhsPk.Table.TableName}.{rhsPk.Name}) ");
            return retVal;
        }

        /// <summary>
        /// Construct a where clause on the expression tree
        /// </summary>
        public SqlStatement Where(Expression<Func<T, bool>> expression)
        {
            var tableMap = TableMapping.Get(typeof(T));
            var queryBuilder = new SqlQueryExpressionBuilder(this.m_alias ?? tableMap.TableName, this.m_provider);
            queryBuilder.Visit(expression.Body);
            return this.Append(new SqlStatement(this.m_provider, "WHERE ").Append(queryBuilder.SqlStatement));
        }

        /// <summary>
        /// Appends an inner join
        /// </summary>
        public SqlStatement<TReturn> InnerJoin<TJoinTable, TReturn>()
        {
            var retVal = new SqlStatement<TReturn>(this.m_provider);
            retVal.Append(this).InnerJoin(typeof(T), typeof(TJoinTable));
            return retVal;
        }

        /// <summary>
        /// Create a delete from
        /// </summary>
        public SqlStatement<T> DeleteFrom()
        {
            var tableMap = TableMapping.Get(typeof(T));
            return this.Append(new SqlStatement<T>(this.m_provider, $"DELETE FROM {tableMap.TableName} "));
        }

        /// <summary>
        /// Construct a SELECT FROM statement
        /// </summary>
        public SqlStatement<T> SelectFrom(String tableAlias = null)
        {
            var tableMap = TableMapping.Get(typeof(T));
            return this.Append(new SqlStatement<T>(this.m_provider, $"SELECT * FROM {tableMap.TableName} AS {tableAlias ?? tableMap.TableName} ")
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
            return this.Append(new SqlStatement<T>(this.m_provider, $"UPDATE {tableMap.TableName} SET "));
        }
    }
}