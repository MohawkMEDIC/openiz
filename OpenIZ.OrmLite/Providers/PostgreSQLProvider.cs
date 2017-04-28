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

namespace OpenIZ.OrmLite.Providers
{
    /// <summary>
    /// Represents a IDbProvider for PostgreSQL
    /// </summary>
    public class PostgreSQLProvider : IDbProvider
    {

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
                return "pgsql";
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
                if (context.PrepareStatements)
                {
                    if (!cmd.Parameters.OfType<IDataParameter>().Any(o => o.DbType == DbType.Object))
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
