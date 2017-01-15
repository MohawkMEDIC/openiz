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
    /// Query builder
    /// </summary>
    public static class QueryBuilder
    {
        // Regex to extract property, guards and cast
        private const string m_extractionRegex = @"^(\w*?)(\[(\w*)\])?(\@(\w*))?(\.(.*))?$";

        // Mapper
        private static ModelMapper m_mapper = new ModelMapper(typeof(QueryBuilder).Assembly.GetManifestResourceStream("OpenIZ.Persistence.Data.ADO.Data.Map.ModelMap.xml"));
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
        /// Query query 
        /// </summary>
        /// <param name="query"></param>
        public static SqlStatement CreateQuery<TModel>(IEnumerable<KeyValuePair<String, Object>> query)
        {

            var tableMap = TableMapping.Get(typeof(TModel));
            SqlStatement retVal = new SqlStatement($"SELECT * FROM {tableMap.TableName} ");

            // Is this a sub-type, if so we want to join
            if(typeof(DbSubTable).IsAssignableFrom(typeof(TModel)))
            {
                var t = typeof(TModel);
                //while(t.)
            }

            // We want to process each query and build WHERE clauses - these where clauses are based off of the JSON / XML names
            // on the model, so we have to use those for the time being before translating to SQL
            Regex re = new Regex(m_extractionRegex);
            List<KeyValuePair<String, Object>> workingParameters = new List<KeyValuePair<string, object>>(query);

            while(workingParameters.Count > 0)
            {
                var parm = workingParameters.First();

                // Match the regex and process
                var matches = re.Match(parm.Key);
                if (!matches.Success) throw new ArgumentOutOfRangeException(parm.Key);

                // First we want to collect all the working parameters 
                string propertyPath = matches.Groups[PropertyRegexGroup].Value,
                    castAs = matches.Groups[CastRegexGroup].Value,
                    guard = matches.Groups[GuardRegexGroup].Value,
                    subProperty = matches.Groups[SubPropertyRegexGroup].Value;

                // Next, we want to construct the 
                
            }

            return retVal;

        }

        /// <summary>
        /// Create a single where condition based on the property info
        /// </summary>
        public static SqlStatement CreateWhereCondition<TModel>(String propertyPath, Object value, String tableAlias = null)
        {

            SqlStatement retVal = new SqlStatement();
            
            // Map the type
            var tableType = m_mapper.MapModelType(typeof(TModel));
            var tableMapping = TableMapping.Get(tableType);

            // Map the property 
            var propertyInfo = typeof(TModel).GetXmlProperty(propertyPath);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(propertyPath);
            propertyInfo = m_mapper.MapModelProperty<TModel>(propertyInfo);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(propertyPath);

            // Now map the property path
            tableAlias = tableAlias ?? tableMapping.TableName;
            var columnData = tableMapping.GetColumn(propertyInfo);

            // List of parameters
            var lValue = value as IList;
            if (lValue == null)
                lValue = new List<Object>() { value };

            retVal.Append("(");
            foreach (var itm in lValue)
            {
                var iValue = itm;

                if (iValue is String)
                {
                    var sValue = itm as String;
                    retVal.Append($"{tableAlias}.{columnData.Name}");
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

                if (lValue.IndexOf(itm) < lValue.Count)
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
