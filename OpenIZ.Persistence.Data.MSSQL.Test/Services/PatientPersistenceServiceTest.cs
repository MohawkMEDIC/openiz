using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Test class for patient persistence
    /// </summary>
    [TestClass]
    public class PatientPersistenceServiceTest : PersistenceTest<Patient>
    {
        private static IPrincipal s_authorization;
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
            s_authorization = AuthenticationContext.SystemPrincipal;

        }
        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestPersistPatient()
        {

            Patient p = new Patient()
            {
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "Johnson", "William", "P.", "Bear")
                },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.HomeAddress, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(new AssigningAuthority() { Name = "OHIPCARD", DomainName = "OHIPCARD", Oid = "1.2.3.4.5.6" }, "12343120423")
                },
                Telecoms = new List<EntityTelecomAddress>()
                {
                    new EntityTelecomAddress(AddressUseKeys.WorkPlace, "mailto:will@johnson.com")
                },
                Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "true")
                },
                Notes = new List<EntityNote>()
                {
                    new EntityNote(Guid.Empty, "William is a test patient")
                    {
                        Author = new Person()
                    }
                },
                Extensions = new List<EntityExtension>() {
                    new EntityExtension()
                    {
                        ExtensionType = new ExtensionType()
                        {
                            Name = "http://openiz.org/oiz/birthcertificate",
                            ExtensionHandler = typeof(EntityPersistenceServiceTest)
                        },
                        ExtensionValue = new byte[] { 1 }
                    }
                },
                GenderConceptKey = Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198"),
                DateOfBirth = new DateTime(1984, 03, 22),
                MultipleBirthOrder = 2,
                DeceasedDate = new DateTime(2016,05,02),
                DeceasedDatePrecision = DatePrecision.Day,
                DateOfBirthPrecision = DatePrecision.Day
            };

            Person mother = new Person()
            {
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Johnson", "Martha")
                },
            };

            // Associate: PARENT > CHILD
            p.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.Mother, mother));

            var afterInsert = base.DoTestInsert(p, s_authorization);
            Assert.AreEqual(DatePrecision.Day, afterInsert.DateOfBirthPrecision);
            Assert.AreEqual(DatePrecision.Day, afterInsert.DeceasedDatePrecision);
            Assert.AreEqual(new DateTime(1984, 03, 22), afterInsert.DateOfBirth);
            Assert.AreEqual(new DateTime(2016, 05, 02), afterInsert.DeceasedDate);
            Assert.AreEqual("Male", afterInsert.GenderConcept.Mnemonic);
            Assert.AreEqual(2, afterInsert.MultipleBirthOrder);
            Assert.AreEqual(1, p.Names.Count);
            Assert.AreEqual(1, p.Addresses.Count);
            Assert.AreEqual(1, p.Identifiers.Count);
            Assert.AreEqual(1, p.Telecoms.Count);
            Assert.AreEqual(1, p.Tags.Count);
            Assert.AreEqual(1, p.Notes.Count);
            Assert.AreEqual(EntityClassKeys.Patient, p.ClassConceptKey);
            Assert.AreEqual(DeterminerKeys.Specific, p.DeterminerConceptKey);
            Assert.AreEqual(StatusKeys.Active, p.StatusConceptKey);

            // Test serialization for model
            afterInsert.SetDelayLoad(false);
            var testBundle = Bundle.CreateBundle(afterInsert);

            // Simulate receive
            String json = JsonConvert.SerializeObject(testBundle, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            });
            testBundle = JsonConvert.DeserializeObject<Bundle>(json, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            });
            testBundle.Reconstitute();
            ViewModelSerializer vms = new ViewModelSerializer();
            var jsonSimple = vms.Serialize(testBundle.Entry);
            Assert.IsNotNull(json);
        }
    }
}
