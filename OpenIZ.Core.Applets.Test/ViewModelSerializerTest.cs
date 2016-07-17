using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using System.Collections.Generic;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Applets.ViewModel;
using System.IO;

namespace OpenIZ.Core.Applets.Test
{
    /// <summary>
    /// Represents a view model serialization test
    /// </summary>
    [TestClass]
    public class ViewModelSerializerTest
    {
        /// <summary>
        /// Test patient 
        /// </summary>
        private Patient m_patientUnderTest = new Patient()
        {
            Key = Guid.NewGuid(),
            VersionKey = Guid.NewGuid(),
            VersionSequence = 1,
            CreatedBy = new Core.Model.Security.SecurityUser()
            {
                Key = Guid.NewGuid(),
                UserName = "bob",
                SecurityHash = Guid.NewGuid().ToString(),
                Email = "bob@bob.com",
                InvalidLoginAttempts = 2, 
                UserClass = UserClassKeys.HumanUser
            },
            StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Johnson", "William")
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
                GenderConceptKey = Guid.Parse("f4e3a6bb-612e-46b2-9f77-ff844d971198"),
                DateOfBirth = new DateTime(1984, 03, 22),
                MultipleBirthOrder = 2,
                DeceasedDate = new DateTime(2016, 05, 02),
                DeceasedDatePrecision = DatePrecision.Day,
                DateOfBirthPrecision = DatePrecision.Day,
                CreationTime = DateTimeOffset.Now
            };

        /// <summary>
        /// Test serialization of the IMS patient object
        /// </summary>
        [TestMethod]
        public void TestSerializeComplexIMSObject()
        {

            String json = JsonViewModelSerializer.Serialize(this.m_patientUnderTest);
            Assert.IsNotNull(json);

        }

        /// <summary>
        /// Test de-serialization of the IMS patient object
        /// </summary>
        [TestMethod]
        public void TestDeSerializeComplexIMSObject()
        {

            using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.SimpleModel.json")))
            {
                var json = sr.ReadToEnd();
                var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
                Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, patient.ClassConceptKey);
            }
        }
    }
}
