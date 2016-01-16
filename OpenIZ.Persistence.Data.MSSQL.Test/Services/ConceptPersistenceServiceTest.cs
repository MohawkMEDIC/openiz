﻿using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core;
using OpenIZ.Core.Model.DataTypes;
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
    /// Concept persistence service test
    /// </summary>
    [TestClass]
    public class ConceptPersistenceServiceTest : PersistenceTest<Concept>
    {

        private static IPrincipal s_authorization;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
            IIdentityProviderService identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            identityProvider.CreateIdentity(nameof(ConceptPersistenceServiceTest), "password", null);
            s_authorization = identityProvider.Authenticate(nameof(ConceptPersistenceServiceTest), "password");

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
                ClassId = ConceptClassIds.OtherId,
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
                ClassId = ConceptClassIds.OtherId,
                IsSystemConcept = false,
                Mnemonic = "TESTCODE2"
            };
            
            // Names
            namedConcept.ConceptNames.Add(new ConceptName()
            {
                Name = "Test Code",
                Language = "en",
                PhoneticAlgorithm = PhoneticAlgorithm.EmptyAlgorithm,
                PhoneticCode = "E"
            });

            // Insert
            var afterTest = base.DoTestInsert(namedConcept, s_authorization);
            Assert.AreEqual("TESTCODE2", afterTest.Mnemonic);
            Assert.AreEqual("Other", afterTest.Class.Mnemonic);
            Assert.IsFalse(afterTest.IsSystemConcept);
            Assert.AreEqual(1, afterTest.ConceptNames.Count);
            Assert.AreEqual("en", afterTest.ConceptNames[0].Language);
            Assert.AreEqual("Test Code", afterTest.ConceptNames[0].Name);
            Assert.AreEqual("E", afterTest.ConceptNames[0].PhoneticCode);
        }

    }
}