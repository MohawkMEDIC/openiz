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
    /// Data context functions for the execution of query data
    /// </summary>
    public partial class DataContext
    {

        /// <summary>
        /// Execute a stored procedure transposing the result set back to <typeparamref name="TModel"/>
        /// </summary>
        public IEnumerable<TModel> Query<TModel>(String spName, params object[] arguments) where TModel : new()
        {
            using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToCollection<TModel>(rdr);
        }
        
        /// <summary>
        /// Reader to collection of objects
        /// </summary>
        private IEnumerable<TModel> ReaderToCollection<TModel>(IDataReader rdr) where TModel : new()
        {
            while(rdr.Read())
            {
                yield return this.MapObject<TModel>(rdr);
            }
        }

        /// <summary>
        /// Map an object 
        /// </summary>
        private TModel MapObject<TModel>(IDataReader rdr) where TModel: new()
        {
            var tableMapping = TableMapping.Get(typeof(TModel));
            dynamic result = new TModel();
            // Read each column and pull from reader
            foreach(var itm in tableMapping.Columns)
                itm.SourceProperty.SetValue(result, rdr[itm.Name]);
            return result;
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(String spName, params object[] arguments) where TModel : new()
        {
            using (var dbc = this.m_provider.CreateStoredProcedureCommand(this, spName, arguments))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToResult<TModel>(rdr);
        }

        /// <summary>
        /// First or default returns only the first object or null if not found
        /// </summary>
        public TModel FirstOrDefault<TModel>(Expression<Func<TModel, bool>> querySpec) where TModel : new()
        {
            var stmt = new SqlStatement<TModel>().SelectFrom().Where(querySpec);
            using (var dbc = this.m_provider.CreateCommand(this, stmt))
            using (var rdr = dbc.ExecuteReader())
                return this.ReaderToResult<TModel>(rdr);
        }

        /// <summary>
        /// Parse to a single result
        /// </summary>
        private TModel ReaderToResult<TModel>(IDataReader rdr) where TModel : new()
        {
            if (rdr.Read()) return this.MapObject<TModel>(rdr);
            else return default(TModel);
        }
    }
}
