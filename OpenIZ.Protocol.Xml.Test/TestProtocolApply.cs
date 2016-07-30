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
            Assert.AreEqual(4, acts.Count);


        }
    }
}
