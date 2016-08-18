using ExpressionEvaluator;
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
    [XmlType(nameof(ProtocolWhenClauseCollection), Namespace = "http://openiz.org/protocol")]
    public class ProtocolWhenClauseCollection
    {

        /// <summary>
        /// Operator 
        /// </summary>
        [XmlAttribute("evaluation")]
        public BinaryOperatorType Operator { get; set; }

        /// <summary>
        /// Clause evelators
        /// </summary>
        [XmlElement("imsiExpression", typeof(WhenClauseImsiExpression))]
        [XmlElement("linqXmlExpression", typeof(XmlLambdaExpression))]
        [XmlElement("linqExpression", typeof(String))]
        public List<object> Clause { get; set; }

        // Lock
        private Object m_lockObject = new object();

        /// <summary>
        /// Compile the expression
        /// </summary>
        public void Compile<TData>()
        {
            Expression<Func<TData, bool>> expression = null;
            ParameterExpression expressionParm = Expression.Parameter(typeof(TData), "_scope");
            Expression body = null;
            // Iterate and perform binary operations
            foreach (var itm in this.Clause)
            {

                Expression clauseExpr = null;
                if (itm is WhenClauseImsiExpression)
                {
                    var imsiExpr = itm as WhenClauseImsiExpression;
                    clauseExpr = Expression.Invoke(QueryExpressionParser.BuildLinqExpression<TData>(NameValueCollection.ParseQueryString(imsiExpr.Expression)), expressionParm);
                    if (imsiExpr.NegationIndicator)
                        clauseExpr = Expression.Not(clauseExpr);
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
            this.m_compiledExpression = Expression.Lambda<Func<Object, bool>>(invoke, objParm).Compile();
        }

        // Compiled
        private Func<Object, bool> m_compiledExpression = null;

        /// <summary>
        /// Evaluate the "when" clause
        /// </summary>
        public bool Evaluate<TData>(TData parm)
        {

            if (this.m_compiledExpression == null)
                this.Compile<TData>();

            lock(this.m_lockObject)
                return this.m_compiledExpression.Invoke(parm);
        }
    }

    /// <summary>
    /// Represents a simple IMSI expression
    /// </summary>
    [XmlType(nameof(WhenClauseImsiExpression), Namespace = "http://openiz.org/protocol")]
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