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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
            Assert.AreEqual(60, acts.Count);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
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
            var acts = xmlCp.Calculate(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
            Assert.AreEqual(2, acts.Count);
        }


        /// <summary>
        /// Should schedule all vaccines
        /// </summary>
        [TestMethod]
        public void ShouldScheduleAll()
        {

            SimpleCarePlanService scp = new SimpleCarePlanService();
            ApplicationServiceContext.Current = this;
            scp.Initialize();
            // Patient that is just born = Schedule OPV
            Patient newborn = new Patient()
            {
                Key = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                GenderConcept = new Core.Model.DataTypes.Concept() { Mnemonic = "FEMALE" }
            };

            // Now apply the protocol
            var acts = scp.CreateCarePlan(newborn);
            String json = JsonViewModelSerializer.Serialize(newborn);
            Assert.AreEqual(83, acts.Count());
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
