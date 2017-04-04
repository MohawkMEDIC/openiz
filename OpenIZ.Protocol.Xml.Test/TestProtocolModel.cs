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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Protocol.Xml.Model.XmlLinq;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Linq;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Acts;
using System.Collections.Generic;

namespace OpenIZ.Protocol.Xml.Test
{
    /// <summary>
    /// Tests which ensure the protocol model can be loaded and serialized properly
    /// </summary>
    [TestClass]
    public class TestProtocolModel
    {

        /// <summary>
        /// Represent an expression as a xml string
        /// </summary>
        private String ToXmlString(XmlExpression expr)
        {

            XmlSerializer xsz = new XmlSerializer(expr.GetType());
            using (StringWriter sw = new StringWriter())
            {
                xsz.Serialize(sw, expr);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Represent an expression as a xml string
        /// </summary>
        private XmlExpression FromXmlString(String xml, Type deserType)
        {

            XmlSerializer xsz = new XmlSerializer(deserType);
            using (StringReader sr = new StringReader(xml))
            {
                return xsz.Deserialize(sr) as XmlExpression;
            }
        }
        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestSerializeLambda()
        {

            Expression<Func<Patient, bool>> filterGender = (o) => o.GenderConcept.Mnemonic == "Male";
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterGender);
            Assert.IsInstanceOfType(xmlExpr, typeof(XmlLambdaExpression));
            Assert.AreEqual(typeof(bool), xmlExpr.Type);
            Assert.AreEqual("o", (xmlExpr as XmlLambdaExpression).Parameters[0].ParameterName);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            var expression = parsed.ToExpression();
        }

        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestSerializeWhereCondition()
        {

            Expression<Func<Patient, bool>> filterGender = (o) => o.Names.Where(g => g.NameUseKey == NameUseKeys.OfficialRecord).Any(n => n.Component.Where(g => g.ComponentTypeKey == NameComponentKeys.Family).Any(c => c.Value == "Smith"));
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterGender);
            Assert.IsInstanceOfType(xmlExpr, typeof(XmlLambdaExpression));
            Assert.AreEqual(typeof(bool), xmlExpr.Type);
            Assert.AreEqual("o", (xmlExpr as XmlLambdaExpression).Parameters[0].ParameterName);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            var expression = parsed.ToExpression();
        }

        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestGuardMinimumAgeCondition()
        {

            Expression<Func<Patient, bool>> filterAge = (data) => DateTime.Now.Subtract(data.DateOfBirth.Value).TotalDays >= 42;
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterAge);
            Assert.IsInstanceOfType(xmlExpr, typeof(XmlLambdaExpression));
            Assert.AreEqual(typeof(bool), xmlExpr.Type);
            Assert.AreEqual("data", (xmlExpr as XmlLambdaExpression).Parameters[0].ParameterName);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            var expression = parsed.ToExpression();
        }

        /// <summary>
        /// Test guard condition generation for polio 0
        /// </summary>
        [TestMethod]
        public void TestGuardNoPolioDose0()
        {
            Expression<Func<Patient, bool>> filterCondition = (data) => !data.Participations.Where(guard => guard.ParticipationRoleKey == ActParticipationKey.RecordTarget).Any(o => o.SourceEntity is SubstanceAdministration && (o.SourceEntity as SubstanceAdministration).SequenceId == 0 && o.SourceEntity.Participations.Any(p => p.PlayerEntity.TypeConcept.Mnemonic == "VaccineType-OralPolioVaccine"));
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterCondition);
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            parsed.InitializeContext(null);
            var expression = parsed.ToExpression();

            var compile = (expression as LambdaExpression).Compile();
        }
        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestGuardNotImmunoSuppressed()
        {

            Expression<Func<Patient, bool>> filterCondition = (data) => data.Participations.Where(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget).Any(o => o.SourceEntity is Observation && !o.SourceEntity.IsNegated && o.SourceEntity.TypeConcept.Mnemonic == "Diagnosis" && (o.SourceEntity as CodedObservation).Value.ConceptSets.Any(s => s.Mnemonic == "ImmunoSuppressionDiseases"));
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterCondition);
            Assert.IsInstanceOfType(xmlExpr, typeof(XmlLambdaExpression));
            Assert.AreEqual(typeof(bool), xmlExpr.Type);
            Assert.AreEqual("data", (xmlExpr as XmlLambdaExpression).Parameters[0].ParameterName);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            parsed.InitializeContext(null);
            var expression = parsed.ToExpression();

            var compile = (expression as LambdaExpression).Compile();

        }

        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestPropertySelector()
        {

            Expression<Func<Patient, DateTime?>> filterCondition = (data) => data.DateOfBirth;
            
            XmlExpression xmlExpr = XmlExpression.FromExpression(filterCondition);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            parsed.InitializeContext(null);
            var expression = parsed.ToExpression();
            
            var compile = (expression as LambdaExpression).Compile();
        }

        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestPropertyAddDays()
        {

            Expression<Func<Patient, DateTime?>> filterCondition = (data) => data.DateOfBirth.Value.AddDays(7);

            XmlExpression xmlExpr = XmlExpression.FromExpression(filterCondition);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            parsed.InitializeContext(null);
            var expression = parsed.ToExpression();

            var compile = (expression as LambdaExpression).Compile();

        }
        /// <summary>
        /// Tests that the model can serialize a simple lambda expression
        /// </summary>
        [TestMethod]
        public void TestAgeSelector()
        {

            Expression<Func<Patient, double>> filterCondition = (data) => DateTime.Now.Subtract(data.DateOfBirth.Value).TotalDays;

            XmlExpression xmlExpr = XmlExpression.FromExpression(filterCondition);
            // Serialize
            var xml = this.ToXmlString(xmlExpr);
            Trace.TraceInformation(xml);

            var parsed = this.FromXmlString(xml, typeof(XmlLambdaExpression));
            parsed.InitializeContext(null);
            var expression = parsed.ToExpression();

            var compile = (expression as LambdaExpression).Compile();
        }
    }
}
