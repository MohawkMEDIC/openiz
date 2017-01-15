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

namespace OpenIZ.Persistence.Data.ADO.Providers
{
    /// <summary>
    /// Represents a IDbProvider for PostgreSQL
    /// </summary>
    public class PostgreSQLProvider : IDbProvider
    {
        
        // DB provider factory
        private DbProviderFactory m_provider = null;

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
            return this.CreateCommandInternal(context, CommandType.Text, finStmt.SQL, finStmt.Arguments);
        }
        
        /// <summary>
        /// Create command internally
        /// </summary>
        private IDbCommand CreateCommandInternal(DataContext context, CommandType type, String sql, params object[] parms) {

            var cmd = context.Connection.CreateCommand();
            cmd.CommandType = type;
            cmd.CommandText = sql;
            cmd.Transaction = context.Transaction;

            foreach(var itm in parms)
            {
                var parm = cmd.CreateParameter();

                // Parameter type
                if (itm is String) parm.DbType = System.Data.DbType.String;
                else if (itm is DateTime || itm is DateTimeOffset) parm.DbType = System.Data.DbType.DateTime;
                else if (itm is Int32) parm.DbType = System.Data.DbType.Int32;
                else if (itm is Boolean) parm.DbType = System.Data.DbType.Boolean;
                else if (itm is byte[])
                    parm.DbType = System.Data.DbType.Binary;
                else if (itm is Guid || itm is Guid?)
                    parm.DbType = System.Data.DbType.Guid;
                else if (itm is float || itm is double) parm.DbType = System.Data.DbType.Double;
                else if (itm is Decimal) parm.DbType = System.Data.DbType.Decimal;

                // Set value
                if (itm == null)
                    parm.Value = DBNull.Value;
                else
                    parm.Value = itm;

                parm.Direction = ParameterDirection.Input;
                
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

    }
}
