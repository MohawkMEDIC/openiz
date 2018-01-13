using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.DataTypes;
using System.Collections.Generic;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;
using System.IO;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    [TestClass]
    public class ManufacturedMaterialPersistenceTest : PersistenceTest<ManufacturedMaterial>
    {
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
           

        }
        /// <summary>
        /// Test the update of a manufactured material
        /// </summary>
        [TestMethod]
        public void TestUpdateManufacturedMaterial()
        {
            ManufacturedMaterial mmat = new ManufacturedMaterial()
            {
                LotNumber = "AAAAA",
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(new AssigningAuthority() { DomainName = "GTIN", Name = "Global Trade Identifier", Oid = "1.2.3.4.5.6.9098766" }, "20304303")
                },
                Names = new List<EntityName>() { new EntityName(NameUseKeys.Assigned, "ACME OPV Vaccine") },
                DeterminerConceptKey = DeterminerKeys.Specific,
                ExpiryDate = DateTime.Now,
                IsAdministrative = false
            };
            var afterTest = base.DoTestUpdate(mmat, AuthenticationContext.SystemPrincipal, "LotNumber");

            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Specific, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.ManufacturedMaterial, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "ACME OPV Vaccine")));

            // Update
            
        }
    }
}
