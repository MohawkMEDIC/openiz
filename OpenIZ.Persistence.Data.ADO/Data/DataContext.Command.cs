using OpenIZ.Core.Model.Map;
using OpenIZ.Persistence.Data.ADO.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data
{
    /// <summary>
    /// Multi type result used when a result set is a join
    /// </summary>
    public abstract class MultiTypeResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public Object[] Values { get; protected set; }

        // Parse values
        public abstract void ParseValues(IDataReader rdr);

        /// <summary>
        /// Parse the data
        /// </summary>
        protected TData Parse<TData>(IDataReader rdr)
        {
            var tableMapping = TableMapping.Get(typeof(TData));
            dynamic result = Activator.CreateInstance(typeof(TData));
            // Read each column and pull from reader
            foreach (var itm in tableMapping.Columns)
            {
                object value = null;
                if (MapUtil.TryConvert(rdr[itm.Name], itm.SourceProperty.PropertyType, out value))
                    itm.SourceProperty.SetValue(result, value);
            }
            return result;
        }
    }


    /// <summary>
    /// Multi-type result for two types
    /// </summary>
    public class MultiTypeResult<TData1, TData2> : MultiTypeResult
    {

        public TData1 Object1 { get { return (TData1)this.Values[0]; } }
        public TData2 Object2 { get { return (TData2)this.Values[1]; } }

        public override void ParseValues(IDataReader rdr)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr), this.Parse<TData2>(rdr) };
        }
    }

    /// <summary>
    /// Multi-type result for three types
    /// </summary>
    public class MultiTypeResult<TData1, TData2, TData3> : MultiTypeResult<TData1, TData2>
    {
        public TData3 Object3 { get { return (TData3)this.Values[2]; } }

        public override void ParseValues(IDataReader rdr)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr), this.Parse<TData2>(rdr), this.Parse<TData3>(rdr) };
        }
    }

    /// <summary>
    /// Multi-type result for four types
    /// </summary>
    public class MultiTypeResult<TData1, TData2, TData3, TData4> : MultiTypeResult<TData1, TData2, TData3>
    {
        public TData4 Object4 { get { return (TData4)this.Values[3]; } }

        public override void ParseValues(IDataReader rdr)
        {
            this.Values = new object[] { this.Parse<TData1>(rdr), this.Parse<TData2>(rdr), this.Parse<TData3>(rdr), this.Parse<TData4>(rdr) };
        }
    }

    /// <summary>
    /// Data context functions for the execution of query data
    /// </summary>
    public partial class DataContext
    {

        /// <summary>
        /// Execute a stored procedure transposing the result set back to <typeparamref name="TModel"/>
        /// </summary>
        public IEnumerable<TModel> Query<TModel>(String spName, params object[] arguments)
        {
            using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToCollection<TModel>(rdr).ToList();
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
            if (typeof(MultiTypeResult).IsAssignableFrom(typeof(TModel)))
            {
                var retVal = Activator.CreateInstance(typeof(TModel));
                (retVal as MultiTypeResult).ParseValues(rdr);
                return (TModel)retVal;
            }
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
                object value = null;
                if (MapUtil.TryConvert(rdr[itm.Name], itm.SourceProperty.PropertyType, out value))
                    itm.SourceProperty.SetValue(result, value);
            }
            return result;

        }
        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(String spName, params object[] arguments)
        {
            using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToResult<TModel>(rdr);
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
            var stmt = this.m_provider.Take(new SqlStatement<TModel>().SelectFrom().Where(querySpec), 1);
            using (var dbc = this.m_provider.CreateCommand(this, stmt))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToResult<TModel>(rdr);
        }

        /// <summary>
        /// Returns only if only one result is available
        /// </summary>
        public TModel SingleOrDefault<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
            var stmt = this.m_provider.Take(new SqlStatement<TModel>().SelectFrom().Where(querySpec), 2);
            using (var dbc = this.m_provider.CreateCommand(this, stmt))
            using (var rdr = dbc.ExecuteReader())
            {
                var retVal = this.ReaderToResult<TModel>(rdr);
                if (!rdr.Read()) return retVal;
                else return default(TModel);
            }
        }


        /// <summary>
        /// Returns only if only one result is available
        /// </summary>
        public bool Any<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
            var stmt = this.m_provider.Exists(new SqlStatement<TModel>().SelectFrom().Where(querySpec));
            using (var dbc = this.m_provider.CreateCommand(this, stmt))
                return (bool)dbc.ExecuteScalar();
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
        /// Execute the specified query
        /// </summary>
        public IEnumerable<TModel> Query<TModel>(Expression<Func<TModel, bool>> querySpec)
        {
            var query = new SqlStatement<TModel>().SelectFrom().Where(querySpec);
            using (var dbc = this.m_provider.CreateCommand(this, query))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToCollection<TModel>(rdr).ToList();
        }

        /// <summary>
        /// Query using the specified statement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<TModel> Query<TModel>(SqlStatement query)
        {
            using (var dbc = this.m_provider.CreateCommand(this, query))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToCollection<TModel>(rdr).ToList();
        }

        /// <summary>
        /// Insert the specified object
        /// </summary>
        public TModel Insert<TModel>(TModel value)
        {
            // First we want to map object to columns
            var tableMap = TableMapping.Get(typeof(TModel));

            SqlStatement columnNames = new SqlStatement(),
                values = new SqlStatement();
            foreach (var col in tableMap.Columns)
            {
                columnNames.Append($"{col.Name}");

                var val = col.SourceProperty.GetValue(value);
                if (val == Activator.CreateInstance(col.SourceProperty.PropertyType))
                    val = null;


                // Append value
                values.Append("?", val);

                if (col != tableMap.Columns.Last())
                {
                    values.Append(",");
                    columnNames.Append(",");
                }
            }

            var returnKeys = tableMap.Columns.Where(o => o.IsAutoGenerated);

            // Return arrays
            var stmt = this.m_provider.Returning(
                new SqlStatement($"INSERT INTO {tableMap.TableName} (").Append(columnNames).Append(") VALUES (").Append(values).Append(")"),
                returnKeys.ToArray()
            );

            // Execute
            using (var dbc = this.m_provider.CreateCommand(this, stmt))
                if (returnKeys.Count() > 0)
                {
                    using (var rdr = dbc.ExecuteReader())
                        if (rdr.Read())
                            foreach (var itm in returnKeys)
                                itm.SourceProperty.SetValue(value, rdr[itm.Name]);
                }
                else
                    dbc.ExecuteNonQuery();

            return value;

        }

        /// <summary>
        /// Delete from the database
        /// </summary>
        public void Delete<TModel>(Expression<Func<TModel, bool>> where)
        {
            var query = new SqlStatement<TModel>().DeleteFrom().Where(where);
            using (var dbc = this.m_provider.CreateCommand(this, query))
                dbc.ExecuteNonQuery();
        }
    }
}
