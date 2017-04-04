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
using ExpressionEvaluator;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;

namespace OpenIZ.Protocol.Xml.Model
{
    /// <summary>
    /// Reperesents a then condition clause
    /// </summary>
    [XmlType(nameof(ProtocolThenClauseCollection), Namespace = "http://openiz.org/cdss")]
    public class ProtocolThenClauseCollection : DecisionSupportBaseElement
    {

        // JSON Serializer
        private static JsonViewModelSerializer s_serializer = new JsonViewModelSerializer();

        /// <summary>
        /// Actions to be performed
        /// </summary>
        [XmlElement("action", Type = typeof(ProtocolDataAction))]
        public List<ProtocolDataAction> Action { get; set; }

        /// <summary>
        /// Evaluate the actions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Act> Evaluate(Patient p, Dictionary<String, Delegate> variableFunc)
        {
            List<Act> retVal = new List<Act>();
            Dictionary<String, Object> calculatedScopes = new Dictionary<string, object>()
            {
                { ".", p }
            };

            foreach (var itm in this.Action)
            {

                Act act = null;
                if (itm.Element is String) // JSON
                {
                    itm.Element = s_serializer.DeSerialize<Act>(itm.Element as String);
                    // Load all concepts for the specified objects 
                }
                act = (itm.Element as Act).Clone() as Act;
                act.Participations = new List<ActParticipation>((itm.Element as Act).Participations.Select(o=>o.Clone() as ActParticipation));
                act.Relationships = new List<ActRelationship>((itm.Element as Act).Relationships.Select(o => o.Clone() as ActRelationship));
                act.Protocols = new List<ActProtocol>();// (itm.Element as Act).Protocols);
                // Now do the actions to the properties as stated
                foreach (var instr in itm.Do)
                {
                    instr.Evaluate(act, p, variableFunc, calculatedScopes);
                }

                // Assign this patient as the record target
                act.Key = act.Key ?? Guid.NewGuid();
                Guid pkey = Guid.NewGuid();
                act.Participations.Add(new ActParticipation(ActParticipationKey.RecordTarget, p.Key) { ParticipationRole = new Core.Model.DataTypes.Concept() { Key = ActParticipationKey.RecordTarget, Mnemonic = "RecordTarget" }, Key = pkey });
                // Add record target to the source for forward rules
                p.Participations.Add(new ActParticipation(ActParticipationKey.RecordTarget, p) { SourceEntity = act, ParticipationRole = new Core.Model.DataTypes.Concept() { Key = ActParticipationKey.RecordTarget, Mnemonic = "RecordTarget" }, Key = pkey });
                act.CreationTime = DateTime.Now;
                // The act to the return value
                retVal.Add(act);

            }

            return retVal;
        }
    }

    /// <summary>
    /// Asset data action base
    /// </summary>
    [XmlType(nameof(ProtocolDataAction), Namespace = "http://openiz.org/cdss")]
    public class ProtocolDataAction
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ProtocolDataAction()
        {
        }

        /// <summary>
        /// Gets the elements to be performed
        /// </summary>
        [XmlElement("Act", typeof(Act), Namespace = "http://openiz.org/model")]
        [XmlElement("TextObservation", typeof(TextObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("SubstanceAdministration", typeof(SubstanceAdministration), Namespace = "http://openiz.org/model")]
        [XmlElement("QuantityObservation", typeof(QuantityObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("CodedObservation", typeof(CodedObservation), Namespace = "http://openiz.org/model")]
        [XmlElement("PatientEncounter", typeof(PatientEncounter), Namespace = "http://openiz.org/model")]
        [XmlElement("jsonModel", typeof(String))]
        public Object Element { get; set; }

        /// <summary>
        /// Associate the specified data for stuff that cannot be serialized
        /// </summary>
        [XmlElement("assign", typeof(PropertyAssignAction))]
        [XmlElement("add", typeof(PropertyAddAction))]
        public List<PropertyAction> Do { get; set; }
    }

    /// <summary>
    /// Associate data
    /// </summary>
    [XmlType(nameof(PropertyAction), Namespace = "http://openiz.org/cdss")]
    public abstract class PropertyAction : ProtocolDataAction
    {
        /// <summary>
        /// Action name
        /// </summary>
        public abstract String ActionName { get; }

        /// <summary>
        /// The name of the property
        /// </summary>
        [XmlAttribute("propertyName")]
        public String PropertyName { get; set; }

        /// <summary>
        /// Evaluate the expression
        /// </summary>
        /// <returns></returns>
        public abstract object Evaluate(Act act, Patient recordTarget, Dictionary<String, Delegate> variableFunc, IDictionary<String, Object> scopes);
    }

    /// <summary>
    /// Property assign value
    /// </summary>
    [XmlType(nameof(PropertyAssignAction), Namespace = "http://openiz.org/cdss")]
    public class PropertyAssignAction : PropertyAction
    {
        // The setter action
        private Delegate m_setter;
        // Select method
        private MethodInfo m_scopeSelectMethod;
        // Linq expression to select scope
        private Expression m_linqExpression;

        /// <summary>
        /// Action name
        /// </summary>
        public override string ActionName
        {
            get
            {
                return "SetValue";
            }
        }

        /// <summary>
        /// Selection of scope
        /// </summary>
        [XmlAttribute("scope")]
        public String ScopeSelector { get; set; }

        /// <summary>
        /// Where filter for scope
        /// </summary>
        [XmlAttribute("where")]
        public String WhereFilter { get; set; }

        /// <summary>
        /// Value expression
        /// </summary>
        [XmlText()]
        public String ValueExpression { get; set; }

        /// <summary>
        /// Get the specified value
        /// </summary>
        public object GetValue(Act act, Patient recordTarget, Dictionary<String, Delegate> variableFunc, IDictionary<String, Object> scopes)
        {
            if (this.m_setter == null)
            {

                CompiledExpression exp = new CompiledExpression(this.ValueExpression);
                exp.TypeRegistry = new TypeRegistry();
                exp.TypeRegistry.RegisterDefaultTypes();
                exp.TypeRegistry.RegisterType<Guid>();
                exp.TypeRegistry.RegisterType<DateTimeOffset>();
                exp.TypeRegistry.RegisterType<TimeSpan>();
                exp.TypeRegistry.RegisterParameter("now", () => DateTime.Now);
                foreach (var itm in variableFunc)
                    exp.TypeRegistry.RegisterParameter(itm.Key, itm.Value);

                // Scope
                if (!String.IsNullOrEmpty(this.ScopeSelector))
                {

                    var scopeProperty = recordTarget.GetType().GetRuntimeProperty(this.ScopeSelector);
                    var scopeValue = scopeProperty.GetValue(recordTarget);

                    if (scopeProperty == null) return null; // no scope

                    // Where clause?
                    if (!String.IsNullOrEmpty(this.WhereFilter))
                    {
                        var itemType = scopeProperty.PropertyType.GetTypeInfo().GenericTypeArguments[0];
                        var predicateType = typeof(Func<,>).MakeGenericType(new Type[] { itemType, typeof(bool) });
                        var builderMethod = typeof(QueryExpressionParser).GetGenericMethod(nameof(QueryExpressionParser.BuildLinqExpression), new Type[] { itemType }, new Type[] { typeof(NameValueCollection) });
                        this.m_linqExpression = builderMethod.Invoke(null, new Object[] { NameValueCollection.ParseQueryString(this.WhereFilter) }) as Expression;

                        // Call where clause
                        builderMethod = typeof(Expression).GetGenericMethod(nameof(Expression.Lambda), new Type[] { predicateType }, new Type[] { typeof(Expression), typeof(ParameterExpression[]) });
                        var firstMethod = typeof(Enumerable).GetGenericMethod("FirstOrDefault",
                               new Type[] { itemType },
                               new Type[] { scopeProperty.PropertyType, predicateType });

                        this.m_scopeSelectMethod = (MethodInfo)firstMethod;

                    }
                    exp.TypeRegistry.RegisterType(this.m_scopeSelectMethod.ReturnType.Name, this.m_scopeSelectMethod.ReturnType);

                    var compileMethod = typeof(CompiledExpression).GetGenericMethod(nameof(CompiledExpression.ScopeCompile), new Type[] { this.m_scopeSelectMethod.ReturnType }, new Type[] { });
                    this.m_setter = (compileMethod.Invoke(exp, null) as Delegate);
                }
                else
                    this.m_setter = exp.ScopeCompile<Patient>();
            }

            Object setValue = null;
            // Where clause?
            if (!String.IsNullOrEmpty(this.ScopeSelector))
            {
                // Is the scope selector key present?
                String scopeKey = $"{this.ScopeSelector}.{this.WhereFilter}";
                object scope = null;
                if (!scopes.TryGetValue(scopeKey, out scope))
                {
                    var scopeProperty = recordTarget.GetType().GetRuntimeProperty(this.ScopeSelector);
                    var scopeValue = scopeProperty.GetValue(recordTarget);
                    scope = scopeValue;
                    if (!String.IsNullOrEmpty(this.WhereFilter))
                        scope = this.m_scopeSelectMethod.Invoke(null, new Object[] { scopeValue, (this.m_linqExpression as LambdaExpression).Compile() });
                    lock (scopes)
                        if(!scopes.ContainsKey(scopeKey))
                            scopes.Add(scopeKey, scope);
                }
                setValue = this.m_setter.DynamicInvoke(scope);
            }
            else
                setValue = this.m_setter.DynamicInvoke(recordTarget);

            return setValue;
        }

        /// <summary>
        /// Evaluate the specified action on the object
        /// </summary>
        public override object Evaluate(Act act, Patient recordTarget, Dictionary<String, Delegate> variableFunc, IDictionary<String, Object> scopes)
        {

            var propertyInfo = act.GetType().GetRuntimeProperty(this.PropertyName);

            if (this.Element != null)
                propertyInfo.SetValue(act, this.Element);
            else
            {
                var setValue = this.GetValue(act, recordTarget, variableFunc, scopes);

                //exp.TypeRegistry.RegisterSymbol("data", expressionParm);
                if (Core.Model.Map.MapUtil.TryConvert(setValue, propertyInfo.PropertyType, out setValue))
                    propertyInfo.SetValue(act, setValue);

            }

            return propertyInfo.GetValue(act);
        }

    }

    /// <summary>
    /// Add something to a property collection
    /// </summary>
    [XmlType(nameof(PropertyAddAction), Namespace = "http://openiz.org/cdss")]
    public class PropertyAddAction : PropertyAction
    {

        /// <summary>
        /// Evaluate
        /// </summary>
        public override object Evaluate(Act act, Patient recordTarget, Dictionary<String, Delegate> variableFunc, IDictionary<String, Object> scopes)
        {
            var value = act.GetType().GetRuntimeProperty(this.PropertyName) as IList;
            value?.Add(this.Element);
            return value;
        }

        /// <summary>
        /// Add action
        /// </summary>
        public override string ActionName
        {
            get
            {
                return "Add";
            }
        }
    }

}