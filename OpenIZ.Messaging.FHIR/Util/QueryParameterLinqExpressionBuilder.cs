/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-27
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Globalization;
using OpenIZ.Core.Model.Attributes;
using System.Collections;
using System.IO;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR;
using OpenIZ.Core.Model;

namespace OpenIZ.Messaging.FHIR.Util
{

    /// <summary>
    /// OpenIZ FHIR query
    /// </summary>
    public class OpenIzFhirQuery<TModel> : FhirQuery
    {

        /// <summary>
        /// Query expression
        /// </summary>
        public LambdaExpression QueryExpression { get; set; }

        /// <summary>
        /// Gets or sets the composed query
        /// </summary>
        public Expression<Func<TModel, bool>> ToPredicate()
        {
            return Expression.Lambda<Func<TModel, bool>>(this.QueryExpression.Body, this.QueryExpression.Parameters);
        }
    }
   
    /// <summary>
    /// A class which is responsible for translating a series of Query Parmaeters to a LINQ expression
    /// to be passed to the persistence layer
    /// </summary>
    public class QueryParameterLinqExpressionBuilder
    {
        private static TraceSource s_tracer = new TraceSource("OpenIZ.Messaging.FHIR");

        // The query parameter map
        private static QueryParameterMap s_map;

        /// <summary>
        /// Static CTOR
        /// </summary>
        static QueryParameterLinqExpressionBuilder () {
            using (Stream s = typeof(QueryParameterLinqExpressionBuilder).Assembly.GetManifestResourceStream("OpenIZ.Messaging.FHIR.ParameterMap.xml"))
            {
                XmlSerializer xsz = new XmlSerializer(typeof(QueryParameterMap));
                s_map = xsz.Deserialize(s) as QueryParameterMap;
            }
        }

        /// <summary>
        /// Get generic method
        /// </summary>
        private static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }


        /// <summary>
        /// Build a LINQ expression
        /// </summary>
        public static OpenIzFhirQuery<TModelType> BuildFhirQueryObject<TModelType>(NameValueCollection httpQueryParameters)  
        {
            var retVal = new OpenIzFhirQuery<TModelType>()
            {
                QueryExpression = BuildLinqExpression<TModelType>(httpQueryParameters, "o"),
            };

            // Control parameters
            foreach(var itm in httpQueryParameters.AllKeys.Where(o=>o.StartsWith("_")))
            {
                switch(itm)
                {
                    case "_count":
                        // Forces the caller to use "Take()"
                        retVal.Quantity = Int32.Parse(httpQueryParameters[itm]);
                        break;
                    case "_summary":
                        // Forces the caller to expand history items
                        retVal.IncludeHistory = Boolean.Parse(httpQueryParameters[itm]);
                        break;
                    case "_offset":
                        // Forces the caller to use "Skip()"
                        retVal.Start = Int32.Parse(httpQueryParameters[itm]);
                        break;
                    case "_include":
                        // TODO: Support this
                        break;
                    case "_contained":
                        retVal.IncludeContained = Boolean.Parse(httpQueryParameters[itm]);
                        break;
                    case "_lastUpdatedTime":
                        // What this piece of code does:
                        // 1. Determine lastUpdateTime as DTO
                        // 2. Modifies the expression to include: 
                        //     a. When Creation time is present: o=>(filter) && (o.CreationTime >= _lastUpdatedTime)
                        //     b. When Update time is present: o=>(filter) && (o.UpdatedTime != null && o.UpdatedTime >= _lastUpdatedTime)
                        //     c. When Creation and Update is present: o=>(filter) && (o.CreationTime >= _lastUpdatedTime || (o.UpdatedTime != null && o.UpdatedTime >= _lastUpdatedTime))
                        // 3. Profit!
                        var updt = httpQueryParameters[itm];
                        if (updt.StartsWith(">")) updt = updt.Substring(1);
                        DateTimeOffset dt = DateTimeOffset.Parse(updt);

                        PropertyInfo updateTimeProperty = typeof(TModelType).GetProperty("UpdatedTime"),
                            createTimeProperty = typeof(TModelType).GetProperty("CreationTime");

                        Expression timeFilter = null;
                        if (createTimeProperty != null)
                            timeFilter = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, Expression.MakeMemberAccess(retVal.QueryExpression.Parameters[0], createTimeProperty),
                                Expression.Constant(dt));
                        if(updateTimeProperty != null)
                        {

                            var subFilter = Expression.AndAlso(
                                Expression.MakeBinary(ExpressionType.NotEqual, Expression.MakeMemberAccess(retVal.QueryExpression.Parameters[0], updateTimeProperty), Expression.Constant(null)),
                                Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, Expression.MakeMemberAccess(retVal.QueryExpression.Parameters[0], updateTimeProperty), Expression.Constant(dt))
                            );
                            if (timeFilter == null)
                                timeFilter = Expression.OrElse(timeFilter, subFilter);
                            else timeFilter = subFilter;
                        }

                        // now append to query expression
                        if (timeFilter != null)
                            retVal.QueryExpression =Expression.Lambda(Expression.AndAlso(retVal.QueryExpression.Body, timeFilter), retVal.QueryExpression.Parameters);

                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Build LINQ expression
        /// </summary>
        public static LambdaExpression BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, String parameterName)
        { 
            var parameterExpression = Expression.Parameter(typeof(TModelType), parameterName);
            Expression retVal = null;
            var typeParameterMap = s_map.Map.FirstOrDefault(o => o.SourceType == typeof(TModelType));
            
            // Iterate 
            foreach (var rawFhirParm in httpQueryParameters.AllKeys.Distinct())
            {

                var parmMap = typeParameterMap.Map.FirstOrDefault(o => o.FhirName == rawFhirParm);
                if (parmMap == null)
                    continue; // TODO: Filter the raw parms

                var nvc = parmMap.ModelName;

                // Create accessor expression
                Expression keyExpression = null;
                Expression accessExpression = parameterExpression;
                String[] memberPath = nvc.Split('.');
                String path = "";
                foreach(var pMember in memberPath)
                {
                    path += pMember + ".";
                    var memberInfo = accessExpression.Type.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<XmlElementAttribute>()?.ElementName == pMember);
                    if (memberInfo == null)
                        throw new ArgumentOutOfRangeException(nvc);


                    // Handle XML props
                    if (memberInfo.Name.EndsWith("Xml"))
                        memberInfo = accessExpression.Type.GetProperty(memberInfo.Name.Replace("Xml", ""));
                    else if (pMember != memberPath.Last())
                    {
                        var backingFor = accessExpression.Type.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<DelayLoadAttribute>()?.KeyPropertyName == memberInfo.Name);
                        if (backingFor != null)
                            memberInfo = backingFor;
                    }
                    accessExpression = Expression.MakeMemberAccess(accessExpression, memberInfo);

                    // List expression, we want the Any() operator
                    if(accessExpression.Type.GetInterface(typeof(IList).FullName) != null)
                    {

                        Type itemType = accessExpression.Type.GetGenericArguments()[0];
                        Type predicateType = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

                        var anyMethod = GetGenericMethod(typeof(Enumerable), "Any",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }, BindingFlags.Static) as MethodInfo;
                        
                        // Add sub-filter
                        NameValueCollection subFilter = new NameValueCollection();
                        foreach(var val in httpQueryParameters.GetValues(nvc))
                            subFilter.Add(nvc.Substring(path.Length), val);
                        var builderMethod = GetGenericMethod(typeof(QueryParameterLinqExpressionBuilder), nameof(BuildLinqExpression), new Type[] { itemType }, new Type[] { typeof(NameValueCollection), typeof(String) }, BindingFlags.Instance | BindingFlags.Public);

                        Expression predicate = (builderMethod.Invoke(null, new object[] { subFilter, pMember }) as LambdaExpression);
                        keyExpression = Expression.Call(anyMethod, accessExpression, predicate);
                        httpQueryParameters.Remove(nvc);
                        break;  // skip
                    }

                }

                // Now expression
                var kp = httpQueryParameters.GetValues(nvc);
                if(kp != null)
                    foreach(var value in kp)
                    {

                        String pValue = value;
                        ExpressionType et = ExpressionType.Equal;
                        if(parmMap.FhirType == FhirQueryType.Number ||
                            parmMap.FhirType == FhirQueryType.Date)
                            switch(value.Substring(0, 2))
                            {
                                case "lt":
                                    et = ExpressionType.LessThan;
                                    pValue = value.Substring(2);
                                    break;
                                case "le":
                                    et = ExpressionType.LessThanOrEqual;
                                    pValue = value.Substring(2);
                                    break;
                                case "gt":
                                    et = ExpressionType.GreaterThan;
                                    pValue = value.Substring(2);
                                    break;
                                case "ge":
                                    et = ExpressionType.GreaterThanOrEqual;
                                    pValue = value.Substring(2);
                                    break;
                                case "ne":
                                    et = ExpressionType.NotEqual;
                                    pValue = value.Substring(2);
                                    break;
                            }
                       
                        // The expression
                        Expression valueExpr = null;
                        if (accessExpression.Type == typeof(String))
                            valueExpr = Expression.Constant(pValue);
                        else if (accessExpression.Type == typeof(DateTime))
                            valueExpr = Expression.Constant(DateTime.Parse(pValue));
                        else if (accessExpression.Type == typeof(DateTimeOffset))
                            valueExpr = Expression.Constant(DateTimeOffset.Parse(pValue));
                        else if (accessExpression.Type == typeof(Guid))
                            valueExpr = Expression.Constant(Guid.Parse(pValue));
                        // TODO: Complex cases like identifier or code
                        else
                            valueExpr = Expression.Constant(Convert.ChangeType(pValue, accessExpression.Type));
                        Expression singleExpression = Expression.MakeBinary(et, accessExpression, valueExpr);
                        if (keyExpression == null)
                            keyExpression = singleExpression;
                        else
                            keyExpression = Expression.MakeBinary(ExpressionType.OrElse, keyExpression, singleExpression);
                    }

                if (retVal == null)
                    retVal = keyExpression;
                else
                    retVal = Expression.MakeBinary(ExpressionType.AndAlso, retVal, keyExpression);

            }

            s_tracer.TraceInformation("Converted {0} to {1}", httpQueryParameters, retVal);
            return Expression.Lambda(retVal, parameterExpression);

        }
    }
}
