using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Query builder hack
    /// </summary>
    public interface IQueryBuilderHack
    {

        /// <summary>
        /// Hacks the query in some manner
        /// </summary>
        /// <param name="sqlStatement">The current vanilla (no WHERE clause) query</param>
        /// <param name="whereClause">The current where clause</param>
        /// <param name="property">The property which is currently being hacked</param>
        /// <param name="predicate">The current predicate</param>
        /// <param name="scopedTables">The tables that are scoped for the current query</param>
        /// <returns></returns>
        bool HackQuery(QueryBuilder builder, SqlStatement sqlStatement, SqlStatement whereClause, PropertyInfo property, String queryPrefix, QueryPredicate predicate, Object values, IEnumerable<TableMapping> scopedTables);

    }
}
