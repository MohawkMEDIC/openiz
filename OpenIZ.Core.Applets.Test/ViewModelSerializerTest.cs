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
 * Date: 2016-7-6
 */
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
        /// Test parsing of a form submission
        /// </summary>
        [TestMethod]
        public void TestParseFormSubmission()
        {
            using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.RegistrationForm.json")))
            {
                var json = sr.ReadToEnd();
                var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
                json = JsonViewModelSerializer.Serialize(patient);
                Assert.IsNotNull(patient.Tags[0].TagKey);
                //Assert.AreEqual(this.m_patientUnderTest.ClassConceptKey, patient.ClassConceptKey);
            }
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

        /// <summary>
        /// Doesn't desearialize key for some reason
        /// </summary>
        [TestMethod]
        public void TestBugfixDoesntDeserializeKey()
        {
            using (var sr = new StreamReader(typeof(TestRenderApplets).Assembly.GetManifestResourceStream("OpenIZ.Core.Applets.Test.DoesntSerializeKey.json")))
            {
                var json = sr.ReadToEnd();
                var patient = JsonViewModelSerializer.DeSerialize<Patient>(json);
                Assert.IsNotNull(patient.Key);
            }

        }
    }
}
