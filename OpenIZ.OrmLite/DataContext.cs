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
using System.Diagnostics;
using OpenIZ.OrmLite.Providers;
using OpenIZ.Core.Diagnostics;
using System.Threading;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Data.Warehouse;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Represents a data context
    /// </summary>
    public partial class DataContext : IDisposable
    {
        // Lock object
        private object m_lockObject = new object();

        // the connection
        private IDbConnection m_connection;

        // the transaction
        private IDbTransaction m_transaction;

        // The provider
        private IDbProvider m_provider;

        // Data dictionary
        private Dictionary<String, Object> m_dataDictionary = new Dictionary<string, object>();

        // Items to be added to cache after an action
        private Dictionary<Guid, IdentifiedData> m_cacheCommit = new Dictionary<Guid, IdentifiedData>();

        // Cached query
        private Dictionary<String, IEnumerable<Object>> m_cachedQuery = new Dictionary<string, IEnumerable<object>>();

        // Trace source
        private Tracer m_tracer = Tracer.GetTracer(typeof(DataContext));

        // Commands prepared on this connection
        private Dictionary<String, IDbCommand> m_preparedCommands = new Dictionary<string, IDbCommand>();

        /// <summary>
        /// Connection
        /// </summary>
        public IDbConnection Connection { get { return this.m_connection; } }

        /// <summary>
        /// Load state
        /// </summary>
        public LoadState LoadState { get; set; }

        /// <summary>
        /// Data dictionary
        /// </summary>
        public IDictionary<String, Object> Data { get { return this.m_dataDictionary; } }

        /// <summary>
        /// Cache on commit
        /// </summary>
        public IEnumerable<IdentifiedData> CacheOnCommit
        {
            get
            {
                return this.m_cacheCommit.Values;
            }
        }

        /// <summary>
        /// True if the context should prepare statements
        /// </summary>
        public bool PrepareStatements { get; set; }

        /// <summary>
        /// Current Transaction
        /// </summary>
        public IDbTransaction Transaction { get { return this.m_transaction; } }

        /// <summary>
        /// Query builder
        /// </summary>
        public QueryBuilder GetQueryBuilder(ModelMapper map)
        {
            return new QueryBuilder(map, this.m_provider);
        }


        /// <summary>
        /// Creates a new data context
        /// </summary>
        public DataContext(IDbProvider provider, IDbConnection connection)
        {
            this.m_provider = provider;
            this.m_connection = connection;
        }

        /// <summary>
        /// Creates a new data context
        /// </summary>
        public DataContext(IDbProvider provider, IDbConnection connection, bool isReadonly)
        {
            this.m_provider = provider;
            this.m_connection = connection;
            this.IsReadonly = isReadonly;
        }

        /// <summary>
        /// Creates a new data context
        /// </summary>
        public DataContext(IDbProvider provider, IDbConnection connection, IDbTransaction tx) : this(provider, connection)
        {
            this.m_transaction = tx;
        }

        /// <summary>
        /// Begin a transaction
        /// </summary>
        public IDbTransaction BeginTransaction()
        {
            if (this.m_transaction == null)
                this.m_transaction = this.m_connection.BeginTransaction();
            return this.m_transaction;
        }


        /// <summary>
        /// Get the datatype
        /// </summary>
        public String GetDataType(SchemaPropertyType type)
        {
            return this.m_provider.MapDatatype(type);
        }

        /// <summary>
        /// Open the connection to the database
        /// </summary>
        public void Open()
        {
            if (this.m_connection.State == ConnectionState.Closed)
                this.m_connection.Open();
            else if (this.m_connection.State == ConnectionState.Broken)
            {
                this.m_connection.Close();
                this.m_connection.Open();
            }
        }

        /// <summary>
        /// Add a prepared command
        /// </summary>
        internal void AddPreparedCommand(IDbCommand cmd)
        {
            if (!this.m_preparedCommands.ContainsKey(cmd.CommandText))
                lock (this.m_preparedCommands)
                    if (!this.m_preparedCommands.ContainsKey(cmd.CommandText)) // Check after lock
                        this.m_preparedCommands.Add(cmd.CommandText, cmd);

        }

        /// <summary>
        /// Opens a cloned context
        /// </summary>
        public DataContext OpenClonedContext()
        {
            if (this.Transaction != null)
                throw new InvalidOperationException("Cannot clone connection in transaction");
            var retVal = this.m_provider.CloneConnection(this);
            retVal.Open();
            retVal.m_dataDictionary = this.m_dataDictionary; // share data
            retVal.m_cachedQuery = this.m_cachedQuery;
            retVal.LoadState = this.LoadState;
            //retVal.PrepareStatements = this.PrepareStatements;
            return retVal;
        }

        /// <summary>
        /// Get prepared command
        /// </summary>
        internal IDbCommand GetPreparedCommand(string sql)
        {
            IDbCommand retVal = null;
            this.m_preparedCommands.TryGetValue(sql, out retVal);
            return retVal;
        }

        /// <summary>
        /// True if the command is prepared
        /// </summary>
        internal bool IsPreparedCommand(IDbCommand cmd)
        {
            IDbCommand retVal = null;
            return this.m_preparedCommands.TryGetValue(cmd.CommandText, out retVal) && retVal == cmd;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            if(this.m_preparedCommands != null)
                foreach (var itm in this.m_preparedCommands.Values)
                    itm?.Dispose();
            this.m_cacheCommit?.Clear();
            this.m_cacheCommit = null;
            this.m_cachedQuery?.Clear();
            this.m_cachedQuery = null;
            this.m_transaction?.Dispose();
            this.m_connection?.Dispose();
        }

        /// <summary>
        /// Add cache commit
        /// </summary>
        public void AddCacheCommit(IdentifiedData data)
        {
            try
            {
                IdentifiedData existing = null;
                if (data.Key.HasValue && !this.m_cacheCommit.TryGetValue(data.Key.Value, out existing))
                {
                    lock (this.m_lockObject)
                        // check again
                        if (!m_cacheCommit.ContainsKey(data.Key.Value))
                            this.m_cacheCommit.Add(data.Key.Value, data);
                }
                else if (data.Key.HasValue && data.LoadState > (existing?.LoadState ?? 0))
                    this.m_cacheCommit[data.Key.Value] = data;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceWarning("Object {0} won't be added to cache: {1}", data, e);
            }
        }


        /// <summary>
        /// Add cache commit
        /// </summary>
        public IdentifiedData GetCacheCommit(Guid key)
        {
            IdentifiedData retVal = null;
            lock(this.m_lockObject)
                this.m_cacheCommit.TryGetValue(key, out retVal);
            return retVal;
        }

        /// <summary>
        /// Create sql statement
        /// </summary>
        public SqlStatement CreateSqlStatement()
        {
            return new SqlStatement(this.m_provider);
        }

        /// <summary>
        /// Create sql statement
        /// </summary>
        public SqlStatement CreateSqlStatement(String sql, params object[] args)
        {
            return new SqlStatement(this.m_provider, sql, args);
        }

        /// <summary>
        /// Create SQL statement
        /// </summary>
        public SqlStatement<T> CreateSqlStatement<T>()
        {
            return new SqlStatement<T>(this.m_provider);
        }

        /// <summary>
        /// Query
        /// </summary>
        public String GetQueryLiteral(SqlStatement query)
        {
            StringBuilder retVal = new StringBuilder(query.SQL);
            String sql = retVal.ToString();
            var qList = query.Arguments.ToArray();
            int parmId = 0;
            while (sql.Contains("?"))
            {
                var pIndex = sql.IndexOf("?");
                retVal.Remove(pIndex, 1);
                var obj = qList[parmId++];
                if (obj is String || obj is Guid || obj is Guid? || obj is DateTime || obj is DateTimeOffset)
                    obj = $"'{obj}'";
                retVal.Insert(pIndex, obj);
                sql = retVal.ToString();
            }
            return retVal.ToString();
        }

        /// <summary>
        /// Add a cached set of query results
        /// </summary>
        public void AddQuery(SqlStatement domainQuery, IEnumerable<object> results)
        {
            var key = this.GetQueryLiteral(domainQuery);
            lock (this.m_cachedQuery)
                if (!this.m_cachedQuery.ContainsKey(key))
                    this.m_cachedQuery.Add(key, results);
        }

        /// <summary>
        /// Cache a query 
        /// </summary>
        public IEnumerable<Object> CacheQuery(SqlStatement domainQuery)
        {
            var key = this.GetQueryLiteral(domainQuery);
            IEnumerable<Object> retVal = null;
            this.m_cachedQuery.TryGetValue(key, out retVal);
            return retVal;
        }

    }
}
