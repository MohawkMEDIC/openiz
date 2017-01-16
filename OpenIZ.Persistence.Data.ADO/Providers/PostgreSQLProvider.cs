using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Persistence.Data.ADO.Util;
using System.Configuration;
using System.Data.Common;
using OpenIZ.Persistence.Data.ADO.Data;
using System.Linq.Expressions;
using System.Diagnostics;

namespace OpenIZ.Persistence.Data.ADO.Providers
{
    /// <summary>
    /// Represents a IDbProvider for PostgreSQL
    /// </summary>
    public class PostgreSQLProvider : IDbProvider
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(AdoDataConstants.TraceSourceName);

        // DB provider factory
        private DbProviderFactory m_provider = null;

        /// <summary>
        /// Trace SQL commands
        /// </summary>
        public bool TraceSql { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public ConnectionStringSettings ReadonlyConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public ConnectionStringSettings ConnectionString { get; set; }

        /// <summary>
        /// Get provider factory
        /// </summary>
        /// <returns></returns>
        private DbProviderFactory GetProviderFactory()
        {
            if (this.m_provider == null)
                this.m_provider = DbProviderFactories.GetFactory(this.ConnectionString.ProviderName);
            return this.m_provider;
        }

        /// <summary>
        /// Gets a readonly connection
        /// </summary>
        public DataContext GetReadonlyConnection()
        {
            var conn = this.GetProviderFactory().CreateConnection();
            conn.ConnectionString = this.ReadonlyConnectionString.ConnectionString;
            return new DataContext(this, conn);
        }

        /// <summary>
        /// Get a connection that can be written to
        /// </summary>
        /// <returns></returns>
        public DataContext GetWriteConnection()
        {
            var conn = this.GetProviderFactory().CreateConnection();
            conn.ConnectionString = this.ConnectionString.ConnectionString;
            return new DataContext(this, conn);
        }

        /// <summary>
        /// Create a command
        /// </summary>
        public IDbCommand CreateCommand(DataContext context, SqlStatement stmt)
        {
            var finStmt = stmt.Build();
            return this.CreateCommandInternal(context, CommandType.Text, finStmt.SQL, finStmt.Arguments.ToArray());
        }

        /// <summary>
        /// Create command internally
        /// </summary>
        private IDbCommand CreateCommandInternal(DataContext context, CommandType type, String sql, params object[] parms)
        {

            var pno = 0;
            while (sql.Contains("?"))
            {
                var idx = sql.IndexOf("?");
                sql = sql.Remove(idx, 1).Insert(idx, $"@parm{pno++}");
            }

            if (pno !=  parms.Length)
                throw new ArgumentOutOfRangeException(nameof(sql), $"Parameter mismatch query expected {pno} but {parms.Length} supplied");

            var cmd = context.Connection.CreateCommand();
            cmd.CommandType = type;
            cmd.CommandText = sql;
            cmd.Transaction = context.Transaction;

            if (this.TraceSql)
                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "[{0}] {1}", type, sql);

            pno = 0;
            foreach (var itm in parms)
            {
                var parm = cmd.CreateParameter();
                var value = itm;

                // Parameter type
                if (value is String) parm.DbType = System.Data.DbType.String;
                else if (value is DateTime || value is DateTimeOffset) parm.DbType = System.Data.DbType.DateTime;
                else if (value is Int32) parm.DbType = System.Data.DbType.Int32;
                else if (value is Boolean) parm.DbType = System.Data.DbType.Boolean;
                else if (value is byte[])
                    parm.DbType = System.Data.DbType.Binary;
                else if (value is Guid || value is Guid?)
                    parm.DbType = System.Data.DbType.Guid;
                else if (value is float || value is double) parm.DbType = System.Data.DbType.Double;
                else if (value is Decimal) parm.DbType = System.Data.DbType.Decimal;

                // Set value
                if (itm == null)
                    parm.Value = DBNull.Value;
                else
                    parm.Value = itm;

                parm.ParameterName = $"parm{pno++}";
                parm.Direction = ParameterDirection.Input;

                if (this.TraceSql)
                    this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "\t [{0}] {1} ({2})", cmd.Parameters.Count, parm.Value, parm.DbType);


                cmd.Parameters.Add(parm);
            }

            return cmd;
        }

        /// <summary>
        /// Create a stored procedure command
        /// </summary>
        public IDbCommand CreateStoredProcedureCommand(DataContext context, string spName, params object[] parms)
        {
            return this.CreateCommandInternal(context, CommandType.StoredProcedure, spName, parms);
        }

        /// <summary>
        /// Create a command from string sql
        /// </summary>
        public IDbCommand CreateCommand(DataContext context, string sql, params object[] parms)
        {
            return this.CreateCommandInternal(context, CommandType.Text, sql, parms);
        }

        /// <summary>
        /// Construct an ORDER BY statement
        /// </summary>
        public SqlStatement OrderBy(SqlStatement sql, string columnName)
        {
            return sql.Append($"ORDER BY {columnName} ASC ");
        }

        /// <summary>
        /// Construct an ORDER BY DESCENDING statement
        /// </summary>
        public SqlStatement OrderByDescending(SqlStatement sql, string columnName)
        {
            return sql.Append($"ORDER BY {columnName} DESC ");
        }

        /// <summary>
        /// Construct an appropriate TAKE function
        /// </summary>
        public SqlStatement Take(SqlStatement sql, int quantity)
        {
            return sql.Append($"LIMIT {quantity} ");

        }

        /// <summary>
        /// Construct an appropriate SKIP function
        /// </summary>
        public SqlStatement Skip(SqlStatement sql, int offset)
        {
            return sql.Append($"OFFSET {offset} ");
        }

        /// <summary>
        /// Return exists
        /// </summary>
        public SqlStatement Exists(SqlStatement sqlStatement)
        {
            return new SqlStatement("SELECT CASE WHEN EXISTS (").Append(sqlStatement).Append(") THEN true ELSE false END");
        }

        /// <summary>
        /// Append a returning statement
        /// </summary>
        public SqlStatement Returning(SqlStatement sqlStatement, params ColumnMapping[] returnColumns)
        {
            if (returnColumns.Length == 0)
                return sqlStatement;
            return sqlStatement.Append($" RETURNING {String.Join(",", returnColumns.Select(o => o.Name))}");
        }
    }
}
