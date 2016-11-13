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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Diagnostics;
using System.IO;
using OpenIZ.Core.Model.Constants;
using System.Threading;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;

namespace OpenIZ.Protocol.Xml
{
    /// <summary>
    /// Clinicl protocol that is stored/loaded via XML
    /// </summary>
    public class XmlClinicalProtocol : IClinicalProtocol
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(XmlClinicalProtocol));
        
        /// <summary>
        /// Default ctor
        /// </summary>
        public XmlClinicalProtocol()
        {

        }

        /// <summary>
        /// Creates a new protocol from the specified definition
        /// </summary>
        /// <param name="defn"></param>
        public XmlClinicalProtocol(ProtocolDefinition defn)
        {
            this.Definition = defn;
        }

        /// <summary>
        /// Gets or sets the definition of the protocol
        /// </summary>
        public ProtocolDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the id of the protocol
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.Definition.Uuid;
            }
        }

        /// <summary>
        /// Gets or sets the name of the protocol
        /// </summary>
        public string Name
        {
            get
            {
                return this.Definition?.Name;
            }
        }

        /// <summary>
        /// XmlClinicalProtocol
        /// </summary>
        static XmlClinicalProtocol()
        {
            Func<Int32> expr = () => { return (int)s_variables["index"]; };
            s_callbacks.Add("index", expr);
        }

        /// <summary>
        /// Local index
        /// </summary>
        [ThreadStatic]
        private static Dictionary<String, Object> s_variables = null;

        // Callbacks
        private static Dictionary<String, Delegate> s_callbacks = new Dictionary<string, Delegate>();

        /// <summary>
        /// Calculate the protocol against a atient
        /// </summary>
        public List<Act> Calculate(Patient triggerPatient)
        {

            try
            {
#if DEBUG
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif

                // Get a clone to make decisions on
                Patient patient = null;
                lock (triggerPatient)
                    patient = triggerPatient.Clone() as Patient;
                patient.Participations = new List<ActParticipation>(triggerPatient.Participations);

                this.m_tracer.TraceInfo("Calculate ({0}) for {1}...", this.Name, patient);

                s_variables = new Dictionary<string, object>() { { "index", 0 } };

                // Evaluate eligibility
                if (this.Definition.When?.Evaluate(patient, s_callbacks) == false)
                {
                    this.m_tracer.TraceInfo("{0} does not meet criteria for {1}", patient, this.Id);
                    return new List<Act>();
                }

                List<Act> retVal = new List<Act>();

                // Rules
                foreach (var rule in this.Definition.Rules)
                    for (var index = 0; index < rule.Repeat; index++)
                    {

                        s_variables["index"] = index;
                        foreach (var itm in rule.Variables)
                        {
                            if (!s_variables.ContainsKey(itm.VariableName))
                                s_variables.Add(itm.VariableName, itm.GetValue(null, patient, s_callbacks));
                            else
                                s_variables[itm.VariableName] = itm.GetValue(null, patient, s_callbacks);
                            if (!s_callbacks.ContainsKey(itm.VariableName))
                            {
                                Func<Object> funcBody = () =>
                                {
                                    return s_variables[itm.VariableName];
                                };

                                var varType = s_variables[itm.VariableName].GetType();
                                Delegate func = Expression.Lambda(typeof(Func<>).MakeGenericType(varType), Expression.Convert(Expression.Call(funcBody.Target == null ? null : Expression.Constant(funcBody.Target), funcBody.GetMethodInfo()), varType)).Compile();
                                s_callbacks.Add(itm.VariableName, func);
                            }
                        }

                        // TODO: Variable initialization 
                        if (rule.When.Evaluate(patient, s_callbacks))
                        {
                            var acts = rule.Then.Evaluate(patient, s_callbacks);
                            retVal.AddRange(acts);
                        }
                        else
                            this.m_tracer.TraceInfo("{0} does not meet criteria for rule {1}.{2}", patient, this.Name, rule.Name);

                    }

                // Assign protocol
                foreach (var itm in retVal)
                    itm.Protocols.Add(new ActProtocol()
                    {
                        ProtocolKey = this.Id
                    });

                // Now we want to add the stuff to the patient
                lock (triggerPatient)
                    triggerPatient.Participations.AddRange(retVal.Where(o=>o != null).Select(o=>new ActParticipation(ActParticipationKey.RecordTarget, triggerPatient) { Act = o, ParticipationRole = new Core.Model.DataTypes.Concept() { Key = ActParticipationKey.RecordTarget, Mnemonic = "RecordTarget" }, Key = Guid.NewGuid() }));
#if DEBUG
                sw.Stop();
                this.m_tracer.TraceVerbose("Protocol {0} took {1} ms", this.Name, sw.ElapsedMilliseconds);
#endif

                return retVal;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error applying protocol {0}: {1}", this.Name, e);
                return new List<Act>();
            }
        }

        /// <summary>
        /// Get protocol data
        /// </summary>
        public Core.Model.Acts.Protocol GetProtcolData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                this.Definition.Save(ms);
                return new Core.Model.Acts.Protocol()
                {
                    HandlerClass = this.GetType(),
                    Name = this.Name,
                    Definition = ms.ToArray(),
                    Key = this.Id
                };
            }
        }

        /// <summary>
        /// Create the protocol data from the protocol instance
        /// </summary>
        public void Load(Core.Model.Acts.Protocol protocolData)
        {
            if (protocolData == null) throw new ArgumentNullException(nameof(protocolData));
            using (MemoryStream ms = new MemoryStream(protocolData.Definition))
                this.Definition = ProtocolDefinition.Load(ms);

        }

        /// <summary>
        /// Updates an existing plan
        /// </summary>
        public List<Act> Update(Patient p, List<Act> existingPlan)
        {
            throw new NotImplementedException();
        }
    }
}
