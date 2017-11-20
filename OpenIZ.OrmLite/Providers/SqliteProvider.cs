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
 * Date: 2017-2-2
 */
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Map;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Serialization;
using OpenIZ.Core.Model;
using System.Data.Common;
using OpenIZ.Core.Data.Warehouse;

namespace OpenIZ.OrmLite.Providers
{
    /// <summary>
    /// SQL Lite provider
    /// </summary>
    public class SqliteProvider : IDbProvider
    {

        // Provider
        private DbProviderFactory m_provider = null;

        // Lock object as only one thread can access SQLite
        private Dictionary<String, Object> m_locks = new Dictionary<string, object>();

        // Tracer for the objects
        private Tracer m_tracer = Tracer.GetTracer(typeof(SqliteProvider));

        /// <summary>
        /// Gets or sets the connection string for this provier
        /// </summary>
        public String ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the readonly connection string
        /// </summary>
        public String ReadonlyConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the trace SQL flag
        /// </summary>
        public bool TraceSql { get; set; }

        /// <summary>
        /// Features of the SQL engine
        /// </summary>
        public SqlEngineFeatures Features
        {
            get
            {
                return SqlEngineFeatures.AutoGenerateTimestamps;
            }
        }

        /// <summary>
        /// Get the name of the provider
        /// </summary>
        public string Name
        {
            get
            {
                return "sqlite";
            }
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
        /// Create a command from the specified contxt with sql statement
        /// </summary>
        public IDbCommand CreateCommand(DataContext context, SqlStatement stmt)
        {
            var c = stmt.Build();
            return this.CreateCommandInternal(context, CommandType.Text, c.SQL, c.Arguments.ToArray());
        }

        /// <summary>
        /// Create command from string and params
        /// </summary>
        public IDbCommand CreateCommand(DataContext context, string sql, params object[] parms)
        {
            return this.CreateCommandInternal(context, CommandType.Text, sql, parms);
        }

        /// <summary>
        /// Create stored procedure command
        /// </summary>
        public IDbCommand CreateStoredProcedureCommand(DataContext context, string spName, params object[] parms)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create command internally
        /// </summary>
        private IDbCommand CreateCommandInternal(DataContext context, CommandType type, String sql, params object[] parms)
        {

            var cmd = context.Connection.CreateCommand();
            cmd.CommandType = type;
            cmd.CommandText = sql.Replace("ILIKE", "LIKE");
            cmd.Transaction = context.Transaction;

            if (this.TraceSql)
                this.m_tracer.TraceVerbose("[{0}] {1}", type, sql);

            foreach (var itm in parms)
            {
                var parm = cmd.CreateParameter();
                var value = itm;

                // Parameter type
                if (value is String) parm.DbType = System.Data.DbType.String;
                else if (value is DateTime)
                {
                    parm.DbType = System.Data.DbType.Int32;
                    if (itm != null)
                        parm.Value = this.ConvertValue(itm, typeof(Int32));
                }
                else if (value is DateTimeOffset)
                {
                    parm.DbType = DbType.Int32;
                    if (itm != null)
                        parm.Value = this.ConvertValue(itm, typeof(Int32));
                }
                else if (value is Int32) parm.DbType = System.Data.DbType.Int32;
                else if (value is Boolean) parm.DbType = System.Data.DbType.Boolean;
                else if (value is byte[])
                    parm.DbType = System.Data.DbType.Binary;
                else if (value is Guid || value is Guid?)
                {
                    parm.DbType = System.Data.DbType.Binary;
                    if (itm != null)
                        parm.Value = ((Guid)itm).ToByteArray();
                }
                else if (value is float || value is double) parm.DbType = System.Data.DbType.Double;
                else if (value is Decimal) parm.DbType = System.Data.DbType.Decimal;

                // Set value
                if (itm == null)
                    parm.Value = DBNull.Value;
                else if (parm.Value == null)
                    parm.Value = itm;

                parm.Direction = ParameterDirection.Input;

                if (this.TraceSql)
                    this.m_tracer.TraceVerbose("\t [{0}] {1} ({2})", cmd.Parameters.Count, parm.Value, parm.DbType);


                cmd.Parameters.Add(parm);
            }

            return cmd;
        }

        /// <summary>
        /// Gets read only connection
        /// </summary>
        public virtual DataContext GetReadonlyConnection()
        {
            if (this.m_provider == null)
                this.m_provider = Activator.CreateInstance(Type.GetType("System.Data.SQLite.SQLiteProviderFactory, System.Data.SQLite, Culture=nuetral")) as DbProviderFactory;
            var conn = this.m_provider.CreateConnection();
            conn.ConnectionString = this.ReadonlyConnectionString;
            return new DataContext(this, conn);
        }

        /// <summary>
        /// Get a write connection
        /// </summary>
        /// <returns></returns>
        public virtual DataContext GetWriteConnection()
        {
            if (this.m_provider == null)
                this.m_provider = Activator.CreateInstance(Type.GetType("System.Data.SQLite.SQLiteProviderFactory, System.Data.SQLite, Culture=nuetral")) as DbProviderFactory;
            var conn = this.m_provider.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return new DataContext(this, conn);
        }

        /// <summary>
        /// Returning statement
        /// </summary>
        public SqlStatement Returning(SqlStatement sqlStatement, params ColumnMapping[] returnColumns)
        {
            return sqlStatement;
        }

        /// <summary>
        /// Lock the specified connection
        /// </summary>
        public object Lock(IDbConnection connection)
        {
            Object _lock = null;
            if (!this.m_locks.TryGetValue(connection.ConnectionString, out _lock))
            {
                _lock = new object();
                lock (this.m_locks)
                    if (!this.m_locks.ContainsKey(connection.ConnectionString))
                        this.m_locks.Add(connection.ConnectionString, _lock);
                    else
                        return this.m_locks[connection.ConnectionString];
            }
            return _lock;
        }

        /// <summary>
        /// Convert object
        /// </summary>
        public object ConvertValue(Object value, Type toType)
        {
            object retVal = null;
            if (value == null)
                return null;
            else if (typeof(DateTime) == toType.StripNullable() &&
                (value is Int32 || value is Int64))
                retVal = OpenIZConvert.ParseDateTime(Convert.ToInt32(value));
            else if (typeof(DateTimeOffset) == toType.StripNullable() &&
                (value is Int32 || value is Int64))
                retVal = OpenIZConvert.ParseDateTimeOffset(Convert.ToInt32(value));
            else if (typeof(Int32) == toType.StripNullable() &&
                value is DateTime)
                retVal = OpenIZConvert.ToDateTime((DateTime)value);
            else if (typeof(Int32) == toType.StripNullable() &&
                value is DateTimeOffset)
                retVal = OpenIZConvert.ToDateTimeOffset((DateTimeOffset)value);
            else
                MapUtil.TryConvert(value, toType, out retVal);
            return retVal;
        }

        /// <summary>
        /// Clone this connection
        /// </summary>
        public DataContext CloneConnection(DataContext source)
        {
            if (this.m_provider == null)
                this.m_provider = Activator.CreateInstance(Type.GetType("System.Data.SQLite.SQLiteProviderFactory, System.Data.SQLite, Culture=nuetral")) as DbProviderFactory;
            var conn = this.m_provider.CreateConnection();
            conn.ConnectionString = source.Connection.ConnectionString;
            return new DataContext(this, conn);
        }

        /// <summary>
        /// Create SQL keyword
        /// </summary>
        public string CreateSqlKeyword(SqlKeyword keywordType)
        {
            switch (keywordType)
            {
                case SqlKeyword.ILike:
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
                    return "blob";
                case SchemaPropertyType.Boolean:
                    return "boolean";
                case SchemaPropertyType.Date:
                case SchemaPropertyType.TimeStamp:
                case SchemaPropertyType.DateTime:
                    return "bigint";
                case SchemaPropertyType.Decimal:
                    return "decimal";
                case SchemaPropertyType.Float:
                    return "float";
                case SchemaPropertyType.Integer:
                    return "integer";
                case SchemaPropertyType.String:
                    return "varchar(128)";
                case SchemaPropertyType.Uuid:
                    return "blob(16)";
                default:
                    return null;
            }
        }
    }
}
