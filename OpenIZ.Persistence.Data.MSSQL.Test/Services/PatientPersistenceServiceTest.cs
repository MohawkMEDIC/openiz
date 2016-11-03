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
 * Date: 2016-6-28
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Core.Applets.ViewModel.Json;
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
                    new EntityIdentifier(new AssigningAuthority() { Name = "OHIPCARD12", DomainName = "OHIPCARD12", Oid = "1.2.3.4.5.6" }, "12343120423")
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
                        ExtensionValueXml = new byte[] { 1 }
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
                }
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

            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(afterInsert);
            Assert.IsNotNull(json);

        }

        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestShouldAdhereToClassifierCodes()
        {
            AssigningAuthority aa = new AssigningAuthority()
            {
                Name = "Ontario Health Insurance Card",
                DomainName = "OHIPCARD",
                Oid = "1.2.3.4.5.67"
            };
            var aaPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
            var ohipAuth = aaPersistence.Insert(aa, s_authorization, TransactionMode.Commit);

            Patient p = new Patient()
            {
                StatusConcept = new Concept()
                {
                    Mnemonic = "ACTIVE"
                },
                Names = new List<EntityName>()
                {
                    new EntityName() {
                        NameUse = new Concept() { Mnemonic = "OfficialRecord" },
                        Component = new List<EntityNameComponent>() {
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Family" },
                                Value = "Johnson"
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "William"
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "P."
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "Bear"
                            }
                        }
                    }
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(
                        new AssigningAuthority() { DomainName = "OHIPCARD" }, "12343120423")
                },
                Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "true")
                },
                Extensions = new List<EntityExtension>() {
                    new EntityExtension()
                    {
                        ExtensionType = new ExtensionType()
                        {
                            Name = "http://openiz.org/oiz/birthcertificate",
                            ExtensionHandler = typeof(EntityPersistenceServiceTest)
                        },
                        ExtensionValueXml = new byte[] { 1 }
                    }
                },
                GenderConcept = new Concept() {  Mnemonic = "Male" },
                DateOfBirth = new DateTime(1984, 03, 22),
                MultipleBirthOrder = 2,
                DeceasedDate = new DateTime(2016, 05, 02),
                DeceasedDatePrecision = DatePrecision.Day,
                DateOfBirthPrecision = DatePrecision.Day
            };

            var afterInsert = base.DoTestInsert(p, s_authorization);
            Assert.AreEqual("Male", afterInsert.GenderConcept.Mnemonic);
            Assert.AreEqual(EntityClassKeys.Patient, p.ClassConceptKey);
            Assert.AreEqual(DeterminerKeys.Specific, p.DeterminerConceptKey);
            Assert.AreEqual(StatusKeys.Active, p.StatusConceptKey);
            Assert.AreEqual(aa.Key, afterInsert.Identifiers[0].AuthorityKey);
            var jsonSerializer = new JsonViewModelSerializer();
            String json = jsonSerializer.Serialize(afterInsert);

            Assert.IsNotNull(json);

        }

        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestQueryByGender()
        {
            Patient p = new Patient()
            {
                StatusConcept = new Concept()
                {
                    Mnemonic = "ACTIVE"
                },
                Names = new List<EntityName>()
                {
                    new EntityName() {
                        NameUse = new Concept() { Mnemonic = "OfficialRecord" },
                        Component = new List<EntityNameComponent>() {
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "Allison"
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "P."
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Family" },
                                Value = "Bear"
                            }
                        }
                    }
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(
                        new AssigningAuthority() { DomainName = "OHIPCARD" }, "3242342323")
                },
                Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "false")
                },
                Extensions = new List<EntityExtension>() {
                    new EntityExtension()
                    {
                        ExtensionType = new ExtensionType()
                        {
                            Name = "http://openiz.org/oiz/birthcertificate",
                            ExtensionHandler = typeof(EntityPersistenceServiceTest)
                        },
                        ExtensionValueXml = BitConverter.GetBytes(true)
                    }
                },
                GenderConcept = new Concept() { Mnemonic = "Female" },
                DateOfBirth = new DateTime(1990, 03, 22),
                MultipleBirthOrder = 2,
                DateOfBirthPrecision = DatePrecision.Day
            };

            var afterInsert = base.DoTestInsert(p, s_authorization);

            var result = base.DoTestQuery(o => o.GenderConcept.Mnemonic == "Female", afterInsert.Key, s_authorization);
            
        }

        /// <summary>
        /// Test the persistence of a person
        /// </summary>
        [TestMethod]
        public void TestQueryByCreationDate()
        {
            Patient p = new Patient()
            {
                StatusConcept = new Concept()
                {
                    Mnemonic = "ACTIVE"
                },
                Names = new List<EntityName>()
                {
                    new EntityName() {
                        NameUse = new Concept() { Mnemonic = "OfficialRecord" },
                        Component = new List<EntityNameComponent>() {
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "Jamie"
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Given" },
                                Value = "A."
                            },
                            new EntityNameComponent() {
                                ComponentType = new Concept() { Mnemonic = "Family" },
                                Value = "Bear"
                            }
                        }
                    }
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(
                        new AssigningAuthority() { Name="OHIP", Oid="1.2.3.4.5.6", DomainName = "OHIPCARD" }, "43234453453")
                },
                Tags = new List<EntityTag>()
                {
                    new EntityTag("hasBirthCertificate", "false")
                },
                Extensions = new List<EntityExtension>() {
                    new EntityExtension()
                    {
                        ExtensionType = new ExtensionType()
                        {
                            Name = "http://openiz.org/oiz/birthcertificate",
                            ExtensionHandler = typeof(EntityPersistenceServiceTest)
                        },
                        ExtensionValueXml = BitConverter.GetBytes(true)
                    }
                },
                GenderConcept = new Concept() { Mnemonic = "Female" },
                DateOfBirth = new DateTime(1999, 07, 22),
                DateOfBirthPrecision = DatePrecision.Day
            };

            var afterInsert = base.DoTestInsert(p, s_authorization);

            var result = base.DoTestQuery(o => o.CreationTime < DateTimeOffset.Now.AddDays(2), afterInsert.Key, s_authorization);

        }
    }
}
