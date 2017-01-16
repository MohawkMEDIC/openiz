using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Query;
using OpenIZ.Persistence.Data.ADO.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using System.Collections;
using System.Text.RegularExpressions;
using OpenIZ.Persistence.Data.ADO.Data.Model;

namespace OpenIZ.Persistence.Data.ADO.Util
{

    /// <summary>
    /// Query builder for model objects
    /// </summary>
    /// <remarks>
    /// Because the ORM used in the ADO persistence layer is very very lightweight, this query builder exists to parse 
    /// LINQ or HTTP query parameters into complex queries which implement joins/CTE/etc. across tables. Stuff that the
    /// classes in the little data model can't possibly support via LINQ expression.
    /// 
    /// To use this, simply pass a model based LINQ expression to the CreateQuery method. Examples are in the test project. 
    /// 
    /// Some reasons to use this:
    ///     - The generated SQL will gather all table instances up the object hierarchy for you (one hit instead of multiple)
    ///     - The queries it writes use efficient CTE tables
    ///     - It can do intelligent join conditions
    ///     - It uses Model LINQ expressions directly to SQL without the need to translate from Model LINQ to Domain LINQ queries
    /// </remarks>
    /// <example lang="cs" name="LINQ Expression illustrating join across tables">
    /// <![CDATA[QueryBuilder.CreateQuery<Patient>(o => o.DeterminerConcept.Mnemonic == "Instance")]]>
    /// </example>
    /// <example lang="sql" name="Resulting SQL query">
    /// <![CDATA[
    /// WITH 
    ///     cte0 AS (
    ///         SELECT cd_tbl.cd_id 
    ///         FROM cd_vrsn_tbl AS cd_vrsn_tbl 
    ///             INNER JOIN cd_tbl AS cd_tbl ON (cd_tbl.cd_id = cd_vrsn_tbl.cd_id) 
    ///         WHERE (cd_vrsn_tbl.mnemonic = ? )
    ///     )
    /// SELECT * 
    /// FROM pat_tbl AS pat_tbl 
    ///     INNER JOIN psn_tbl AS psn_tbl ON (pat_tbl.ent_vrsn_id = psn_tbl.ent_vrsn_id) 
    ///     INNER JOIN ent_vrsn_tbl AS ent_vrsn_tbl ON (psn_tbl.ent_vrsn_id = ent_vrsn_tbl.ent_vrsn_id) 
    ///     INNER JOIN ent_tbl AS ent_tbl ON (ent_tbl.ent_id = ent_vrsn_tbl.ent_id) 
    ///     INNER JOIN cte0 ON (ent_tbl.dtr_cd_id = cte0.cd_id)
    /// ]]>
    /// </example>
    public static class QueryBuilder
    {
        // Regex to extract property, guards and cast
        private const string m_extractionRegex = @"^(\w*?)(\[(\w*)\])?(\@(\w*))?(\.(.*))?$";

        // Mapper
        private static ModelMapper m_mapper = new ModelMapper(typeof(QueryBuilder).Assembly.GetManifestResourceStream(AdoDataConstants.MapResourceName));
        private const int PropertyRegexGroup = 1;
        private const int GuardRegexGroup = 3;
        private const int CastRegexGroup = 5;
        private const int SubPropertyRegexGroup = 7;

        /// <summary>
        /// Create a query 
        /// </summary>
        public static SqlStatement CreateQuery<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            var nvc = QueryExpressionBuilder.BuildQuery(predicate, true);
            return CreateQuery<TModel>(nvc);
        }

        /// <summary>
        /// Create query
        /// </summary>
        public static SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, params ColumnMapping[] selector)
        {
            return CreateQuery<TModel>(query, null, selector);
        }

        /// <summary>
        /// Query query 
        /// </summary>
        /// <param name="query"></param>
        public static SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, String tablePrefix, params ColumnMapping[] selector)
        {
            var tableType = m_mapper.MapModelType(typeof(TModel));
            var tableMap = TableMapping.Get(tableType);
            List<TableMapping> scopedTables = new List<TableMapping>() { tableMap };

            bool skipParentJoin = true;
            SqlStatement selectStatement = null;
            if (selector == null || selector.Length == 0)
            {
                skipParentJoin = false;
                selectStatement = new SqlStatement($"SELECT * FROM {tableMap.TableName} AS {tablePrefix}{tableMap.TableName} ");
            }
            else
            {
                var columnList = String.Join(",", selector.Select(o => {
                    var rootCol = tableMap.GetColumn(o.SourceProperty);
                    skipParentJoin &= rootCol != null;
                    if (skipParentJoin)
                        return $"{tablePrefix}{rootCol.Table.TableName}.{rootCol.Name}";
                    else
                        return $"{tablePrefix}{o.Table.TableName}.{o.Name}";
                }));
                selectStatement = new SqlStatement($"SELECT {columnList} FROM {tableMap.TableName} AS {tablePrefix}{tableMap.TableName} ");
            }

            // Is this a sub-type, if so we want to join
            if (typeof(DbSubTable).IsAssignableFrom(tableMap.OrmType) && !skipParentJoin)
            {
                var joinTableMap = tableMap;
                var parentKeyProp = tableMap.OrmType.GetProperty("ParentKey");
                while (parentKeyProp != null)
                {
                    var parentFk = joinTableMap.GetColumn(parentKeyProp);
                    var fkAtt = parentFk.ForeignKey;
                    var subMap = TableMapping.Get(fkAtt.Table);
                    scopedTables.Add(subMap);
                    var subCol = subMap.GetColumn(fkAtt.Column);
                    selectStatement.Append($"INNER JOIN {subMap.TableName} AS {tablePrefix}{subMap.TableName} ON ({tablePrefix}{joinTableMap.TableName}.{parentFk.Name} = {tablePrefix}{subMap.TableName}.{subCol.Name}) ");
                    parentKeyProp = fkAtt.Table.GetProperty("ParentKey");
                    joinTableMap = TableMapping.Get(fkAtt.Table);
                }
                //while(t.)
                // Join non versioned root key?
                selectStatement.Append(CreateBaseTableJoin(joinTableMap, tablePrefix, scopedTables));
            }
            else if(!skipParentJoin)
                selectStatement.Append(CreateBaseTableJoin(tableMap, tablePrefix, scopedTables));

            // We want to process each query and build WHERE clauses - these where clauses are based off of the JSON / XML names
            // on the model, so we have to use those for the time being before translating to SQL
            Regex re = new Regex(m_extractionRegex);
            List<KeyValuePair<String, Object>> workingParameters = new List<KeyValuePair<string, object>>(query);

            // Where clause
            SqlStatement whereClause = new SqlStatement();
            List<SqlStatement> cteStatements = new List<SqlStatement>();
            
            // Construct
            while (workingParameters.Count > 0)
            {
                var parm = workingParameters.First();
                workingParameters.RemoveAt(0);

                // Match the regex and process
                var matches = re.Match(parm.Key);
                if (!matches.Success) throw new ArgumentOutOfRangeException(parm.Key);

                // First we want to collect all the working parameters 
                string propertyPath = matches.Groups[PropertyRegexGroup].Value,
                    castAs = matches.Groups[CastRegexGroup].Value,
                    guard = matches.Groups[GuardRegexGroup].Value,
                    subProperty = matches.Groups[SubPropertyRegexGroup].Value;

                // Next, we want to construct the 
                var otherParms = workingParameters.Where(o => re.Match(o.Key).Groups[PropertyRegexGroup].Value == propertyPath).ToArray();

                // Remove the working parameters if the column is FK then all parameters
                if (otherParms.Any() || !String.IsNullOrEmpty(guard) || !String.IsNullOrEmpty(subProperty))
                {
                    foreach (var o in otherParms)
                        workingParameters.Remove(o);

                    // We need to do a sub query
                    IEnumerable<KeyValuePair<String, Object>> subQuery = new List<KeyValuePair<String, Object>>() { new KeyValuePair<string, object>(subProperty, parm.Value) };
                    subQuery = otherParms.Select(o => new KeyValuePair<String, Object>(re.Match(o.Key).Groups[PropertyRegexGroup].Value, o.Value)).Union(subQuery);

                    // Grab the appropriate builder
                    var subProp = typeof(TModel).GetXmlProperty(propertyPath, true);
                    if (subProp == null) throw new MissingMemberException(propertyPath);

                    // Link to this table in the other?
                    // Is this a collection?
                    if (typeof(IEnumerable).IsAssignableFrom(subProp.PropertyType)) // Other table points at this on
                    {
                        var propertyType = subProp.PropertyType.StripGeneric();
                        // map and get ORM def'n
                        var subTableType = m_mapper.MapModelType(propertyType);
                        var subTableMap = TableMapping.Get(subTableType);
                        var linkColumn = subTableMap.Columns.SingleOrDefault(o => o.ForeignKey.Table == tableMap.OrmType);
                        if (linkColumn == null) throw new InvalidOperationException($"Cannot find foreign key reference to table {tableMap.TableName} in {subTableMap.TableName}");

                        // Generate method
                        var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { propertyType }, new Type[] { subQuery.GetType(), typeof(String), typeof(ColumnMapping[]) });
                        SqlStatement subQueryStatement = genMethod.Invoke(null, new Object[] { subQuery, IncrementSubQueryAlias(tablePrefix), new ColumnMapping[] { linkColumn } }) as SqlStatement;

                        // TODO: Check if limiting the the query is better
                        var localTable = scopedTables.Where(o => o.GetColumn(linkColumn.ForeignKey.Column) != null).FirstOrDefault();
                        whereClause.And($"{localTable.TableName}.{localTable.GetColumn(linkColumn.ForeignKey.Column)} IN (").Append(subQueryStatement).Append(")");

                    }
                    else // this table points at other
                    {
                        TableMapping tableMapping = null;
                        var subPropKey = typeof(TModel).GetXmlProperty(propertyPath);

                        // Get column info
                        PropertyInfo domainProperty = scopedTables.Select(o => { tableMapping = o; return m_mapper.MapModelProperty(typeof(TModel), o.OrmType, subPropKey); }).FirstOrDefault(o => o != null);
                        var linkColumn = tableMapping.GetColumn(domainProperty);
                        var fkTableDef = TableMapping.Get(linkColumn.ForeignKey.Table);
                        var fkColumnDef = fkTableDef.GetColumn(linkColumn.ForeignKey.Column);

                        // Create the sub-query
                        var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { subProp.PropertyType }, new Type[] { subQuery.GetType(), typeof(ColumnMapping[]) });
                        SqlStatement subQueryStatement = genMethod.Invoke(null, new Object[] { subQuery, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                        cteStatements.Add(new SqlStatement($"cte{cteStatements.Count} AS (").Append(subQueryStatement).Append(")"));
                        //subQueryStatement.And($"{tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = {sqName}{fkTableDef.TableName}.{fkColumnDef.Name} ");

                        selectStatement.Append($"INNER JOIN cte{cteStatements.Count - 1} ON ({tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = cte{cteStatements.Count - 1}.{fkColumnDef.Name})");

                    }

                }
                else
                    whereClause.Append(CreateWhereCondition<TModel>(propertyPath, parm.Value, tablePrefix, scopedTables));

            }

            // Return statement
            SqlStatement retVal = new SqlStatement();
            if (cteStatements.Count > 0)
            {
                retVal.Append("WITH ");
                foreach (var c in cteStatements)
                {
                    retVal.Append(c);
                    if (c != cteStatements.Last())
                        retVal.Append(",");
                }
            }
            retVal.Append(selectStatement.Where(whereClause));
            return retVal;
        }

        /// <summary>
        /// Increment sub-query alias
        /// </summary>
        private static String IncrementSubQueryAlias(string tablePrefix)
        {
            if (String.IsNullOrEmpty(tablePrefix))
                return "sq0";
            else
            {
                int sq = 0;
                if (Int32.TryParse(tablePrefix.Substring(2), out sq))
                    return "sq" + (sq + 1);
                else
                    return "sq0";
            }
        }

        /// <summary>
        /// Create a table join if needed
        /// </summary>
        private static SqlStatement CreateBaseTableJoin(TableMapping tableMap, String tablePrefix, List<TableMapping> scopedTables)
        {
            if (typeof(DbVersionedData).IsAssignableFrom(tableMap.OrmType))
            {
                var kfa = tableMap.GetColumn("Key");
                var nvtMap = TableMapping.Get(kfa.ForeignKey.Table);
                var fkMap = nvtMap.GetColumn(kfa.ForeignKey.Column);
                scopedTables.Add(nvtMap);
                return new SqlStatement($"INNER JOIN {nvtMap.TableName} AS {tablePrefix}{nvtMap.TableName} ON ({tablePrefix}{nvtMap.TableName}.{fkMap.Name} = {tablePrefix}{tableMap.TableName}.{kfa.Name}) ");
            }
            return null;
        }

        /// <summary>
        /// Create a single where condition based on the property info
        /// </summary>
        public static SqlStatement CreateWhereCondition<TModel>(String propertyPath, Object value, String tablePrefix, List<TableMapping> scopedTables)
        {

            SqlStatement retVal = new SqlStatement();

            // Map the type
            var tableMapping = scopedTables.First();
            var propertyInfo = typeof(TModel).GetXmlProperty(propertyPath);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(propertyPath);
            PropertyInfo domainProperty = scopedTables.Select(o=> { tableMapping = o; return m_mapper.MapModelProperty(typeof(TModel), o.OrmType, propertyInfo); }).FirstOrDefault(o=>o != null);

            // Now map the property path
            var tableAlias = $"{tablePrefix}{tableMapping.TableName}";
            var columnData = tableMapping.GetColumn(propertyInfo);

            // List of parameters
            var lValue = value as IList;
            if (lValue == null)
                lValue = new List<Object>() { value };

            retVal.Append("(");
            foreach (var itm in lValue)
            {
                retVal.Append($"{tableAlias}.{columnData.Name}");
                var iValue = itm;
                if (iValue is String)
                {
                    var sValue = itm as String;
                    switch (sValue[0])
                    {
                        case '<':
                            if (sValue[1] == '=')
                                retVal.Append(" <= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" < ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '>':
                            if (sValue[1] == '=')
                                retVal.Append(" >= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" > ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '!':
                            if (sValue.Equals("!null"))
                                retVal.Append(" IS NOT NULL");
                            else
                                retVal.Append(" <> ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '~':
                            retVal.Append(" LIKE '%' || ? || '%'", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        default:
                            if (sValue.Equals("null"))
                                retVal.Append(" IS NULL");
                            else
                                retVal.Append(" = ? ", CreateParameterValue(sValue, propertyInfo.PropertyType));
                            break;
                    }
                }
                else
                    retVal.Append(" = ? ", CreateParameterValue(iValue, propertyInfo.PropertyType));

                if (lValue.IndexOf(itm) < lValue.Count - 1)
                    retVal.Append(" AND ");
            }
            retVal.Append(")");

            return retVal;
        }

        /// <summary>
        /// Create a parameter value
        /// </summary>
        private static object CreateParameterValue(object value, Type toType)
        {
            object retVal = null;
            if (value.GetType() == toType ||
                value.GetType() == toType.StripNullable())
                return value;
            else if (MapUtil.TryConvert(value, toType, out retVal))
                return retVal;
            else
                throw new ArgumentOutOfRangeException(value.ToString());
        }
    }
}
