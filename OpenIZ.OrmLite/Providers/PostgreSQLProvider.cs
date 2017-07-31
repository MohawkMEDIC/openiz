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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Linq.Expressions;
using System.Diagnostics;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Data.Warehouse;
using System.Net;
using System.Text.RegularExpressions;

namespace OpenIZ.OrmLite.Providers
{
    /// <summary>
    /// Represents a IDbProvider for PostgreSQL
    /// </summary>
    public class PostgreSQLProvider : IDbProvider
    {

        // Last rr host used
        private int m_lastRrHost = 0;

        // Readonly IP Addresses
        private IPAddress[] m_readonlyIpAddresses;

        // Trace source
        private Tracer m_tracer = Tracer.GetTracer(typeof(PostgreSQLProvider));

        // DB provider factory
        private DbProviderFactory m_provider = null;

        /// <summary>
        /// Trace SQL commands
        /// </summary>
        public bool TraceSql { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public String ReadonlyConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        public String ConnectionString { get; set; }

        /// <summary>
        /// SQL Engine features
        /// </summary>
        public SqlEngineFeatures Features
        {
            get
            {
                return SqlEngineFeatures.AutoGenerateGuids | SqlEngineFeatures.AutoGenerateTimestamps | SqlEngineFeatures.ReturnedInserts;
            }
        }

        /// <summary>
        /// Get name of provider
        /// </summary>
        public string Name
        {
            get
            {
                return "npgsql";
            }
        }

        /// <summary>
        /// Get provider factory
        /// </summary>
        /// <returns></returns>
        private DbProviderFactory GetProviderFactory()
        {
            if (this.m_provider == null) // HACK for Mono
                this.m_provider = typeof(DbProviderFactories).GetMethod("GetFactory", new Type[] { typeof(String) }).Invoke(null, new object[] { "Npgsql" }) as DbProviderFactory;

            if (this.m_provider == null)
                throw new InvalidOperationException("Missing Npgsql provider");
            return this.m_provider;
        }

        /// <summary>
        /// Gets a readonly connection
        /// </summary>
        public DataContext GetReadonlyConnection()
        {

            var conn = this.GetProviderFactory().CreateConnection();

            DbConnectionStringBuilder dbst = new DbConnectionStringBuilder();
            dbst.ConnectionString = this.ReadonlyConnectionString;
            Object host = String.Empty;
            if(this.m_readonlyIpAddresses == null && dbst.TryGetValue("host", out host) || dbst.TryGetValue("server", out host))
            {
                IPAddress ip = null;
                if (IPAddress.TryParse(host.ToString(), out ip)) // server is an IP, no need to dns
                    this.m_readonlyIpAddresses = new IPAddress[] { ip };
                else if (host.ToString() == "localhost") {
                    conn.ConnectionString = this.ReadonlyConnectionString;
                    return new DataContext(this, conn, true);
                }
                else
                    this.m_readonlyIpAddresses = Dns.GetHostAddresses(host.ToString());
                dbst.Remove("host");
                dbst.Remove("server");
                this.ReadonlyConnectionString = dbst.ConnectionString;
            }

            // Readonly IP address
            if(this.m_readonlyIpAddresses?.Length > 0)
            {
                dbst["server"] = this.m_readonlyIpAddresses[this.m_lastRrHost++ % this.m_readonlyIpAddresses.Length].ToString();
                if (this.m_lastRrHost > this.m_readonlyIpAddresses.Length) this.m_lastRrHost = 0;
                conn.ConnectionString = dbst.ConnectionString;
            }
            else
                conn.ConnectionString = this.ReadonlyConnectionString;

            return new DataContext(this, conn, true);
        }

        /// <summary>
        /// Get a connection that can be written to
        /// </summary>
        /// <returns></returns>
        public DataContext GetWriteConnection()
        {
            var conn = this.GetProviderFactory().CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return new DataContext(this, conn, false);
        }

        /// <summary>
        /// Create a command
        /// </summary>
        public IDbCommand CreateCommand(DataContext context, SqlStatement stmt)
        {
            var finStmt = stmt.Build();

#if DB_DEBUG
            if(System.Diagnostics.Debugger.IsAttached)
                this.Explain(context, CommandType.Text, finStmt.SQL, finStmt.Arguments.ToArray());
#endif 

            return this.CreateCommandInternal(context, CommandType.Text, finStmt.SQL, finStmt.Arguments.ToArray());
        }

        /// <summary>
        /// Perform an explain query
        /// </summary>
        private void Explain(DataContext context, CommandType text, string sQL, object[] v)
        {
            using (var cmd = this.CreateCommandInternal(context, CommandType.Text, "EXPLAIN " + sQL, v))
            using (var plan = cmd.ExecuteReader())
                while (plan.Read())
                {
                    if (plan.GetValue(0).ToString().Contains("Seq"))
                        System.Diagnostics.Debugger.Break();
                }
        }


        // Parameter regex
        private readonly Regex m_parmRegex = new Regex(@"\?");

        /// <summary>
        /// Create command internally
        /// </summary>
        private IDbCommand CreateCommandInternal(DataContext context, CommandType type, String sql, params object[] parms)
        {

            var pno = 0;
            
            sql = this.m_parmRegex.Replace(sql, o => $"@parm{pno++}");

            if (pno !=  parms.Length && type == CommandType.Text)
                throw new ArgumentOutOfRangeException(nameof(sql), $"Parameter mismatch query expected {pno} but {parms.Length} supplied");


            IDbCommand cmd = context.GetPreparedCommand(sql);
            if (cmd == null)
            {
                cmd = context.Connection.CreateCommand();
                cmd.Transaction = context.Transaction;
                cmd.CommandType = type;
                cmd.CommandText = sql;

                if (this.TraceSql)
                    this.m_tracer.TraceVerbose("[{0}] {1}", type, sql);

                pno = 0;
                foreach (var itm in parms)
                {
                    var parm = cmd.CreateParameter();
                    var value = itm;

                    // Parameter type
                    if (value is String) parm.DbType = System.Data.DbType.String;
                    else if (value is DateTime) parm.DbType = System.Data.DbType.DateTime;
                    else if (value is DateTimeOffset) parm.DbType = DbType.DateTimeOffset;
                    else if (value is Int32) parm.DbType = System.Data.DbType.Int32;
                    else if (value is Boolean) parm.DbType = System.Data.DbType.Boolean;
                    else if (value is byte[])
                        parm.DbType = System.Data.DbType.Binary;
                    else if (value is Guid || value is Guid?)
                        parm.DbType = System.Data.DbType.Guid;
                    else if (value is float || value is double) parm.DbType = System.Data.DbType.Double;
                    else if (value is Decimal) parm.DbType = System.Data.DbType.Decimal;
                    else if (value == null) parm.DbType = DbType.Object;
                    // Set value
                    if (itm == null)
                        parm.Value = DBNull.Value;
                    else
                        parm.Value = itm;

                    if (type == CommandType.Text)
                        parm.ParameterName = $"parm{pno++}";
                    parm.Direction = ParameterDirection.Input;

                    if (this.TraceSql)
                        this.m_tracer.TraceVerbose("\t [{0}] {1} ({2})", cmd.Parameters.Count, parm.Value, parm.DbType);


                    cmd.Parameters.Add(parm);
                }

                // Prepare command
                if (context.PrepareStatements && !cmd.CommandText.StartsWith("EXPLAIN"))
                {
                    if (!cmd.Parameters.OfType<IDataParameter>().Any(o => o.DbType == DbType.Object) &&
                        context.Transaction == null)
                        cmd.Prepare();

                    context.AddPreparedCommand(cmd);
                }
            }
            else
            {
                if (cmd.Parameters.Count != parms.Length)
                    throw new ArgumentOutOfRangeException(nameof(parms), "Argument count mis-match");

                for(int i = 0; i < parms.Length; i++)
                    (cmd.Parameters[i] as IDataParameter).Value = parms[i] ?? DBNull.Value; 
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
        /// Return exists
        /// </summary>
        public SqlStatement Count(SqlStatement sqlStatement)
        {
            return new SqlStatement(this, "SELECT COUNT(*) FROM (").Append(sqlStatement.Build()).Append(") Q0");
        }

        /// <summary>
        /// Return exists
        /// </summary>
        public SqlStatement Exists(SqlStatement sqlStatement)
        {
            return new SqlStatement(this, "SELECT CASE WHEN EXISTS (").Append(sqlStatement.Build()).Append(") THEN true ELSE false END");
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

        /// <summary>
        /// Gets a lock
        /// </summary>
        public object Lock(IDbConnection conn)
        {
            return new object();
        }

        /// <summary>
        /// Convert value just uses the mapper if needed
        /// </summary>
        public object ConvertValue(object value, Type toType)
        {
            object retVal = null;
            if(value != DBNull.Value)
                MapUtil.TryConvert(value, toType, out retVal);
            return retVal;
        }

        /// <summary>
        /// Create a new connection from an existing data source
        /// </summary>
        public DataContext CloneConnection(DataContext source)
        {
            return source.IsReadonly ? this.GetReadonlyConnection() : this.GetWriteConnection();
        }

        /// <summary>
        /// Create SQL keyword
        /// </summary>
        public string CreateSqlKeyword(SqlKeyword keywordType)
        {
            switch(keywordType)
            {
                case SqlKeyword.ILike:
                    return " ILIKE ";
                case SqlKeyword.Like:
                    return " LIKE ";
                case SqlKeyword.Lower:
                    return " LOWER ";
                case SqlKeyword.Upper:
                    return " UPPER ";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Map datatype
        /// </summary>
        public string MapDatatype(SchemaPropertyType type)
        {
            switch (type)
            {
                case SchemaPropertyType.Binary:
                    return "VARBINARY";
                case SchemaPropertyType.Boolean:
                    return "BOOLEAN";
                case SchemaPropertyType.Date:
                    return "DATE";
                case SchemaPropertyType.DateTime:
                    return "TIMESTAMP";
                case SchemaPropertyType.TimeStamp:
                    return "TIMESTAMPTZ";
                case SchemaPropertyType.Decimal:
                    return "DECIMAL";
                case SchemaPropertyType.Float:
                    return "FLOAT";
                case SchemaPropertyType.Integer:
                    return "INTEGER";
                case SchemaPropertyType.String:
                    return "VARCHAR(128)";
                case SchemaPropertyType.Uuid:
                    return "UUID";
                default:
                    return null;
            }
        }
    }
}
