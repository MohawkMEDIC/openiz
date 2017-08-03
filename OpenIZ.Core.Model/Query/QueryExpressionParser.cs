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
 * Date: 2016-8-2
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
using System.Runtime.CompilerServices;

namespace OpenIZ.Core.Model.Query
{


    /// <summary>
    /// A class which is responsible for translating a series of Query Parmaeters to a LINQ expression
    /// to be passed to the persistence layer
    /// </summary>
    public class QueryExpressionParser
    {

        // Member cache
        private static Dictionary<Type, Dictionary<String, PropertyInfo>> m_memberCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        // Cast cache
        private static Dictionary<String, Type> m_castCache = new Dictionary<string, Type>();
        // Redirect cache
        private static Dictionary<Type, Dictionary<String, PropertyInfo>> m_redirectCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// Buidl linq expression
        /// </summary>
        /// <typeparam name="TModelType"></typeparam>
        /// <param name="httpQueryParameters"></param>
        /// <returns></returns>
        public static Expression<Func<TModelType, bool>> BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters)
        {
            return BuildLinqExpression<TModelType>(httpQueryParameters, null);
        }

        /// <summary>
        /// Build a LINQ expression
        /// </summary>
        public static Expression<Func<TModelType, bool>> BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, Dictionary<String, Delegate> variables)
        {
            return BuildLinqExpression<TModelType>(httpQueryParameters, variables, true);

        }

		/// <summary>
		/// Builds the linq expression.
		/// </summary>
		/// <typeparam name="TModelType">The type of the t model type.</typeparam>
		/// <param name="httpQueryParameters">The HTTP query parameters.</param>
		/// <param name="variables">The variables.</param>
		/// <param name="safeNullable">if set to <c>true</c> [safe nullable].</param>
		/// <returns>Expression&lt;Func&lt;TModelType, System.Boolean&gt;&gt;.</returns>
		public static Expression<Func<TModelType, bool>> BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, Dictionary<String, Delegate> variables, bool safeNullable)
        {
            var expression = BuildLinqExpression<TModelType>(httpQueryParameters, "o", variables, safeNullable);

            if (expression == null) // No query!
                return (TModelType o) => o != null;
            else
                return Expression.Lambda<Func<TModelType, bool>>(expression.Body, expression.Parameters);
        }

        /// <summary>
        /// Build LINQ expression
        /// </summary>
        public static LambdaExpression BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, string parameterName, Dictionary<String, Delegate> variables = null, bool safeNullable = true)
        {
            var parameterExpression = Expression.Parameter(typeof(TModelType), parameterName);
            Expression retVal = null;
            List<KeyValuePair<String, String[]>> workingValues = new List<KeyValuePair<string, string[]>>();
            // Iterate 
            foreach (var nvc in httpQueryParameters.Where(p => !p.Key.StartsWith("_")).Distinct())
                workingValues.Add(new KeyValuePair<string, string[]>(nvc.Key, nvc.Value.ToArray()));

            // Get the first values
            while (workingValues.Count > 0)
            {
                var currentValue = workingValues.FirstOrDefault();
                workingValues.Remove(currentValue);

                if (currentValue.Value.Count(o => !String.IsNullOrEmpty(o)) == 0)
                    continue;

                // Create accessor expression
                Expression keyExpression = null;
                Expression accessExpression = parameterExpression;
                String[] memberPath = currentValue.Key.Split('.');
                String path = "";

                foreach (var rawMember in memberPath)
                {
                    var pMember = rawMember;
                    String guard = String.Empty,
                        cast = String.Empty;

                    // Update path
                    path += pMember + ".";
                bool coalesce = false;

                    // Guard token?
                    if (pMember.Contains("[") && pMember.EndsWith("]"))
                    {
                        guard = pMember.Substring(pMember.IndexOf("[") + 1, pMember.Length - pMember.IndexOf("[") - 2);
                        pMember = pMember.Substring(0, pMember.IndexOf("["));
                    }
                    if (pMember.EndsWith("?"))
                    {
                        coalesce = true;
                        pMember = pMember.Substring(0, pMember.Length - 1);
                    }
                    if (pMember.Contains("@"))
                    {
                        cast = pMember.Substring(pMember.IndexOf("@") + 1);
                        pMember = pMember.Substring(0, pMember.IndexOf("@"));
                    }

                    // Get member cache for data
                    Dictionary<String, PropertyInfo> memberCache = null;
                    if(!m_memberCache.TryGetValue(accessExpression.Type, out memberCache))
                    {
                        memberCache = new Dictionary<string, PropertyInfo>();
                        lock (m_memberCache)
                            if (!m_memberCache.ContainsKey(accessExpression.Type))
                                m_memberCache.Add(accessExpression.Type, memberCache);
                    }

                    // Add member info
                    PropertyInfo memberInfo = null;
                    if (!memberCache.TryGetValue(pMember, out memberInfo))
                    {
                        memberInfo = accessExpression.Type.GetRuntimeProperties().FirstOrDefault(p => p.GetCustomAttributes<XmlElementAttribute>()?.Any(a => a.ElementName == pMember) == true);
                        if (memberInfo == null)
                            throw new ArgumentOutOfRangeException(currentValue.Key);

                        // Member cache
                        lock (memberCache)
                            if (!memberCache.ContainsKey(pMember))
                                memberCache.Add(pMember, memberInfo);
                    }

                    // Handle XML props
                    if (memberInfo.Name.EndsWith("Xml"))
                        memberInfo = accessExpression.Type.GetRuntimeProperty(memberInfo.Name.Replace("Xml", ""));
                    else if (pMember != memberPath.Last())
                    {
                        PropertyInfo backingFor = null;

                        // Look in member cache
                        if (!m_redirectCache.TryGetValue(accessExpression.Type, out memberCache))
                        {
                            memberCache = new Dictionary<string, PropertyInfo>();
                            lock (m_redirectCache)
                                if (!m_redirectCache.ContainsKey(accessExpression.Type))
                                    m_redirectCache.Add(accessExpression.Type, memberCache);
                        }

                        // Now find backing
                        if (!memberCache.TryGetValue(pMember, out backingFor))
                        {
                            backingFor = accessExpression.Type.GetRuntimeProperties().FirstOrDefault(p => p.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty == memberInfo.Name);
                            // Member cache
                            lock (memberCache)
                                if (!memberCache.ContainsKey(pMember))
                                    memberCache.Add(pMember, backingFor);
                        }
                        
                        if (backingFor != null)
                            memberInfo = backingFor;
                    }

                    accessExpression = Expression.MakeMemberAccess(accessExpression, memberInfo);


                    if (!String.IsNullOrEmpty(cast))
                    {

                        Type castType = null;
                        if (!m_castCache.TryGetValue(cast, out castType))
                        {
                            castType = typeof(QueryExpressionParser).GetTypeInfo().Assembly.ExportedTypes.FirstOrDefault(o => o.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName == cast);
                            if (castType == null)
                                throw new ArgumentOutOfRangeException(nameof(castType), cast);

                            lock (m_castCache)
                                if (!m_castCache.ContainsKey(cast))
                                    m_castCache.Add(cast, castType);
                        }
                        accessExpression = Expression.TypeAs(accessExpression, castType);
                    }
                    if(coalesce)
                    {
                        accessExpression = Expression.Coalesce(accessExpression, Expression.New(accessExpression.Type));
                    }

                    // Guard on classifier?
                    if (!String.IsNullOrEmpty(guard))
                    {
                        Type itemType = accessExpression.Type.GenericTypeArguments[0];
                        Type predicateType = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));
                        ParameterExpression guardParameter = Expression.Parameter(itemType, "guard");
                        if (guard == "null")
                            guard = null;

                        // Cascade the Classifiers to get the access
                        ClassifierAttribute classAttr = itemType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                        if (classAttr == null)
                            throw new InvalidOperationException("No classifier found for guard expression");
                        PropertyInfo classifierProperty = itemType.GetRuntimeProperty(classAttr.ClassifierProperty);
                        // Handle XML props
                        if (classifierProperty.Name.EndsWith("Xml"))
                            classifierProperty = itemType.GetRuntimeProperty(classifierProperty.Name.Replace("Xml", ""));

                        Expression guardAccessor = guardParameter;
                        while (classifierProperty != null && classAttr != null)
                        {
                            guardAccessor = Expression.MakeMemberAccess(guardAccessor, classifierProperty);
                            
                            
                            classAttr = classifierProperty.PropertyType.GetTypeInfo().GetCustomAttribute<ClassifierAttribute>();
                            if (classAttr != null && guard != null)
                                classifierProperty = classifierProperty.PropertyType.GetRuntimeProperty(classAttr.ClassifierProperty);
                            else if (guard == null)
                                break;
                        }

                        MethodInfo whereMethod = typeof(Enumerable).GetGenericMethod("Where",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }) as MethodInfo;

                        // Now make expression
                        Expression guardExpression = null;
                        if(guard != null)
                            foreach(var g in guard.Split('|'))
                            {
                                var expr = Expression.MakeBinary(ExpressionType.Equal, guardAccessor, Expression.Constant(g));
                                if (guardExpression == null)
                                    guardExpression = expr;
                                else
                                    guardExpression = Expression.MakeBinary(ExpressionType.Or, guardExpression, expr);
                            }
                        else
                            guardExpression = Expression.MakeBinary(ExpressionType.Equal, guardAccessor, Expression.Constant(null));

                        var guardLambda = Expression.Lambda(guardExpression, guardParameter);
                        accessExpression = Expression.Call(whereMethod, accessExpression, guardLambda);

                        if (currentValue.Value.Length == 1 && currentValue.Value[0].EndsWith("null"))
                        {
                            var anyMethod = typeof(Enumerable).GetGenericMethod("Any",
                                new Type[] { itemType },
                                new Type[] { accessExpression.Type }) as MethodInfo;
                            accessExpression = Expression.Call(anyMethod, accessExpression);
                            currentValue.Value[0] = currentValue.Value[0].Replace("null", "false");
                        }

                    }
                    // List expression, we want the Any() operator
                    if (accessExpression.Type.GetTypeInfo().ImplementedInterfaces.Any(o => o == typeof(IEnumerable)) &&
                        accessExpression.Type.GetTypeInfo().IsGenericType)
                    {


                        Type itemType = accessExpression.Type.GenericTypeArguments[0];
                        Type predicateType = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

                        var anyMethod = typeof(Enumerable).GetGenericMethod("Any",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }) as MethodInfo;

                        // Add sub-filter
                        NameValueCollection subFilter = new NameValueCollection();
                        subFilter.Add(currentValue.Key.Substring(path.Length), new List<String>(currentValue.Value));

                        // Add collect other parameters
                        foreach (var wv in workingValues.Where(o => o.Key.StartsWith(path)).ToList())
                        {
                            subFilter.Add(wv.Key.Substring(path.Length), new List<String>(wv.Value));
                            workingValues.Remove(wv);
                        }

                        var builderMethod = typeof(QueryExpressionParser).GetGenericMethod(nameof(BuildLinqExpression), new Type[] { itemType }, new Type[] { typeof(NameValueCollection), typeof(String), typeof(Dictionary<String, Delegate>), typeof(bool) });

                        Expression predicate = (builderMethod.Invoke(null, new object[] { subFilter, pMember, variables, safeNullable }) as LambdaExpression);
                        if (predicate == null)
                            continue;
                        keyExpression = Expression.Call(anyMethod, accessExpression, predicate);
                        currentValue = new KeyValuePair<string, string[]>();
                        break;  // skip
                    }

                }

                // Now expression
                var kp = currentValue.Value;
                if (kp != null)
                    foreach (var qValue in kp.Where(o => !String.IsNullOrEmpty(o)))
                    {
                        var value = qValue;
                        var thisAccessExpression = accessExpression;
                        // HACK: Fuzz dates for intervals
                        if ((thisAccessExpression.Type.StripNullable() == typeof(DateTime) ||
                            thisAccessExpression.Type.StripNullable() == typeof(DateTimeOffset)) &&
                            value.Length <= 7 &&
                            !value.StartsWith("~") &&
                            !value.Contains("null")
                            )
                            value = "~" + value;

                        Expression nullCheckExpr = null;

                        // Correct for nullable
                        if (value != "null" && thisAccessExpression.Type.GetTypeInfo().IsGenericType && thisAccessExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                            safeNullable)
                        {
                            nullCheckExpr = Expression.MakeBinary(ExpressionType.NotEqual, thisAccessExpression, Expression.Constant(null));
                            thisAccessExpression = Expression.MakeMemberAccess(thisAccessExpression, accessExpression.Type.GetRuntimeProperty("Value"));
                        }

                        // Process value
                        String pValue = value;
                        ExpressionType et = ExpressionType.Equal;

                        if (String.IsNullOrEmpty(value)) continue;

                        switch (value[0])
                        {
                            case '<':
                                et = ExpressionType.LessThan;
                                pValue = value.Substring(1);
                                if (pValue[0] == '=')
                                {
                                    et = ExpressionType.LessThanOrEqual;
                                    pValue = pValue.Substring(1);
                                }
                                break;
                            case '>':
                                et = ExpressionType.GreaterThan;
                                pValue = value.Substring(1);
                                if (pValue[0] == '=')
                                {
                                    et = ExpressionType.GreaterThanOrEqual;
                                    pValue = pValue.Substring(1);
                                }
                                break;
                            case '^':
                                et = ExpressionType.Equal;
                                if (thisAccessExpression.Type == typeof(String))
                                {
                                    thisAccessExpression = Expression.Call(thisAccessExpression, typeof(String).GetRuntimeMethod("StartsWith", new Type[] { typeof(String) }), Expression.Constant(pValue.Substring(1)));
                                    pValue = "true";
                                }
                                else
                                    throw new InvalidOperationException("^ can only be applied to string properties");

                                break;
                            case '~':
                                et = ExpressionType.Equal;
                                if (thisAccessExpression.Type == typeof(String))
                                {
                                    thisAccessExpression = Expression.Call(thisAccessExpression, typeof(String).GetRuntimeMethod("Contains", new Type[] { typeof(String) }), Expression.Constant(pValue.Substring(1)));
                                    pValue = "true";
                                }
                                else if (thisAccessExpression.Type == typeof(DateTime) ||
                                    thisAccessExpression.Type == typeof(DateTime?))
                                {
                                    pValue = value.Substring(1);
                                    DateTime dateLow = DateTime.ParseExact(pValue, "yyyy-MM-dd".Substring(0, pValue.Length), CultureInfo.InvariantCulture), dateHigh = DateTime.MaxValue;
                                    if (pValue.Length == 4) // Year
                                        dateHigh = new DateTime(dateLow.Year, 12, 31, 23, 59, 59);
                                    else if (pValue.Length == 7)
                                        dateHigh = new DateTime(dateLow.Year, dateLow.Month, DateTime.DaysInMonth(dateLow.Year, dateLow.Month), 23, 59, 59);
                                    else if (pValue.Length == 10)
                                        dateHigh = new DateTime(dateLow.Year, dateLow.Month, dateLow.Day, 23, 59, 59);
                                    if (thisAccessExpression.Type == typeof(DateTime?))
                                        thisAccessExpression = Expression.MakeMemberAccess(thisAccessExpression, thisAccessExpression.Type.GetRuntimeProperty("Value"));
                                    Expression lowerBound = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, thisAccessExpression, Expression.Constant(dateLow)),
                                        upperBound = Expression.MakeBinary(ExpressionType.LessThanOrEqual, thisAccessExpression, Expression.Constant(dateHigh));
                                    thisAccessExpression = Expression.MakeBinary(ExpressionType.AndAlso, lowerBound, upperBound);
                                    pValue = "true";
                                }
                                else
                                    throw new InvalidOperationException("~ can only be applied to string properties");

                                break;
                            case '!':
                                et = ExpressionType.NotEqual;
                                pValue = value.Substring(1);
                                break;
                        }

                        // The expression
                        Expression valueExpr = null;
                        if (pValue == "null")
                            valueExpr = Expression.Constant(null);
                        else if (pValue.StartsWith("$"))
                        {
                            Delegate val = null;
                            if (variables.TryGetValue(pValue.Replace("$", ""), out val))
                            {
                                if (val.GetMethodInfo().GetParameters().Length > 0)
                                    valueExpr = Expression.Invoke(Expression.Constant(val));
                                else
                                    valueExpr = Expression.Call(val.Target == null ? null : Expression.Constant(val.Target), val.GetMethodInfo());
                            }
                            else
                                valueExpr = Expression.Constant(null);
                        }
                        else if (thisAccessExpression.Type == typeof(String))
                            valueExpr = Expression.Constant(pValue);
                        else if (thisAccessExpression.Type == typeof(DateTime) || thisAccessExpression.Type == typeof(DateTime?))
                            valueExpr = Expression.Constant(DateTime.Parse(pValue));
                        else if (thisAccessExpression.Type == typeof(DateTimeOffset) || thisAccessExpression.Type == typeof(DateTimeOffset?))
                            valueExpr = Expression.Constant(DateTimeOffset.Parse(pValue));
                        else if (thisAccessExpression.Type == typeof(Guid))
                            valueExpr = Expression.Constant(Guid.Parse(pValue));
                        else if (thisAccessExpression.Type == typeof(Guid?))
                            valueExpr = Expression.Convert(Expression.Constant(Guid.Parse(pValue)), typeof(Guid?));
                        else if (thisAccessExpression.Type.GetTypeInfo().IsEnum)
                        {
                            int tryParse = 0;
                            if(Int32.TryParse(pValue, out tryParse))
                               valueExpr = Expression.Constant(Enum.ToObject(thisAccessExpression.Type, Int32.Parse(pValue)));
                            else
                                valueExpr = Expression.Constant(Enum.Parse(thisAccessExpression.Type, pValue));

                        }
                        else
                            valueExpr = Expression.Constant(Convert.ChangeType(pValue, thisAccessExpression.Type));
                        if (valueExpr.Type != thisAccessExpression.Type)
                            valueExpr = Expression.Convert(valueExpr, thisAccessExpression.Type);
                        Expression singleExpression = Expression.MakeBinary(et, thisAccessExpression, valueExpr);

                        if (nullCheckExpr != null)
                            singleExpression = Expression.MakeBinary(ExpressionType.AndAlso, nullCheckExpr, singleExpression);

                        if (keyExpression == null)
                            keyExpression = singleExpression;
                        else if (et == ExpressionType.Equal)
                            keyExpression = Expression.MakeBinary(ExpressionType.OrElse, keyExpression, singleExpression);
                        else
                            keyExpression = Expression.MakeBinary(ExpressionType.AndAlso, keyExpression, singleExpression);
                    }

                if (retVal == null)
                    retVal = keyExpression;
                else
                    retVal = Expression.MakeBinary(ExpressionType.AndAlso, retVal, keyExpression);

            }

            //Debug.WriteLine(String.Format("Converted {0} to {1}", httpQueryParameters, retVal));

            if (retVal == null)
                return null;
            return Expression.Lambda(retVal, parameterExpression);

        }
    }
}
