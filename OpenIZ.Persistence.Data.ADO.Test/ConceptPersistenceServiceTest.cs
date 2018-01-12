using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    /// <summary>
    /// Concept persistence service test
    /// </summary>
    [TestClass]
    public class ConceptPersistenceServiceTest : PersistenceTest<Concept>
    {

        private static IPrincipal s_authorization;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            s_authorization = AuthenticationContext.SystemPrincipal;

        }

        /// <summary>
        /// Tests that the concept persistence service can successfully
        /// insert and retrieve a concept
        /// </summary>
        [TestMethod]
        public void TestInsertSimpleConcept()
        {
            Concept simpleConcept = new Concept()
            {
                ClassKey = ConceptClassKeys.Other,
                IsSystemConcept = true,
                Mnemonic = "TESTCODE1"
            };
            var afterTest = base.DoTestInsert(simpleConcept, s_authorization);
            Assert.AreEqual("TESTCODE1", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsTrue(afterTest.IsSystemConcept);
        }

        /// <summary>
        /// Tests that the concept persistence service can persist a 
        /// simple concept which has a display name
        /// </summary>
        [TestMethod]
        public void TestInsertNamedConcept()
        {
            Concept namedConcept = new Concept()
            {
                ClassKey = ConceptClassKeys.Other,
                IsSystemConcept = false,
                Mnemonic = "TESTCODE2"
            };
            
            // Names
            namedConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm
            });

            // Insert
            var afterTest = base.DoTestInsert(namedConcept, s_authorization);
            Assert.AreEqual("TESTCODE2", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsFalse(afterTest.IsSystemConcept);
            Assert.AreEqual(1, afterTest.ConceptNames.Count);
            Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
            Assert.AreEqual("Test Code", afterTest.ConceptNames[0].Name);
        }

        /// <summary>
        /// Tests that the concept persistence service can persist a 
        /// simple concept which has a display name
        /// </summary>
        [TestMethod]
        public void TestUpdateNamedConcept()
        {

            Concept namedConcept = new Concept()
            {
                ClassKey = ConceptClassKeys.Other,
                IsSystemConcept = false,
                Mnemonic = "TESTCODE3"
            };

            // Names
            namedConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code 1",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
                PhoneticCode = "E"
            });
            namedConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code 2",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
                PhoneticCode = "E"
            });

            // Insert
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            var afterTest = persistenceService.Insert(namedConcept, s_authorization, TransactionMode.Commit);

            Assert.AreEqual("TESTCODE3", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsFalse(afterTest.IsSystemConcept);
            Assert.AreEqual(2, afterTest.ConceptNames.Count);
            Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
            Assert.IsTrue(afterTest.ConceptNames.Exists(n => n.Name == "Test Code 1"));
            Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);
            Assert.IsNotNull(afterTest.CreatedBy);

            var originalId = afterTest.VersionKey;

            // Step 1: Test an ADD of a name
            afterTest.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code 3",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
                PhoneticCode = "E"
            });
            afterTest.Mnemonic = "TESTCODE3_A";
            afterTest = persistenceService.Update(afterTest, s_authorization, TransactionMode.Commit);
            Assert.AreEqual(3, afterTest.ConceptNames.Count);
            Assert.AreEqual("TESTCODE3_A", afterTest.Mnemonic);
            Assert.IsNotNull(afterTest.PreviousVersion);
            Assert.AreEqual(originalId, afterTest.PreviousVersionKey);
            var updateKey = afterTest.VersionKey;

            // Verify 2: Remove a name
            afterTest.ConceptNames.RemoveAt(1);
            afterTest.ConceptNames[0].Language = "fr";
            afterTest = persistenceService.Update(afterTest, s_authorization, TransactionMode.Commit);
            Assert.AreEqual(2, afterTest.ConceptNames.Count);
            Assert.IsTrue(afterTest.ConceptNames.Exists(n => n.Language == "fr"));
            Assert.IsNotNull(afterTest.PreviousVersion);
            Assert.AreEqual(updateKey, afterTest.PreviousVersionKey);
            Assert.IsNotNull(afterTest.PreviousVersion.PreviousVersion);
            Assert.AreEqual(originalId, afterTest.PreviousVersion.PreviousVersionKey);
        }

        /// <summary>
        /// Tests that the concept persistence service can persist a 
        /// simple concept which has a display name
        /// </summary>
        [TestMethod]
        public void TestInsertReferenceTermConcept()
        {
            Concept refTermConcept = new Concept()
            {
                ClassKey = ConceptClassKeys.Other,
                IsSystemConcept = false,
                Mnemonic = "TESTCODE5"
            };

            // Names
            refTermConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code",
                Language = "en"
            });

            // Reference term
            refTermConcept.ReferenceTerms.Add(new ConceptReferenceTerm()
            {
                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs,
                ReferenceTerm = new ReferenceTerm()
                {
                    CodeSystemKey = CodeSystemKeys.LOINC,
                    Mnemonic = "X-4039503-403"
                }
            });

            // Insert
            var afterTest = base.DoTestInsert(refTermConcept, s_authorization);
            Assert.AreEqual("TESTCODE5", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsFalse(afterTest.IsSystemConcept);
            Assert.AreEqual(1, afterTest.ConceptNames.Count);
            Assert.AreEqual(1, afterTest.ReferenceTerms.Count);
            Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
            Assert.AreEqual(ConceptRelationshipTypeKeys.SameAs, afterTest.ReferenceTerms[0].RelationshipTypeKey);
            Assert.IsNotNull(afterTest.ReferenceTerms[0].RelationshipType);
            Assert.IsNotNull(afterTest.ReferenceTerms[0].ReferenceTerm);
            Assert.AreEqual(CodeSystemKeys.LOINC, afterTest.ReferenceTerms[0].ReferenceTerm.CodeSystem.Key);
            Assert.AreEqual("Test Code", afterTest.ConceptNames[0].Name);
        }


        /// <summary>
        /// Tests that the concept persistence service can persist a 
        /// simple concept which has a display name
        /// </summary>
        [TestMethod]
        public void TestUpdateConceptReferenceTerm()
        {
            Concept refTermConcept = new Concept()
            {
                ClassKey = ConceptClassKeys.Other,
                IsSystemConcept = false,
                Mnemonic = "TESTCODE6"
            };

            // Names
            refTermConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
                PhoneticCode = "E"
            });

            // Reference term
            refTermConcept.ReferenceTerms.Add(new ConceptReferenceTerm()
            {
                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs,
                ReferenceTerm = new ReferenceTerm()
                {
                    CodeSystemKey = CodeSystemKeys.LOINC,
                    Mnemonic = "X-4039503-402"
                }
            });

            // Insert
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            var afterTest = persistenceService.Insert(refTermConcept, s_authorization, TransactionMode.Commit);

            Assert.AreEqual("TESTCODE6", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsFalse(afterTest.IsSystemConcept);
            Assert.AreEqual(1, afterTest.ConceptNames.Count);
            Assert.AreEqual(1, afterTest.ReferenceTerms.Count);
            Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
            Assert.AreEqual(ConceptRelationshipTypeKeys.SameAs, afterTest.ReferenceTerms[0].RelationshipTypeKey);
            Assert.IsNotNull(afterTest.ReferenceTerms[0].RelationshipType);
            Assert.IsNotNull(afterTest.ReferenceTerms[0].ReferenceTerm);
            Assert.AreEqual(CodeSystemKeys.LOINC, afterTest.ReferenceTerms[0].ReferenceTerm.CodeSystem.Key);
            Assert.AreEqual("Test Code", afterTest.ConceptNames[0].Name);
            Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);

            // Update
            afterTest.ReferenceTerms.Add(new ConceptReferenceTerm()
            {
                RelationshipTypeKey = ConceptRelationshipTypeKeys.SameAs,
                ReferenceTerm = new ReferenceTerm()
                {
                    CodeSystemKey = CodeSystemKeys.LOINC,
                    Mnemonic = "X-4039503-408"
                }
            });
            afterTest = persistenceService.Update(afterTest, s_authorization, TransactionMode.Commit);
            Assert.AreEqual(2, afterTest.ReferenceTerms.Count);
            Assert.IsTrue(afterTest.ReferenceTerms.Any(o => o.ReferenceTerm.Mnemonic == "X-4039503-408"));

            // Remove one
            afterTest.ReferenceTerms.RemoveAt(0);
            afterTest = persistenceService.Update(afterTest, s_authorization, TransactionMode.Commit);
            Assert.AreEqual(1, afterTest.ReferenceTerms.Count);

        }
    }
}
