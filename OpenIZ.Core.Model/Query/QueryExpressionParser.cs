/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-7
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
using OpenIZ.Core.Model.Reflection;
using OpenIZ.Core.Model.Reflection;
using System.Runtime.CompilerServices;

namespace OpenIZ.Core.Model.Query
{

   
    /// <summary>
    /// A class which is responsible for translating a series of Query Parmaeters to a LINQ expression
    /// to be passed to the persistence layer
    /// </summary>
    public class QueryExpressionParser
    {

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
            var expression = BuildLinqExpression<TModelType>(httpQueryParameters, "o", variables);

            if (expression == null) // No query!
                return (TModelType o) => o != null;
            else 
                return Expression.Lambda<Func<TModelType, bool>>(expression.Body, expression.Parameters);
        }

        /// <summary>
        /// Build LINQ expression
        /// </summary>
        public static LambdaExpression BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, string parameterName, Dictionary<String, Delegate> variables = null)
        { 
            var parameterExpression = Expression.Parameter(typeof(TModelType), parameterName);
            Expression retVal = null;
            List<KeyValuePair<String, String[]>> workingValues = new List<KeyValuePair<string, string[]>>();
            // Iterate 
            foreach (var nvc in httpQueryParameters.Where(p=>!p.Key.StartsWith("_")).Distinct())
                workingValues.Add(new KeyValuePair<string, string[]>(nvc.Key, nvc.Value.ToArray()));

            // Get the first values
            while(workingValues.Count > 0)
            {
                var currentValue = workingValues.FirstOrDefault();
                workingValues.Remove(currentValue);

                // Create accessor expression
                Expression keyExpression = null;
                Expression accessExpression = parameterExpression;
                String[] memberPath = currentValue.Key.Split('.');
                String path = "";
                foreach(var rawMember in memberPath)
                {
                    var pMember = rawMember;
                    String guard = String.Empty,
                        cast = String.Empty;

                    // Update path
                    path += pMember + ".";

                    // Guard token?
                    if (pMember.Contains("[") && pMember.EndsWith("]"))
                    {
                        guard = pMember.Substring(pMember.IndexOf("[") + 1, pMember.Length - pMember.IndexOf("[") - 2);
                        pMember = pMember.Substring(0, pMember.IndexOf("["));
                    }
                    if(pMember.Contains("@"))
                    {
                        cast = pMember.Substring(pMember.IndexOf("@") + 1);
                        pMember = pMember.Substring(0, pMember.IndexOf("@"));
                    }

                    // Get member info
                    var memberInfo = accessExpression.Type.GetRuntimeProperties().FirstOrDefault(p => p.GetCustomAttributes<XmlElementAttribute>()?.Any(a=>a.ElementName == pMember) == true);
                    if (memberInfo == null)
                        throw new ArgumentOutOfRangeException(currentValue.Key);

                    // Handle XML props
                    if (memberInfo.Name.EndsWith("Xml"))
                        memberInfo = accessExpression.Type.GetRuntimeProperty(memberInfo.Name.Replace("Xml", ""));
                    else if (pMember != memberPath.Last())
                    {
                        var backingFor = accessExpression.Type.GetRuntimeProperties().FirstOrDefault(p => p.GetCustomAttribute<SerializationReferenceAttribute>()?.RedirectProperty == memberInfo.Name);
                        if (backingFor != null)
                            memberInfo = backingFor;
                    }

                    accessExpression = Expression.MakeMemberAccess(accessExpression, memberInfo);


                    if (!String.IsNullOrEmpty(cast))
                    {
                        Type castType = typeof(QueryExpressionParser).GetTypeInfo().Assembly.ExportedTypes.FirstOrDefault(o => o.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName == cast);
                        if (castType == null)
                            throw new ArgumentOutOfRangeException(nameof(castType), cast);
                        accessExpression = Expression.TypeAs(accessExpression, castType);
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

                        var whereMethod = typeof(Enumerable).GetGenericMethod("Where",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }) as MethodInfo;

                        // Now make expression
                        var guardLambda = Expression.Lambda(Expression.MakeBinary(ExpressionType.Equal, guardAccessor, Expression.Constant(guard)), guardParameter);
                        accessExpression = Expression.Call(whereMethod, accessExpression, guardLambda);

                    }
                    // List expression, we want the Any() operator
                    if (accessExpression.Type.GetTypeInfo().ImplementedInterfaces.Any(o=>o == typeof(IEnumerable)) &&
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

                        var builderMethod = typeof(QueryExpressionParser).GetGenericMethod(nameof(BuildLinqExpression), new Type[] { itemType }, new Type[] { typeof(NameValueCollection), typeof(String), typeof(Dictionary<String, Delegate>) });

                        Expression predicate = (builderMethod.Invoke(null, new object[] { subFilter, pMember, variables }) as LambdaExpression);
                        keyExpression = Expression.Call(anyMethod, accessExpression, predicate);
                        currentValue = new KeyValuePair<string, string[]>();
                        break;  // skip
                    }

                }

               
                // Now expression
                var kp = currentValue.Value;
                if(kp != null)
                    foreach(var qValue in kp)
                    {
                        var value = qValue;
                        // HACK: Fuzz dates for intervals
                        if ((accessExpression.Type.StripNullable() == typeof(DateTime) ||
                            accessExpression.Type.StripNullable() == typeof(DateTimeOffset)) &&
                            value.Length <= 7 &&
                            !value.StartsWith("~") &&
                            !value.Contains("null")
                            )
                            value = "~" + value;

                        Expression nullCheckExpr = null;

                        // Correct for nullable
                        if (value != "null" && accessExpression.Type.GetTypeInfo().IsGenericType && accessExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            nullCheckExpr = Expression.MakeBinary(ExpressionType.NotEqual, accessExpression, Expression.Constant(null));
                            accessExpression = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetRuntimeProperty("Value"));
                        }

                        // Process value
                        String pValue = value;
                        ExpressionType et = ExpressionType.Equal;
                        switch(value[0])
                        {
                            case '<':
                                et = ExpressionType.LessThan;
                                pValue = value.Substring(1);
                                if(pValue[0] == '=')
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
                            case '~':
                                et = ExpressionType.Equal;
                                if (accessExpression.Type == typeof(String))
                                {
                                    accessExpression = Expression.Call(accessExpression, typeof(String).GetRuntimeMethod("Contains", new Type[] { typeof(String) }), Expression.Constant(pValue.Substring(1).Replace("*", "/")));
                                    pValue = "true";
                                }
                                else if(accessExpression.Type == typeof(DateTime) ||
                                    accessExpression.Type == typeof(DateTime?))
                                {
                                    pValue = value.Substring(1);
                                    DateTime dateLow = DateTime.ParseExact(pValue, "yyyy-MM-dd".Substring(0, pValue.Length), CultureInfo.InvariantCulture), dateHigh = DateTime.MaxValue;
                                    if (pValue.Length == 4) // Year
                                        dateHigh = new DateTime(dateLow.Year, 12, 31, 23, 59, 59);
                                    else if (pValue.Length == 7)
                                        dateHigh = new DateTime(dateLow.Year, dateLow.Month, DateTime.DaysInMonth(dateLow.Year, dateLow.Month), 23, 59, 59);
                                    else if (pValue.Length == 10)
                                        dateHigh = new DateTime(dateLow.Year, dateLow.Month, dateLow.Day, 23, 59, 59);
                                    if (accessExpression.Type == typeof(DateTime?))
                                        accessExpression = Expression.MakeMemberAccess(accessExpression, accessExpression.Type.GetRuntimeProperty("Value"));
                                    Expression lowerBound = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, accessExpression, Expression.Constant(dateLow)),
                                        upperBound = Expression.MakeBinary(ExpressionType.LessThanOrEqual, accessExpression, Expression.Constant(dateHigh));
                                    accessExpression = Expression.MakeBinary(ExpressionType.AndAlso, lowerBound, upperBound);
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
                        else if(pValue.StartsWith("$"))
                        {
                            Delegate val = null;
                            if (variables.TryGetValue(pValue.Replace("$", ""), out val))
                            {
                                if(val.GetMethodInfo().GetParameters().Length > 0)
                                    valueExpr = Expression.Invoke(Expression.Constant(val));
                                else
                                    valueExpr = Expression.Call(val.Target == null ? null : Expression.Constant(val.Target), val.GetMethodInfo());
                            }
                            else
                                valueExpr = Expression.Constant(null);
                        }
                        else if (accessExpression.Type == typeof(String))
                            valueExpr = Expression.Constant(pValue);
                        else if (accessExpression.Type == typeof(DateTime) || accessExpression.Type == typeof(DateTime?))
                            valueExpr = Expression.Constant(DateTime.Parse(pValue));
                        else if (accessExpression.Type == typeof(DateTimeOffset) || accessExpression.Type == typeof(DateTimeOffset?))
                            valueExpr = Expression.Constant(DateTimeOffset.Parse(pValue));
                        else if (accessExpression.Type == typeof(Guid))
                            valueExpr = Expression.Constant(Guid.Parse(pValue));
                        else if (accessExpression.Type == typeof(Guid?))
                            valueExpr = Expression.Convert(Expression.Constant(Guid.Parse(pValue)), typeof(Guid?));
                        else if (accessExpression.Type.GetTypeInfo().IsEnum)
                            valueExpr = Expression.Constant(Enum.ToObject(accessExpression.Type, Int32.Parse(pValue)));
                        else
                            valueExpr = Expression.Constant(Convert.ChangeType(pValue, accessExpression.Type));
                        if (valueExpr.Type != accessExpression.Type)
                            valueExpr = Expression.Convert(valueExpr, accessExpression.Type);
                        Expression singleExpression = Expression.MakeBinary(et, accessExpression, valueExpr);

                        if (nullCheckExpr != null)
                            singleExpression = Expression.MakeBinary(ExpressionType.AndAlso, nullCheckExpr, singleExpression);

                        if (keyExpression == null)
                            keyExpression = singleExpression;
                        else if(et == ExpressionType.Equal)
                            keyExpression = Expression.MakeBinary(ExpressionType.OrElse, keyExpression, singleExpression);
                        else
                            keyExpression = Expression.MakeBinary(ExpressionType.AndAlso, keyExpression, singleExpression);
                    }

                if (retVal == null)
                    retVal = keyExpression;
                else
                    retVal = Expression.MakeBinary(ExpressionType.AndAlso, retVal, keyExpression);

            }

            Debug.WriteLine(String.Format("Converted {0} to {1}", httpQueryParameters, retVal));

            if (retVal == null)
                return null;
            return Expression.Lambda(retVal, parameterExpression);

        }
    }
}
