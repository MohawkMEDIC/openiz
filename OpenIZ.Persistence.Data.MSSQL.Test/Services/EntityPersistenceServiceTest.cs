using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
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
    /// Test persistence of entities
    /// </summary>
    [TestClass]
    public class EntityPersistenceServiceTest : PersistenceTest<Entity>
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
        /// Test the insertion of an anonymous entity
        /// </summary>
        [TestMethod]
        public void TestInsertAnonymousEntity()
        {
            Entity strawberry = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Described,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConcept = new Concept()
                {
                    Key = Guid.NewGuid(), // Some random concept for the "Type",
                    Mnemonic = "TEST_CHAIN_INSERT",
                    CreatedByKey = Guid.Empty,
                    IsSystemConcept = false,
                    StatusConceptKey = StatusKeys.Active
                }
            };
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));

            var afterTest = base.DoTestInsert(strawberry);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Described, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
        }

        /// <summary>
        /// Test the insert of an identified entity
        /// </summary>
        [TestMethod]
        public void TestInsertIdentifiedEntity()
        {
            var mid = String.Format("urn:uuid:{0}", Guid.NewGuid());
            Entity organization = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                TypeConcept = new Concept()
                {
                    Key = Guid.NewGuid(),
                    Mnemonic = "Public Institution"
                },
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier()
                    {
                        Authority = new AssigningAuthority()
                        {
                            Name = "InfomanIdentifier",
                            Oid = "1.2.3.4.6.7.8.9",
                            Key = Guid.NewGuid(),
                            DomainName = "OINFMAN"
                        },
                        Value = mid
                    }
                },
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "HAMILTON HEALTH SCIENCES"),
                    new EntityName(NameUseKeys.Search, "HHS")
                },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress(AddressUseKeys.Direct, "123 Main Street West", "Hamilton", "ON", "CA", "L8K5N2")
                }
            };

            var afterTest = base.DoTestInsert(organization, s_authorization);
            Assert.AreEqual(2, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Specific, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Organization, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "HHS")));
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "HAMILTON HEALTH SCIENCES")));
            Assert.AreEqual(1, afterTest.Addresses.Count);
            Assert.AreEqual(5, afterTest.Addresses[0].Component.Count);
            Assert.AreEqual(AddressUseKeys.Direct, afterTest.Addresses[0].AddressUseKey);
            Assert.AreEqual(1, afterTest.Identifiers.Count);
            Assert.AreEqual("1.2.3.4.6.7.8.9", afterTest.Identifiers[0].Authority.Oid);
            Assert.AreEqual("OINFMAN", afterTest.Identifiers[0].Authority.DomainName);
            Assert.AreEqual(mid, afterTest.Identifiers[0].Value);

            // Test lookup by identifier
            var lookupId = base.DoTestQuery(o => o.Identifiers.Any(i => i.Value == mid && i.Authority.Oid == "1.2.3.4.6.7.8.9"), afterTest.Key, s_authorization);



        }
        /// <summary>
        /// Test the insertion of an anonymous entity
        /// </summary>
        [TestMethod]
        public void TestInsertAnonymousInstanceEntity()
        {
            Entity strawberry = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));
            strawberry.Addresses.Add(new EntityAddress(AddressUseKeys.Direct, "123 Main Street West", "Hamilton", "ON", "Canada", "L8K5N2"));

            var afterTest = base.DoTestInsert(strawberry);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Specific, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
            Assert.AreEqual(1, afterTest.Addresses.Count);
            Assert.AreEqual(5, afterTest.Addresses[0].Component.Count);
            Assert.AreEqual(AddressUseKeys.Direct, afterTest.Addresses[0].AddressUseKey);
            Assert.AreEqual(NameUseKeys.Assigned, afterTest.Names[0].NameUseKey);
        }

        /// <summary>
        /// Obsolete an entity
        /// </summary>
        [TestMethod]
        public void TestObsoleteEntity()
        {
            Entity toBeKilled = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Food,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            toBeKilled.Names.Add(new EntityName(NameUseKeys.Assigned, "Kill Me!"));

            var afterTest = base.DoTestInsert(toBeKilled, s_authorization);
            var id = afterTest.Key;
            Assert.AreEqual(StatusKeys.Active, afterTest.StatusConcept.Key);

            // Obsolete
            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();
            var afterObsolete = idp.Obsolete(afterTest, s_authorization, TransactionMode.Commit);
            afterObsolete = idp.Get(new Identifier<Guid>(id), s_authorization, false);

            // Assert
            Assert.AreEqual(StatusKeys.Obsolete, afterObsolete.StatusConcept.Key);


        }

        /// <summary>
        /// Test query of data by name
        /// </summary>
        [TestMethod]
        public void TestQueryByName()
        {
            Entity toBeQueried = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Described,
                ClassConceptKey = EntityClassKeys.Place,
                StatusConceptKey = StatusKeys.Active,
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            toBeQueried.Names.Add(new EntityName(NameUseKeys.Assigned, "Some Clinic"));

            var afterTest = base.DoTestInsert(toBeQueried, s_authorization);
            var id = afterTest.Key;
            Assert.AreEqual(StatusKeys.Active, afterTest.StatusConcept.Key);

            // Query 
            var query = base.DoTestQuery(o => o.CreationTime > DateTimeOffset.MinValue && o.ClassConcept.Key == EntityClassKeys.Place && o.Names.Any(n => n.NameUse.Key == NameUseKeys.Assigned && n.Component.Any(c => c.Value == "Some Clinic")), id, s_authorization);
            Assert.AreEqual(1, query.Count());

            // No results
            var idp = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();
            query = idp.Query(o => o.CreationTime < DateTimeOffset.MinValue && o.ClassConcept.Key == EntityClassKeys.Place && o.Names.Any(n => n.NameUse.Key == NameUseKeys.Assigned && n.Component.Any(c => c.Value == "Some Clinic")), s_authorization);
            Assert.AreEqual(0, query.Count());

            // One result (like)
            query = idp.Query(o => o.Names.Any(n=>n.Component.Any(c => c.Value.Contains("Clinic"))), s_authorization);
            Assert.AreEqual(1, query.Count());

        }

        [TestMethod]
        public void TestRelateTwoEntities()
        {
            Entity e1 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "PARENT")
                }
            }, e2 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Place,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "CHILD")
                }
            };

            // Associate: PARENT > CHILD
            e1.Relationships.Add(new EntityRelationship(EntityRelationshipTypeKeys.OwnedEntity, e2));

            // Persist
            var afterInsert = base.DoTestInsert(e1);
            Assert.AreEqual(EntityClassKeys.Organization, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Relationships.Count);
            Assert.AreEqual(EntityRelationshipTypeKeys.OwnedEntity, afterInsert.Relationships[0].RelationshipTypeKey);
            Assert.AreEqual(EntityClassKeys.Place, afterInsert.Relationships[0].TargetEntity.ClassConceptKey);

        }

        /// <summary>
        /// Test persisting an entity with telecom address
        /// </summary>
        [TestMethod]
        public void TestEntityWithTelecom()
        {
            Entity e1 = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Organization,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.OfficialRecord, "GOOD HEALTH HOSPITAL SYSTEM")
                }
            };
            e1.Telecoms.Add(new EntityTelecomAddress(TelecomAddressUseKeys.WorkPlace, "mailto:bob@bob.com"));
            
            // Persist
            var afterInsert = base.DoTestInsert(e1, s_authorization);
            Assert.AreEqual(EntityClassKeys.Organization, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Telecoms.Count);
            Assert.AreEqual(AddressUseKeys.WorkPlace, afterInsert.Telecoms[0].AddressUseKey);
            Assert.AreEqual("mailto:bob@bob.com", afterInsert.Telecoms[0].Value);

        }

        /// <summary>
        /// Tests an entity with an extended attribute
        /// </summary>
        [TestMethod]
        public void TestEntityExtensions()
        {
            Entity person = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Person,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Smith", "Baby")
                }
            };
            person.Extensions.Add(new EntityExtension()
            {
                ExtensionType = new ExtensionType()
                {
                    Name = "http://openiz.org/oiz/birthcertificate",
                    ExtensionHandler = typeof(EntityPersistenceServiceTest)
                },
                ExtensionValue = new byte[] { 1 }
            });

            var afterInsert = base.DoTestInsert(person);
            Assert.AreEqual(EntityClassKeys.Person, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Extensions.Count);
            Assert.AreEqual(typeof(EntityPersistenceServiceTest), person.Extensions[0].ExtensionType.ExtensionHandler);
            Assert.IsTrue(BitConverter.ToBoolean(person.Extensions[0].ExtensionValue, 0));
        }

        /// <summary>
        /// Test the adding of notes to a patient
        /// </summary>
        [TestMethod]
        public void TestEntityNotePersistence()
        {
            Entity person = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Person,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Smith", "John")
                }
            };
            person.Notes.Add(new EntityNote(Guid.Empty, "He doesn't even like Peanutbutter!!!!!")
            {
                Author= new Person()
                {
                    Names = new List<EntityName>()
                    {
                        new EntityName(NameUseKeys.Legal, "Doctor", "Doctor")
                    }
                }
            });

            var afterInsert = base.DoTestInsert(person);
            Assert.AreEqual(EntityClassKeys.Person, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Notes.Count);
            Assert.IsNotNull(person.Notes[0].Author);
            Assert.AreEqual("He doesn't even like Peanutbutter!!!!!", person.Notes[0].Text);
        }

        /// <summary>
        /// Test the adding of tags to a patient
        /// </summary>
        [TestMethod]
        public void TestEntityTagPersistence()
        {
            Entity person = new Entity()
            {
                DeterminerConceptKey = DeterminerKeys.Specific,
                ClassConceptKey = EntityClassKeys.Person,
                StatusConceptKey = StatusKeys.Active,
                Names = new List<EntityName>()
                {
                    new EntityName(NameUseKeys.Legal, "Smith", "Johnny")
                }
            };
            person.Tags.Add(new EntityTag("noPeanutButter", "true"));

            var afterInsert = base.DoTestInsert(person, s_authorization);
            Assert.AreEqual(EntityClassKeys.Person, afterInsert.ClassConceptKey);
            Assert.AreEqual(1, afterInsert.Tags.Count);
            Assert.AreEqual("noPeanutButter", person.Tags[0].TagKey);
            Assert.AreEqual("true", person.Tags[0].Value);
        }

    }
        
   
}
