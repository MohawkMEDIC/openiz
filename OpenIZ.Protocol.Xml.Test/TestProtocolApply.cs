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

namespace OpenIZ.Protocol.Xml.Test
{
    /// <summary>
    /// Tests the application of protocol
    /// </summary>
    [TestClass]
    public class TestProtocolApply
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
            Assert.AreEqual(1, acts.Count);
            Assert.IsInstanceOfType(acts[0], typeof(SubstanceAdministration));
            Assert.AreEqual(ActMoodKeys.Propose, acts[0].MoodConceptKey);
            Assert.AreEqual(1, acts[0].Protocols.Count);
            Assert.AreEqual(definition.Uuid, acts[0].Protocols[0].ProtocolKey);
            Assert.AreEqual("IMMUNIZ", acts[0].TypeConcept.Mnemonic);
            Assert.AreEqual(1, acts[0].Participations.Where(o => o.ParticipationRoleKey == ActParticipationKey.Product).Count());
            Assert.AreEqual("ANTIGEN", acts[0].Participations.First(o => o.ParticipationRoleKey == ActParticipationKey.Product).PlayerEntity.TypeConcept.Mnemonic);


        }
    }
}
