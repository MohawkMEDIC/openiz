/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using ExpressionEvaluator;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Query;
using OpenIZ.Protocol.Xml.Model.XmlLinq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Represents a when clause
    /// </summary>
    [XmlType(nameof(ProtocolWhenClauseCollection), Namespace = "http://openiz.org/cdss")]
    public class ProtocolWhenClauseCollection
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ProtocolWhenClauseCollection));

        /// <summary>
        /// Operator 
        /// </summary>
        [XmlAttribute("evaluation")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Clause evelators
        /// </summary>
        [XmlElement("imsiExpression", typeof(WhenClauseImsiExpression))]
        [XmlElement("expressionGrouping", typeof(ProtocolWhenClauseCollection))]
        [XmlElement("linqXmlExpression", typeof(XmlLambdaExpression))]
        [XmlElement("linqExpression", typeof(String))]
        public List<object> Clause { get; set; }

        // Lock
        private Object m_lockObject = new object();

        /// <summary>
        /// Compile the expression
        /// </summary>
        public Expression Compile<TData>(Dictionary<String, Delegate> variableFunc)
        {
            ParameterExpression expressionParm = Expression.Parameter(typeof(TData), "_scope");
            Expression body = null;
            // Iterate and perform binary operations
            foreach (var itm in this.Clause)
            {

                Expression clauseExpr = null;
                if(itm is ProtocolWhenClauseCollection)
                {
                    clauseExpr = Expression.Invoke((itm as ProtocolWhenClauseCollection).Compile<TData>(variableFunc), expressionParm);
                }
                else if (itm is WhenClauseImsiExpression)
                {
                    var imsiExpr = itm as WhenClauseImsiExpression;
                    clauseExpr = Expression.Invoke(QueryExpressionParser.BuildLinqExpression<TData>(NameValueCollection.ParseQueryString(imsiExpr.Expression), variableFunc), expressionParm);
                    if (imsiExpr.NegationIndicator)
                        clauseExpr = Expression.Not(clauseExpr);
                    this.m_tracer.TraceVerbose("Converted WHEN {0} > {1}", imsiExpr.Expression, clauseExpr);
                }
                else if (itm is XmlLambdaExpression)
                {
                    var xmlLambda = itm as XmlLambdaExpression;
                    (itm as XmlLambdaExpression).InitializeContext(null);
                    // replace parameter
                    clauseExpr = Expression.Invoke(((itm as XmlLambdaExpression).ToExpression() as LambdaExpression), expressionParm);
                }
                else
                {
                    CompiledExpression<bool> exp = new CompiledExpression<bool>(itm as String);
                    exp.TypeRegistry = new TypeRegistry();
                    exp.TypeRegistry.RegisterDefaultTypes();
                    exp.TypeRegistry.RegisterType<TData>();
                    exp.TypeRegistry.RegisterType<Guid>();
                    exp.TypeRegistry.RegisterType<TimeSpan>();
                    exp.TypeRegistry.RegisterParameter("now", ()=>DateTime.Now); // because MONO is scumbag
                    foreach (var fn in variableFunc)
                        exp.TypeRegistry.RegisterParameter(fn.Key, fn.Value);
                    //exp.TypeRegistry.RegisterSymbol("data", expressionParm);
                    exp.ScopeCompile<TData>();
                    //Func<TData, bool> d = exp.ScopeCompile<TData>();
                    var linqAction = exp.GenerateLambda<Func<TData,bool>, TData>(true, false);
                    clauseExpr = Expression.Invoke(linqAction, expressionParm);
                    //clauseExpr = Expression.Invoke(d, expressionParm);
                }

                // Append to master expression
                if (body == null)
                    body = clauseExpr;
                else
                    body = Expression.MakeBinary((ExpressionType)Enum.Parse(typeof(ExpressionType), this.Operator.ToString()), body, clauseExpr);
            }

            // Wrap and compile
            var objParm = Expression.Parameter(typeof(Object));
            var bodyCondition = Expression.Lambda<Func<TData, bool>>(body, expressionParm);
            var invoke = Expression.Invoke(
                bodyCondition,
                Expression.Convert(objParm, typeof(TData))
            );
            var uncompiledExpression = Expression.Lambda<Func<Object, bool>>(invoke, objParm);
            this.m_compiledExpression = uncompiledExpression.Compile();
            return uncompiledExpression;
        }

        // Compiled
        private Func<Object, bool> m_compiledExpression = null;

        /// <summary>
        /// Evaluate the "when" clause
        /// </summary>
        public bool Evaluate<TData>(TData parm, Dictionary<String, Delegate> variableFunc)
        {

            if (this.m_compiledExpression == null)
                this.Compile<TData>(variableFunc);

            lock(m_lockObject)
                return this.m_compiledExpression.Invoke(parm);
        }
    }

    /// <summary>
    /// Represents a simple IMSI expression
    /// </summary>
    [XmlType(nameof(WhenClauseImsiExpression), Namespace = "http://openiz.org/cdss")]
    public class WhenClauseImsiExpression
    {

        /// <summary>
        /// Only when the data element DOES NOT match 
        /// </summary>
        [XmlAttribute("negationIndicator")]
        public bool NegationIndicator { get; set; }

        /// <summary>
        /// Represents the expression
        /// </summary>
        [XmlText]
        public String Expression { get; set; }

    }
}