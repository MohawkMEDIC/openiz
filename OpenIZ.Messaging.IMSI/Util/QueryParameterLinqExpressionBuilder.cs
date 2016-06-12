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
 * Date: 2016-2-1
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

namespace OpenIZ.Messaging.IMSI.Util
{

   
    /// <summary>
    /// A class which is responsible for translating a series of Query Parmaeters to a LINQ expression
    /// to be passed to the persistence layer
    /// </summary>
    public class QueryParameterLinqExpressionBuilder
    {
        private TraceSource m_tracer = new TraceSource("OpenIZ.Messaging.IMSI");

        /// <summary>
        /// Get generic method
        /// </summary>
        private MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, BindingFlags flags)
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
        public Expression<Func<TModelType, bool>> BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters)
        {
            var expression = this.BuildLinqExpression<TModelType>(httpQueryParameters, "o");
            return Expression.Lambda<Func<TModelType, bool>>(expression.Body, expression.Parameters);
        }

        /// <summary>
        /// Build LINQ expression
        /// </summary>
        public LambdaExpression BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters, String parameterName)
        { 
            var parameterExpression = Expression.Parameter(typeof(TModelType), parameterName);
            Expression retVal = null;
            List<KeyValuePair<String, String[]>> workingValues = new List<KeyValuePair<string, string[]>>();
            // Iterate 
            foreach (var nvc in httpQueryParameters.AllKeys.Where(p=>!p.StartsWith("_")).Distinct())
                workingValues.Add(new KeyValuePair<string, string[]>(nvc, httpQueryParameters.GetValues(nvc)));

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
                    String guard = String.Empty;

                    // Update path
                    path += pMember + ".";

                    // Guard token?
                    if (pMember.Contains("[") && pMember.EndsWith("]"))
                    {
                        guard = pMember.Substring(pMember.IndexOf("[") + 1, pMember.Length - pMember.IndexOf("[") - 2);
                        pMember = pMember.Substring(0, pMember.IndexOf("["));
                    }

                    // Get member info
                    var memberInfo = accessExpression.Type.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<XmlElementAttribute>()?.ElementName == pMember);
                    if (memberInfo == null)
                        throw new ArgumentOutOfRangeException(currentValue.Key);


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
                    
                    // Guard on classifier?
                    if(!String.IsNullOrEmpty(guard))
                    {
                        Type itemType = accessExpression.Type.GetGenericArguments()[0];
                        Type predicateType = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));
                        ParameterExpression guardParameter = Expression.Parameter(itemType, "guard");

                        // Cascade the Classifiers to get the access
                        ClassifierAttribute classAttr = itemType.GetCustomAttribute<ClassifierAttribute>();
                        if (classAttr == null)
                            throw new InvalidOperationException("No classifier found for guard expression");
                        PropertyInfo classifierProperty = itemType.GetProperty(classAttr.ClassifierProperty);
                        Expression guardAccessor = guardParameter;
                        while (classifierProperty != null && classAttr != null)
                        {
                            guardAccessor = Expression.MakeMemberAccess(guardAccessor, classifierProperty);
                            classAttr = classifierProperty.PropertyType.GetCustomAttribute<ClassifierAttribute>();
                            if(classAttr != null)
                                classifierProperty = classifierProperty.PropertyType.GetProperty(classAttr.ClassifierProperty);
                        }

                        var whereMethod = this.GetGenericMethod(typeof(Enumerable), "Where",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }, BindingFlags.Static) as MethodInfo;

                        // Now make expression
                        var guardLambda = Expression.Lambda(Expression.MakeBinary(ExpressionType.Equal, guardAccessor, Expression.Constant(guard)), guardParameter);
                        accessExpression = Expression.Call(whereMethod, accessExpression, guardLambda);

                    }
                    // List expression, we want the Any() operator
                    if (accessExpression.Type.GetInterface(typeof(IEnumerable).FullName) != null &&
                        accessExpression.Type.IsGenericType)
                    {

                        Type itemType = accessExpression.Type.GetGenericArguments()[0];
                        Type predicateType = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

                        var anyMethod = this.GetGenericMethod(typeof(Enumerable), "Any",
                            new Type[] { itemType },
                            new Type[] { accessExpression.Type, predicateType }, BindingFlags.Static) as MethodInfo;
                        
                        // Add sub-filter
                        NameValueCollection subFilter = new NameValueCollection();
                        foreach(var val in currentValue.Value)
                            subFilter.Add(currentValue.Key.Substring(path.Length), val);
                        // Add collect other parameters
                        foreach (var wv in workingValues.Where(o => o.Key.StartsWith(path)).ToList())
                        {
                            foreach (var val in wv.Value)
                                subFilter.Add(wv.Key.Substring(path.Length), val);
                            workingValues.Remove(wv);
                        }

                        var builderMethod = this.GetGenericMethod(typeof(QueryParameterLinqExpressionBuilder), nameof(BuildLinqExpression), new Type[] { itemType }, new Type[] { typeof(NameValueCollection), typeof(String) }, BindingFlags.Instance | BindingFlags.Public);

                        Expression predicate = (builderMethod.Invoke(this, new object[] { subFilter, pMember }) as LambdaExpression);
                        keyExpression = Expression.Call(anyMethod, accessExpression, predicate);
                        currentValue = new KeyValuePair<string, string[]>();
                        break;  // skip
                    }

                }

                // Now expression
                var kp = currentValue.Value;
                if(kp != null)
                    foreach(var value in kp)
                    {
                        String pValue = value;
                        ExpressionType et = ExpressionType.Equal;
                        switch(value[0])
                        {
                            case '<':
                                et = ExpressionType.LessThan;
                                pValue = value.Substring(1);
                                break;
                            case '>':
                                et = ExpressionType.GreaterThan;
                                pValue = value.Substring(1);
                                break;
                            case '~':
                                et = ExpressionType.Equal;
                                if (accessExpression.Type != typeof(String))
                                    throw new InvalidOperationException("~ can only be applied to string properties");
                                accessExpression = Expression.Call(accessExpression, typeof(String).GetMethod("Contains"), Expression.Constant(pValue.Substring(1).Replace("*","/")));
                                pValue = "true";
                                break;
                            case '=':
                                switch(value[1])
                                {
                                    case '<':
                                        et = ExpressionType.LessThanOrEqual;
                                        pValue = value.Substring(2);
                                        break;
                                    case '>':
                                        et = ExpressionType.GreaterThanOrEqual;
                                        pValue = value.Substring(2);
                                        break;
                                    default:
                                        et = ExpressionType.Equal;
                                        pValue = value.Substring(1);

                                        break;
                                }
                                break;
                            case '!':
                                et = ExpressionType.NotEqual;
                                pValue = value.Substring(1);
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

            this.m_tracer.TraceInformation("Converted {0} to {1}", httpQueryParameters, retVal);
            return Expression.Lambda(retVal, parameterExpression);

        }
    }
}
