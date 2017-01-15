using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Util
{
    /// <summary>
    /// Table information tool
    /// </summary>
    public class TableMapping
    {
        // Hashmap
        private Dictionary<String, ColumnMapping> m_mappings = new Dictionary<string, ColumnMapping>();

        /// <summary>
        /// ORM type model
        /// </summary>
        public Type OrmType { get; private set; }

        /// <summary>
        /// Table name
        /// </summary>
        public String TableName { get; private set; }

        /// <summary>
        /// Column mappings
        /// </summary>
        public IEnumerable<ColumnMapping> Columns { get; private set; }

        /// <summary>
        /// Private ctor for table mapping
        /// </summary>
        private TableMapping(Type t)
        {

            this.OrmType = t;
            this.TableName = t.GetCustomAttribute<TableAttribute>()?.Name ?? t.Name;
            this.Columns = t.GetProperties().Where(o => o.GetCustomAttributes<ColumnAttribute>() != null).Select(o => ColumnMapping.Get(o, this));
            foreach (var itm in this.Columns)
                this.m_mappings.Add(itm.SourceProperty.Name, itm);

        }

        /// <summary>
        /// Get table information
        /// </summary>
        public static TableMapping Get(Type t)
        {
            return new TableMapping(t);
        }

        /// <summary>
        /// Get column mapping
        /// </summary>
        public ColumnMapping GetColumn(PropertyInfo pi)
        {
            ColumnMapping map = null;
            this.m_mappings.TryGetValue(pi.Name, out map);
            return map;
        }

        /// <summary>
        /// Get column mapping
        /// </summary>
        public ColumnMapping GetColumn(MemberInfo mi)
        {
            ColumnMapping map = null;
            this.m_mappings.TryGetValue(mi.Name, out map);
            return map;
        }

        /// <summary>
        /// Get the column mapping for the named property
        /// </summary>
        public ColumnMapping GetColumn(string propertyName)
        {
            ColumnMapping map = null;
            this.m_mappings.TryGetValue(propertyName, out map);
            return map;
        }
    }
}
