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
using OpenIZ.Protocol.Xml.Model;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using System.Linq;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model;
using OpenIZ.Core.Protocol;
using OpenIZ.Core.Services;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenIZ.Core;
using OpenIZ.Core.Applets.ViewModel.Json;

namespace OpenIZ.Protocol.Xml.Test
{
    /// <summary>
    /// Tests the application of protocol
    /// </summary>
    [TestClass]
    public class TestProtocolApply : IServiceProvider
    {

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldScheduleOPV()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.OralPolioVaccine.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };
            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);

            Assert.AreEqual(4, acts.Count);


        }

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldScheduleBCG()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.BcgVaccine.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(1, acts.Count);
        }

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldRepeatWeight()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.Weight.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(60, acts.Count);
        }

        /// <summary>
        /// Test that the care plan schedules weight at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldSkipWeight()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.Weight.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" },
                Participations = new List<ActParticipation>()
                {
                    new ActParticipation()
                    {
                        ParticipationRole = new Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget" },
                        Act = new QuantityObservation()
                        {
                            Value = (decimal)3.2,
                            TypeConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "VitalSign-Weight" },
                            ActTime = DateTime.Now
                        }
                    },
                    new ActParticipation()
                    {
                        ParticipationRole = new Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget" },
                        Act = new PatientEncounter()
                        {
                            ActTime = DateTime.Now
                        }
                    }

                }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(59, acts.Count);
        }



        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldScheduleMR()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.MeaslesRubellaVaccine.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(2, acts.Count);
        }

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldSchedulePCV()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.PCV13Vaccine.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(3, acts.Count);
        }

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldScheduleDTP()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.DTP-HepB-HibTrivalent.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(3, acts.Count);
        }

        /// <summary>
        /// Test that the care plan schedules OPV0 at the correct time
        /// </summary>
        [TestMethod]
        public void TestShouldScheduleRota()
        {

            ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream("OpenIZ.Protocol.Xml.Test.Protocols.RotaVaccine.xml"));
            XmlClinicalProtocol xmlCp = new XmlClinicalProtocol(definition);

            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = xmlCp.Calculate(newborn, null);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(2, acts.Count);
        }


        /// <summary>
        /// Should schedule all vaccines
        /// </summary>
        [TestMethod]
        public void ShouldHandlePartials()
        {

            SimpleCarePlanService scp = new SimpleCarePlanService();
            ApplicationServiceContext.Current = this;
            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = scp.CreateCarePlan(newborn);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(83, acts.Count());
            Assert.IsFalse(acts.Any(o => o.Protocols.Count() > 1));
            acts = scp.CreateCarePlan(newborn);
            //Assert.AreEqual(60, acts.Count());
            newborn.Participations.RemoveAll(o => o.Act is QuantityObservation);
            Assert.AreEqual(23, newborn.Participations.Count);
            acts = scp.CreateCarePlan(newborn);
            //Assert.AreEqual(60, acts.Count());
            Assert.AreEqual(83, newborn.Participations.Count());
            Assert.IsFalse(acts.Any(o => !o.Participations.Any(p => p.ParticipationRoleKey == ActParticipationKey.RecordTarget)));
        }



        /// <summary>
        /// Should schedule all vaccines
        /// </summary>
        [TestMethod]
        public void ShouldExcludeAdults()
        {

            SimpleCarePlanService scp = new SimpleCarePlanService();
            ApplicationServiceContext.Current = this;
            // Patient that is just born = Schedule OPV
            Patient adult = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now.AddMonths(-240),
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };
            
            // Now apply the protocol
            var acts = scp.CreateCarePlan(adult);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(adult);
            Assert.AreEqual(0, acts.Count());
        }


        /// <summary>
        /// Should schedule all vaccines
        /// </summary>
        [TestMethod]
        public void ShouldScheduleAll()
        {

            SimpleCarePlanService scp = new SimpleCarePlanService();
            ApplicationServiceContext.Current = this;
            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = scp.CreateCarePlan(newborn);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(83, acts.Count());
            Assert.IsFalse(acts.Any(o => o.Protocols.Count() > 1));
        }

        /// <summary>
        /// Should group into appointments
        /// </summary>
        [TestMethod]
        public void ShouldScheduleAppointments()
        {
            SimpleCarePlanService scp = new SimpleCarePlanService();
            ApplicationServiceContext.Current = this;
            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = scp.CreateCarePlan(newborn, true);
            var jsonSerializer = new JsonViewModelSerializer();
			string json = jsonSerializer.Serialize(newborn);
            Assert.AreEqual(61, acts.Count());
            Assert.IsFalse(acts.Any(o => o.Protocols.Count() > 1));

        }

        /// <summary>
        /// Get service
        /// </summary>
        public object GetService(Type serviceType)
        {
            return new DummyProtocolRepository();
        }
    }

    /// <summary>
    /// Dummy clinical repository
    /// </summary>
    internal class DummyProtocolRepository : IClinicalProtocolRepositoryService
    {
        public IEnumerable<Core.Model.Acts.Protocol> FindProtocol(Expression<Func<Core.Model.Acts.Protocol, bool>> predicate, int offset, int? count, out int totalResults)
        {
            List<Core.Model.Acts.Protocol> retVal = new List<Core.Model.Acts.Protocol>();

            foreach(var i in typeof(DummyProtocolRepository).Assembly.GetManifestResourceNames())
                if(i.EndsWith(".xml"))
                {
                    ProtocolDefinition definition = ProtocolDefinition.Load(typeof(TestProtocolApply).Assembly.GetManifestResourceStream(i));
                    retVal.Add(new XmlClinicalProtocol(definition).GetProtcolData());
                }
            totalResults = retVal.Count;
            return retVal;
        }

        public Core.Model.Acts.Protocol InsertProtocol(Core.Model.Acts.Protocol data)
        {
            throw new NotImplementedException();
        }
    }
}
