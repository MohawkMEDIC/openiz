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
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Query;
using OpenIZ.OrmLite.Attributes;
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
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using OpenIZ.OrmLite.Providers;
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.OrmLite
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
    public class QueryBuilder
    {
        // Regex to extract property, guards and cast
        private readonly Regex m_extractionRegex = new Regex(@"^(\w*?)(\[(.*?)\])?(\@(\w*))?(\.(.*))?$");

        // Join cache
        private Dictionary<String, KeyValuePair<SqlStatement, List<TableMapping>>> s_joinCache = new Dictionary<String, KeyValuePair<SqlStatement, List<TableMapping>>>();

        // Mapper
        private ModelMapper m_mapper;
        private IDbProvider m_provider;

        private const int PropertyRegexGroup = 1;
        private const int GuardRegexGroup = 3;
        private const int CastRegexGroup = 5;
        private const int SubPropertyRegexGroup = 7;

        /// <summary>
        /// Represents model mapper
        /// </summary>
        /// <param name="mapper"></param>
        public QueryBuilder(ModelMapper mapper, IDbProvider provider)
        {
            this.m_mapper = mapper;
            this.m_provider = provider;
        }
        
        /// <summary>
        /// Create a query 
        /// </summary>
        public SqlStatement CreateQuery<TModel>(Expression<Func<TModel, bool>> predicate, params ColumnMapping[] selector)
        {
            var nvc = QueryExpressionBuilder.BuildQuery(predicate, true);
            return CreateQuery<TModel>(nvc, selector);
        }

        /// <summary>
        /// Create query
        /// </summary>
        public SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, params ColumnMapping[] selector)
        {
            return CreateQuery<TModel>(query, null, selector);
        }

        /// <summary>
        /// Query query 
        /// </summary>
        /// <param name="query"></param>
        public SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query, String tablePrefix, params ColumnMapping[] selector)
        {
            var tableType = m_mapper.MapModelType(typeof(TModel));
            var tableMap = TableMapping.Get(tableType);
            List<TableMapping> scopedTables = new List<TableMapping>() { tableMap };

            bool skipParentJoin = true;
            SqlStatement selectStatement = null;
            KeyValuePair<SqlStatement, List<TableMapping>> cacheHit;
            if (!s_joinCache.TryGetValue($"{tablePrefix}.{typeof(TModel).Name}", out cacheHit))
            {
                selectStatement = new SqlStatement(this.m_provider, $" FROM {tableMap.TableName} AS {tablePrefix}{tableMap.TableName} ");

                Stack<TableMapping> fkStack = new Stack<TableMapping>();
                fkStack.Push(tableMap);
                // Always join tables?
                do
                {
                    var dt = fkStack.Pop();
                    foreach (var jt in dt.Columns.Where(o => o.IsAlwaysJoin))
                    {
                        var fkTbl = TableMapping.Get(jt.ForeignKey.Table);
                        var fkAtt = fkTbl.GetColumn(jt.ForeignKey.Column);
                        selectStatement.Append($"INNER JOIN {fkAtt.Table.TableName} AS {tablePrefix}{fkAtt.Table.TableName} ON ({tablePrefix}{jt.Table.TableName}.{jt.Name} = {tablePrefix}{fkAtt.Table.TableName}.{fkAtt.Name}) ");
                        if (!scopedTables.Contains(fkTbl))
                            fkStack.Push(fkTbl);
                        scopedTables.Add(fkAtt.Table);
                    }
                } while (fkStack.Count > 0);

                // Add the heavy work to the cache
                lock (s_joinCache)
                    if(!s_joinCache.ContainsKey($"{tablePrefix}.{typeof(TModel).Name}"))
                        s_joinCache.Add($"{tablePrefix}.{typeof(TModel).Name}", new KeyValuePair<SqlStatement, List<TableMapping>>(selectStatement.Build(), scopedTables));
            }
            else
            {
                selectStatement = cacheHit.Key.Build();
                scopedTables = cacheHit.Value;
            }

            // Column definitions
            var columnSelector = selector;
            if (selector == null || selector.Length == 0)
            {
                selectStatement = new SqlStatement(this.m_provider, $"SELECT * ").Append(selectStatement);
                // columnSelector = scopedTables.SelectMany(o => o.Columns).ToArray();
            }
            else
            {
                var columnList = String.Join(",", columnSelector.Select(o =>
                {
                    var rootCol = tableMap.GetColumn(o.SourceProperty);
                    skipParentJoin &= rootCol != null;
                    if (skipParentJoin)
                        return $"{tablePrefix}{rootCol.Table.TableName}.{rootCol.Name}";
                    else
                        return $"{tablePrefix}{o.Table.TableName}.{o.Name}";
                }));
                selectStatement = new SqlStatement(this.m_provider, $"SELECT {columnList} ").Append(selectStatement);
            }

            // We want to process each query and build WHERE clauses - these where clauses are based off of the JSON / XML names
            // on the model, so we have to use those for the time being before translating to SQL
            List<KeyValuePair<String, Object>> workingParameters = new List<KeyValuePair<string, object>>(query);

            // Where clause
            SqlStatement whereClause = new SqlStatement(this.m_provider);
            List<SqlStatement> cteStatements = new List<SqlStatement>();

            // Construct
            while (workingParameters.Count > 0)
            {
                var parm = workingParameters.First();
                workingParameters.RemoveAt(0);

                // Match the regex and process
                var matches = this.m_extractionRegex.Match(parm.Key);
                if (!matches.Success) throw new ArgumentOutOfRangeException(parm.Key);

                // First we want to collect all the working parameters 
                string propertyPath = matches.Groups[PropertyRegexGroup].Value,
                    castAs = matches.Groups[CastRegexGroup].Value,
                    guard = matches.Groups[GuardRegexGroup].Value,
                    subProperty = matches.Groups[SubPropertyRegexGroup].Value;

                // Next, we want to construct the 
                var otherParms = workingParameters.Where(o => this.m_extractionRegex.Match(o.Key).Groups[PropertyRegexGroup].Value == propertyPath).ToArray();

                // Remove the working parameters if the column is FK then all parameters
                if (otherParms.Any() || !String.IsNullOrEmpty(guard) || !String.IsNullOrEmpty(subProperty))
                {
                    foreach (var o in otherParms)
                        workingParameters.Remove(o);

                    // We need to do a sub query

                    IEnumerable<KeyValuePair<String, Object>> queryParms = new List<KeyValuePair<String, Object>>() { parm }.Union(otherParms);

                    // Grab the appropriate builder
                    var subProp = typeof(TModel).GetXmlProperty(propertyPath, true);
                    if (subProp == null) throw new MissingMemberException(propertyPath);

                    // Link to this table in the other?
                    // Is this a collection?
                    if (typeof(IList).IsAssignableFrom(subProp.PropertyType)) // Other table points at this on
                    {
                        var propertyType = subProp.PropertyType.StripGeneric();
                        // map and get ORM def'n
                        var subTableType = m_mapper.MapModelType(propertyType);
                        var subTableMap = TableMapping.Get(subTableType);
                        var linkColumns = subTableMap.Columns.Where(o => scopedTables.Any(s => s.OrmType == o.ForeignKey?.Table));
                        //var linkColumn = linkColumns.Count() > 1 ? linkColumns.FirstOrDefault(o=>o.SourceProperty.Name == "SourceKey") : linkColumns.FirstOrDefault();
                        var linkColumn = linkColumns.Count() > 1 ? linkColumns.FirstOrDefault(o => subProperty.StartsWith("source") ? o.SourceProperty.Name != "SourceKey" : o.SourceProperty.Name == "SourceKey") : linkColumns.FirstOrDefault();

                        // Link column is null, is there an assoc attrib?
                        SqlStatement subQueryStatement = new SqlStatement(this.m_provider);

                        var subTableColumn = linkColumn;
                        if (linkColumn == null)
                        {
                            var tableWithJoin = scopedTables.Select(o => o.AssociationWith(subTableMap)).FirstOrDefault(o=>o != null);
                            linkColumn = tableWithJoin.Columns.SingleOrDefault(o => scopedTables.Any(s => s.OrmType == o.ForeignKey?.Table));
                            var targetColumn = tableWithJoin.Columns.SingleOrDefault(o => o.ForeignKey.Table == subTableMap.OrmType);
                            subTableColumn = subTableMap.GetColumn(targetColumn.ForeignKey.Column);
                            // The sub-query statement needs to be joined as well 
                            var lnkPfx = IncrementSubQueryAlias(tablePrefix);
                            subQueryStatement.Append($"SELECT {lnkPfx}{tableWithJoin.TableName}.{linkColumn.Name} FROM {tableWithJoin.TableName} AS {lnkPfx}{tableWithJoin.TableName} WHERE {lnkPfx}{tableWithJoin.TableName}.{targetColumn.Name} IN (");
                            //throw new InvalidOperationException($"Cannot find foreign key reference to table {tableMap.TableName} in {subTableMap.TableName}");
                        }
                            
                        var guardConditions = queryParms.GroupBy(o => this.m_extractionRegex.Match(o.Key).Groups[GuardRegexGroup].Value);
                        foreach (var guardClause in guardConditions)
                        {
                            var subQuery = guardClause.Select(o => new KeyValuePair<String, Object>(this.m_extractionRegex.Match(o.Key).Groups[SubPropertyRegexGroup].Value, o.Value)).ToList();

                            // TODO: GUARD CONDITION HERE!!!!
                            if(!String.IsNullOrEmpty(guardClause.Key))
                            {
                                StringBuilder guardCondition = new StringBuilder();
                                var clsModel = propertyType;
                                while(clsModel.GetCustomAttribute<ClassifierAttribute>() != null)
                                {
                                    var clsProperty = clsModel.GetRuntimeProperty(clsModel.GetCustomAttribute<ClassifierAttribute>().ClassifierProperty);
                                    clsModel = clsProperty.PropertyType.StripGeneric();
                                    var redirectProperty = clsProperty.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty;
                                    if (redirectProperty != null)
                                        clsProperty = clsProperty.DeclaringType.GetRuntimeProperty(redirectProperty);

                                    guardCondition.Append(clsProperty.GetCustomAttributes<XmlElementAttribute>().First().ElementName);
                                    if (typeof(IdentifiedData).IsAssignableFrom(clsModel))
                                        guardCondition.Append(".");
                                }
                                subQuery.Add(new KeyValuePair<string, object>(guardCondition.ToString(), guardClause.Key.Split('|')));
                            }

                            // Generate method
                            var prefix = IncrementSubQueryAlias(tablePrefix);
                            var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { propertyType }, new Type[] { subQuery.GetType(), typeof(String), typeof(ColumnMapping[])});
                            subQueryStatement.Append("(");
                            subQueryStatement.Append(genMethod.Invoke(this, new Object[] { subQuery, prefix, new ColumnMapping[] { subTableColumn } }) as SqlStatement);
                            subQueryStatement.Append(")");

                            // TODO: Check if limiting the the query is better
                            if (guardConditions.Last().Key != guardClause.Key)
                                subQueryStatement.Append(" INTERSECT ");
                        }

                        if (subTableColumn != linkColumn)
                            subQueryStatement.Append(")");
                        
                        var localTable = scopedTables.Where(o => o.GetColumn(linkColumn.ForeignKey.Column) != null).FirstOrDefault();
                        whereClause.And($"{tablePrefix}{localTable.TableName}.{localTable.GetColumn(linkColumn.ForeignKey.Column).Name} IN (").Append(subQueryStatement).Append(")");

                    }
                    else // this table points at other
                    {
                        var subQuery = queryParms.Select(o => new KeyValuePair<String, Object>(this.m_extractionRegex.Match(o.Key).Groups[SubPropertyRegexGroup].Value, o.Value)).ToList();

                        if (!subQuery.Any(o => o.Key == "obsoletionTime") && typeof(IBaseEntityData).IsAssignableFrom(subProp.PropertyType))
                            subQuery.Add(new KeyValuePair<string, object>("obsoletionTime", "null"));

                        TableMapping tableMapping = null;
                        var subPropKey = typeof(TModel).GetXmlProperty(propertyPath);

                        // Get column info
                        PropertyInfo domainProperty = scopedTables.Select(o => { tableMapping = o; return m_mapper.MapModelProperty(typeof(TModel), o.OrmType, subPropKey); })?.FirstOrDefault(o => o != null);
                        ColumnMapping linkColumn = null;
                        // If the domain property is not set, we may have to infer the link
                        if (domainProperty == null)
                        {
                            var subPropType = m_mapper.MapModelType(subProp.PropertyType);
                            // We find the first column with a foreign key that points to the other !!!
                            linkColumn = scopedTables.SelectMany(o => o.Columns).FirstOrDefault(o => o.ForeignKey?.Table == subPropType);
                        }
                        else
                            linkColumn = tableMapping.GetColumn(domainProperty);

                        var fkTableDef = TableMapping.Get(linkColumn.ForeignKey.Table);
                        var fkColumnDef = fkTableDef.GetColumn(linkColumn.ForeignKey.Column);

                        // Create the sub-query
                        //var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { subProp.PropertyType }, new Type[] { subQuery.GetType(), typeof(ColumnMapping[]) });
                        //SqlStatement subQueryStatement = genMethod.Invoke(this, new Object[] { subQuery, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                        SqlStatement subQueryStatement = null;
                        if (String.IsNullOrEmpty(castAs))
                        {
                            var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { subProp.PropertyType }, new Type[] { subQuery.GetType(), typeof(ColumnMapping[]) });
                            subQueryStatement = genMethod.Invoke(this, new Object[] { subQuery, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                        }
                        else // we need to cast!
                        {
                            var castAsType = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType("OpenIZ.Core.Model", castAs);

                            var genMethod = typeof(QueryBuilder).GetGenericMethod("CreateQuery", new Type[] { castAsType }, new Type[] { subQuery.GetType(), typeof(ColumnMapping[]) });
                            subQueryStatement = genMethod.Invoke(this, new Object[] { subQuery, new ColumnMapping[] { fkColumnDef } }) as SqlStatement;
                        }
                        cteStatements.Add(new SqlStatement(this.m_provider, $"{tablePrefix}cte{cteStatements.Count} AS (").Append(subQueryStatement).Append(")"));
                        //subQueryStatement.And($"{tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = {sqName}{fkTableDef.TableName}.{fkColumnDef.Name} ");

                        selectStatement.Append($"INNER JOIN {tablePrefix}cte{cteStatements.Count - 1} ON ({tablePrefix}{tableMapping.TableName}.{linkColumn.Name} = {tablePrefix}cte{cteStatements.Count - 1}.{fkColumnDef.Name})");

                    }

                }
                else
                    whereClause.And(CreateWhereCondition<TModel>(propertyPath, parm.Value, tablePrefix, scopedTables));

            }

            // Return statement
            SqlStatement retVal = new SqlStatement(this.m_provider);
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
        /// Create a single where condition based on the property info
        /// </summary>
        public SqlStatement CreateWhereCondition<TModel>(String propertyPath, Object value, String tablePrefix, List<TableMapping> scopedTables)
        {

            SqlStatement retVal = new SqlStatement(this.m_provider);

            // Map the type
            var tableMapping = scopedTables.First();
            var propertyInfo = typeof(TModel).GetXmlProperty(propertyPath);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(propertyPath);
            PropertyInfo domainProperty = scopedTables.Select(o => { tableMapping = o; return m_mapper.MapModelProperty(typeof(TModel), o.OrmType, propertyInfo); }).FirstOrDefault(o => o != null);

            // Now map the property path
            var tableAlias = $"{tablePrefix}{tableMapping.TableName}";
            if (domainProperty == null)
                throw new ArgumentException($"Can't find SQL based property for {propertyPath} on {tableMapping.TableName}");
            var columnData = tableMapping.GetColumn(domainProperty);

            // List of parameters
            var lValue = value as IList;
            if (lValue == null)
                lValue = new List<Object>() { value };

            retVal.Append("(");
            foreach (var itm in lValue)
            {
                retVal.Append($"{tableAlias}.{columnData.Name}");
                var semantic = " OR ";
                var iValue = itm;
                if (iValue is String)
                {
                    var sValue = itm as String;
                    switch (sValue[0])
                    {
                        case '<':
                            semantic = " AND ";
                            if (sValue[1] == '=')
                                retVal.Append(" <= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" < ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '>':
                            semantic = " AND ";
                            if (sValue[1] == '=')
                                retVal.Append(" >= ?", CreateParameterValue(sValue.Substring(2), propertyInfo.PropertyType));
                            else
                                retVal.Append(" > ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '!':
                            semantic = " AND ";
                            if (sValue.Equals("!null"))
                                retVal.Append(" IS NOT NULL");
                            else
                                retVal.Append(" <> ?", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '~':
                            if(sValue.Contains("*") || sValue.Contains("?"))
                                retVal.Append(" ILIKE ? ", CreateParameterValue(sValue.Substring(1).Replace("*","%"), propertyInfo.PropertyType));
                            else
                                retVal.Append(" ILIKE '%' || ? || '%'", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
                            break;
                        case '^':
                            retVal.Append(" ILIKE ? || '%'", CreateParameterValue(sValue.Substring(1), propertyInfo.PropertyType));
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
                    retVal.Append(semantic);
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
