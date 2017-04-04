using OpenIZ.Core.Model.Map;
using OpenIZ.OrmLite.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Multi type result used when a result set is a join
    /// </summary>
    public abstract class CompositeResult
    {

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public Object[] Values { get; protected set; }

        // Parse values
        public abstract void ParseValues(IDataReader rdr, IDbProvider provider);

        /// <summary>
        /// Parse the data
        /// </summary>
        protected TData Parse<TData>(IDataReader rdr, IDbProvider provider)
        {
            var tableMapping = TableMapping.Get(typeof(TData));
            dynamic result = Activator.CreateInstance(typeof(TData));
            // Read each column and pull from reader
            foreach (var itm in tableMapping.Columns)
            {
                try
                {
                    object value = provider.ConvertValue(rdr[itm.Name], itm.SourceProperty.PropertyType);
                    itm.SourceProperty.SetValue(result, value);
                }
                catch (Exception e)
                {
                    throw new MissingFieldException(tableMapping.TableName, itm.Name);
                }
            }
            return result;
        }
    }


    /// <summary>
    /// Multi-type result for two types
    /// </summary>
    public class CompositeResult<TData1, TData2> : CompositeResult
    {

        public TData1 Object1 { get { return (TData1)this.Values[0]; } }
        public TData2 Object2 { get { return (TData2)this.Values[1]; } }

        public override void ParseValues(IDataReader rdr, IDbProvider provider)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr, provider), this.Parse<TData2>(rdr, provider) };
        }
    }

    /// <summary>
    /// Multi-type result for three types
    /// </summary>
    public class CompositeResult<TData1, TData2, TData3> : CompositeResult<TData1, TData2>
    {
        public TData3 Object3 { get { return (TData3)this.Values[2]; } }

        public override void ParseValues(IDataReader rdr, IDbProvider provider)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr, provider), this.Parse<TData2>(rdr, provider), this.Parse<TData3>(rdr, provider) };
        }
    }

    /// <summary>
    /// Multi-type result for four types
    /// </summary>
    public class CompositeResult<TData1, TData2, TData3, TData4> : CompositeResult<TData1, TData2, TData3>
    {
        public TData4 Object4 { get { return (TData4)this.Values[3]; } }

        public override void ParseValues(IDataReader rdr, IDbProvider provider)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr, provider), this.Parse<TData2>(rdr, provider), this.Parse<TData3>(rdr, provider), this.Parse<TData4>(rdr, provider) };
        }
    }

    /// <summary>
    /// Data context functions for the execution of query data
    /// </summary>
    public partial class DataContext
    {

        // Base types
        private static readonly HashSet<Type> BaseTypes = new HashSet<Type>()
        {
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(int?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(String),
            typeof(Guid),
            typeof(Guid?),
            typeof(Type),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(UInt32),
            typeof(UInt32?),
            typeof(byte[])
        };

        /// <summary>
        /// Execute a stored procedure transposing the result set back to <typeparamref name="TModel"/>
        /// </summary>
        public IEnumerable<TModel> Query<TModel>(String spName, params object[] arguments)
        {
#if DEBUG 
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToCollection<TModel>(rdr).ToList();
                }
                else
                    using(DataContext subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateStoredProcedureCommand(subContext, spName, arguments))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToCollection<TModel>(rdr).ToList();
#if DEBUG 
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("QUERY {0} executed in {1} ms", spName, sw.ElapsedMilliseconds);
            }
#endif
        }

        

        /// <summary>
        /// Reader to collection of objects
        /// </summary>
        private IEnumerable<TModel> ReaderToCollection<TModel>(IDataReader rdr)
        {
            while (rdr.Read())
                yield return this.MapObject<TModel>(rdr);
        }

        /// <summary>
        /// Map an object 
        /// </summary>
        private TModel MapObject<TModel>(IDataReader rdr)
        {
            if (typeof(CompositeResult).IsAssignableFrom(typeof(TModel)))
            {
                var retVal = Activator.CreateInstance(typeof(TModel));
                (retVal as CompositeResult).ParseValues(rdr, this.m_provider);
                foreach (var itm in (retVal as CompositeResult).Values.OfType<IAdoLoadedData>())
                    itm.Context = this;
                return (TModel)retVal;
            }
            else if (BaseTypes.Contains(typeof(TModel)))
                return (TModel)rdr[0];
            else
                return (TModel)this.MapObject(typeof(TModel), rdr);
        }

        /// <summary>
        /// Map an object 
        /// </summary>
        private object MapObject(Type tModel, IDataReader rdr)
        {

            var tableMapping = TableMapping.Get(tModel);
            dynamic result = Activator.CreateInstance(tModel);
            // Read each column and pull from reader
            foreach (var itm in tableMapping.Columns)
            {
                try
                {
                    object value = this.m_provider.ConvertValue(rdr[itm.Name], itm.SourceProperty.PropertyType);
                    itm.SourceProperty.SetValue(result, value);
                }
                catch(Exception e)
                {
                    this.m_tracer.TraceError("Error mapping: {0} : {1}", itm.Name, e.ToString());
                    throw new MissingFieldException(tableMapping.TableName, itm.Name);
                }
            }

            if (result is IAdoLoadedData)
                (result as IAdoLoadedData).Context = this;
            else
                this.m_tracer.TraceWarning("Type {0} does not implement IAdoLoadedData", tModel);
            return result;

        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public Object FirstOrDefault(Type returnType, SqlStatement stmt)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToResult(returnType, rdr);
                }
                else
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToResult(returnType, rdr);
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("QUERY {0} executed in {1} ms", stmt, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(String spName, params object[] arguments)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToResult<TModel>(rdr);
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateStoredProcedureCommand(subContext, spName, arguments))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToResult<TModel>(rdr);
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("FIRST {0} executed in {1} ms", spName, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = new SqlStatement<TModel>().SelectFrom().Where(querySpec).Limit(1);
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToResult<TModel>(rdr);
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToResult<TModel>(rdr);
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("FIRST {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(SqlStatement stmt)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt.Limit(1)))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToResult<TModel>(rdr);
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt.Limit(1)))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToResult<TModel>(rdr);

                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("FIRST {0} executed in {1} ms", stmt, sw.ElapsedMilliseconds);
            }
#endif
        }


        /// <summary>
        /// Returns only if only one result is available
        /// </summary>
        public TModel SingleOrDefault<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = new SqlStatement<TModel>().SelectFrom().Where(querySpec).Limit(2);

                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                        using (var rdr = dbc.ExecuteReader())
                        {
                            var retVal = this.ReaderToResult<TModel>(rdr);
                            if (!rdr.Read()) return retVal;
                            else throw new InvalidOperationException("Sequence contains more than one element");
                        }
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                    using (var rdr = dbc.ExecuteReader())
                    {
                        var retVal = this.ReaderToResult<TModel>(rdr);
                        if (!rdr.Read()) return retVal;
                        else throw new InvalidOperationException("Sequence contains more than one element");
                    }
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("SINGLE {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }


        /// <summary>
        /// Returns only if only one result is available
        /// </summary>
        public bool Any<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = this.m_provider.Exists(new SqlStatement<TModel>().SelectFrom().Where(querySpec));
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                            return (bool)dbc.ExecuteScalar();
                }
                else
                {
                    using(var subContext = this.m_provider.CloneConnection(this))
                        using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                            return (bool)dbc.ExecuteScalar();
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("ANY {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Represents the count function
        /// </summary>
        internal bool Any(SqlStatement querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = this.m_provider.Exists(querySpec);
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                            return (bool)dbc.ExecuteScalar();
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                        return (bool)dbc.ExecuteScalar();
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("ANY {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Returns only if only one result is available
        /// </summary>
        public int Count<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = this.m_provider.Count(new SqlStatement<TModel>().SelectFrom().Where(querySpec));
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                            return (int)dbc.ExecuteScalar();
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                        return (int)dbc.ExecuteScalar();
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("COUNT {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Represents the count function
        /// </summary>
        public int Count(SqlStatement querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var stmt = this.m_provider.Count(querySpec);
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, stmt))
                            return Convert.ToInt32(dbc.ExecuteScalar());
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, stmt))
                        return Convert.ToInt32(dbc.ExecuteScalar());
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("COUNT {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Parse to a single result
        /// </summary>
        private TModel ReaderToResult<TModel>(IDataReader rdr)
        {
            if (rdr.Read()) return this.MapObject<TModel>(rdr);
            else return default(TModel);
        }

        /// <summary>
        /// Parse to a single result
        /// </summary>
        private object ReaderToResult(Type returnType, IDataReader rdr)
        {
            if (rdr.Read()) return this.MapObject(returnType, rdr);
            else return null;
        }

        /// <summary>
        /// Execute the specified query
        /// </summary>
        public IEnumerable<TModel> Query<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var query = new SqlStatement<TModel>().SelectFrom().Where(querySpec);
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, query))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToCollection<TModel>(rdr).ToList();
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(subContext, query))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToCollection<TModel>(rdr).ToList();
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("QUERY {0} executed in {1} ms", querySpec, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Adds data in a safe way
        /// </summary>
        public void AddData(string key, object value)
        {
            lock (this.m_dataDictionary)
                if (!this.m_dataDictionary.ContainsKey(key))
                    this.m_dataDictionary.Add(key, value);
        }

        /// <summary>
        /// Query using the specified statement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<TModel> Query<TModel>(SqlStatement query)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                if (this.Transaction != null)
                {
                    lock (this.m_lockObject)
                        using (var dbc = this.m_provider.CreateCommand(this, query))
                        using (var rdr = dbc.ExecuteReader())
                            return this.ReaderToCollection<TModel>(rdr).ToList();
                }
                else
                {
                    using (var subContext = this.m_provider.CloneConnection(this))
                    using (var dbc = this.m_provider.CreateCommand(this, query))
                    using (var rdr = dbc.ExecuteReader())
                        return this.ReaderToCollection<TModel>(rdr).ToList();
                }
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("QUERY {0} executed in {1} ms", query.SQL, sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public TModel Insert<TModel>(TModel value)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                // First we want to map object to columns
                var tableMap = TableMapping.Get(typeof(TModel));

                SqlStatement columnNames = new SqlStatement(),
                    values = new SqlStatement();
                foreach (var col in tableMap.Columns)
                {
                    var val = col.SourceProperty.GetValue(value);
                    if (val == null ||
                        !col.IsNonNull && (
                        val.Equals(default(Guid)) ||
                        val.Equals(default(DateTime)) ||
                        val.Equals(default(DateTimeOffset)) ||
                        val.Equals(default(Decimal))))
                        val = null;

                    if (col.IsAutoGenerated && val == null) continue;

                    columnNames.Append($"{col.Name}");


                    // Append value
                    values.Append("?", val);

                    values.Append(",");
                    columnNames.Append(",");
                }
                values.RemoveLast();
                columnNames.RemoveLast();

                var returnKeys = tableMap.Columns.Where(o => o.IsAutoGenerated);

                // Return arrays
                var stmt = this.m_provider.Returning(
                    new SqlStatement($"INSERT INTO {tableMap.TableName} (").Append(columnNames).Append(") VALUES (").Append(values).Append(")"),
                    returnKeys.ToArray()
                );

                // Execute
                lock(this.m_lockObject)
                using (var dbc = this.m_provider.CreateCommand(this, stmt))
                    if (returnKeys.Count() > 0 && this.m_provider.Features.HasFlag(SqlEngineFeatures.ReturnedInserts))
                    {
                        using (var rdr = dbc.ExecuteReader())
                            if (rdr.Read())
                                foreach (var itm in returnKeys)
                                {
                                    object ov = null;
                                    if (MapUtil.TryConvert(rdr[itm.Name], itm.SourceProperty.PropertyType, out ov))
                                        itm.SourceProperty.SetValue(value, ov);
                                }
                    }
                    else
                        dbc.ExecuteNonQuery();

                if (value is IAdoLoadedData)
                    (value as IAdoLoadedData).Context = this;

                return value;
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("INSERT executed in {0} ms", sw.ElapsedMilliseconds);
            }
#endif

        }

        /// <summary>
        /// Delete from the database
        /// </summary>
        public void Delete<TModel>(Expression<Func<TModel, bool>> where)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var query = new SqlStatement<TModel>().DeleteFrom().Where(where);
                lock(this.m_lockObject)
                using (var dbc = this.m_provider.CreateCommand(this, query))
                    dbc.ExecuteNonQuery();
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("DELETE executed in {0} ms", sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Delete from the database
        /// </summary>
        public void Delete<TModel>(TModel obj)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                var tableMap = TableMapping.Get(typeof(TModel));
                SqlStatement whereClause = new SqlStatement();
                foreach (var itm in tableMap.Columns)
                {
                    var itmValue = itm.SourceProperty.GetValue(obj);
                    if (itm.IsPrimaryKey)
                        whereClause.And($"{itm.Name} = ?", itmValue);
                }

                var query = new SqlStatement<TModel>().DeleteFrom().Where(whereClause);
                lock(this.m_lockObject)
                using (var dbc = this.m_provider.CreateCommand(this, query))
                    dbc.ExecuteNonQuery();
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("DELETE executed in {0} ms", sw.ElapsedMilliseconds);
            }
#endif
        }


        /// <summary>
        /// Updates the specified object
        /// </summary>
        public TModel Update<TModel>(TModel value)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                // Build the command
                var tableMap = TableMapping.Get(typeof(TModel));
                SqlStatement<TModel> query = new SqlStatement<TModel>().UpdateSet();
                SqlStatement whereClause = new SqlStatement();
                foreach (var itm in tableMap.Columns)
                {
                    var itmValue = itm.SourceProperty.GetValue(value);

                    if (itmValue == null ||
                        itmValue.Equals(default(Guid)) ||
                        itmValue.Equals(default(DateTime)) ||
                        itmValue.Equals(default(DateTimeOffset)) ||
                        itmValue.Equals(default(Decimal)))
                        itmValue = null;

                    query.Append($"{itm.Name} = ? ", itmValue);
                    query.Append(",");
                    if (itm.IsPrimaryKey)
                        whereClause.And($"{itm.Name} = ?", itmValue);
                }
                query.RemoveLast();
                query.Where(whereClause);

                // Now update
                lock(this.m_lockObject)
                using (var dbc = this.m_provider.CreateCommand(this, query))
                    dbc.ExecuteNonQuery();

                if (value is IAdoLoadedData)
                    (value as IAdoLoadedData).Context = this;

                return value;
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("UPDATE executed in {0} ms", sw.ElapsedMilliseconds);
            }
#endif
        }

        /// <summary>
        /// Execute a non query
        /// </summary>
        public void ExecuteNonQuery(SqlStatement stmt)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#endif
                lock(this.m_lockObject)
                using (var dbc = this.m_provider.CreateCommand(this, stmt))
                    dbc.ExecuteNonQuery();
#if DEBUG
            }
            finally
            {
                sw.Stop();
                this.m_tracer.TraceVerbose("EXECUTE NON QUERY executed in {0} ms", sw.ElapsedMilliseconds);
            }
#endif
        }
    }
}
