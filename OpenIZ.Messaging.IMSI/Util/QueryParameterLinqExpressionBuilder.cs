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
        /// Build a LINQ expression
        /// </summary>
        public Expression<Func<TModelType, bool>> BuildLinqExpression<TModelType>(NameValueCollection httpQueryParameters)
        {
            var parameterExpression = Expression.Parameter(typeof(TModelType), "o");
            Expression retVal = null;
            // Iterate 
            foreach (var nvc in httpQueryParameters.AllKeys.Distinct())
            {
                // Create accessor expression
                Expression accessExpression = parameterExpression;
                String[] memberPath = nvc.Split('.');
                foreach(var pMember in memberPath)
                {
                    var memberInfo = accessExpression.Type.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<XmlElementAttribute>()?.ElementName == pMember);
                    if (memberInfo == null)
                        throw new ArgumentOutOfRangeException(nvc);

                    // Handle XML props
                    if (memberInfo.Name.EndsWith("Xml"))
                        memberInfo = accessExpression.Type.GetProperty(memberInfo.Name.Replace("Xml", ""));
                    accessExpression = Expression.MakeMemberAccess(accessExpression, memberInfo);

                }

                // Now expression
                Expression keyExpression = null;
                var kp = httpQueryParameters.GetValues(nvc);
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
                        valueExpr = Expression.Constant(DateTime.ParseExact(pValue, "o", System.Globalization.CultureInfo.InvariantCulture));
                    else if (accessExpression.Type == typeof(DateTimeOffset))
                        valueExpr = Expression.Constant(DateTimeOffset.ParseExact(pValue, "o", CultureInfo.InvariantCulture));
                    else if (accessExpression.Type == typeof(Guid))
                        valueExpr = Expression.Constant(Guid.Parse(pValue));
                    else if (accessExpression.Type == typeof(Decimal))
                        valueExpr = Expression.Constant(Decimal.Parse(pValue));
                    else
                        valueExpr = Expression.Convert(Expression.Constant(pValue), accessExpression.Type);

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
            return Expression.Lambda<Func<TModelType, bool>>(retVal, parameterExpression);

        }
    }
}
