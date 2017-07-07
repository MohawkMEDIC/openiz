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
using OpenIZ.OrmLite.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.OrmLite
{
    /// <summary>
    /// Table information tool
    /// </summary>
    public class TableMapping
    {
        // Hashmap
        private Dictionary<String, ColumnMapping> m_mappings = new Dictionary<string, ColumnMapping>();

        // Tabl mappings 
        private static Dictionary<Type, TableMapping> m_tableMappings = new Dictionary<Type, TableMapping>();

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
            this.Columns = t.GetProperties().Where(o => o.GetCustomAttribute<ColumnAttribute>() != null).Select(o => ColumnMapping.Get(o, this)).ToList();
            foreach (var itm in this.Columns)
                this.m_mappings.Add(itm.SourceProperty.Name, itm);

        }

        /// <summary>
        /// Get table information
        /// </summary>
        public static TableMapping Get(Type t)
        {
            TableMapping retVal = null;
            if (!m_tableMappings.TryGetValue(t, out retVal))
                lock (m_tableMappings)
                {
                    retVal = new TableMapping(t);
                    if (!m_tableMappings.ContainsKey(t))
                        m_tableMappings.Add(t, retVal);
                }
            return retVal;
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


        /// <summary>
        /// Gets the association table mapping
        /// </summary>
        public TableMapping AssociationWith(TableMapping subTableMap)
        {
            var att = this.OrmType.GetCustomAttributes<AssociativeTableAttribute>().FirstOrDefault(o => o.TargetTable == subTableMap.OrmType);
            if (att == null) return null;
            else
                return TableMapping.Get(att.AssociationTable);
        }
    }
}
