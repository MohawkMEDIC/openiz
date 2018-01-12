using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.DataTypes;
using System.IO;

namespace OpenIZ.Persistence.Data.ADO.Test
{
    /// <summary>
    /// Phonetic algorithm test
    /// </summary>
    [TestClass]
    public class PhoneticAlgorithmPersistenceServiceTest : PersistenceTest<PhoneticAlgorithm>
    {
        

        /// <summary>
        /// Tests the persistence layer successfully inserts a phonetic algorithm
        /// </summary>
        [TestMethod]
        public void TestInsertPhoneticAlgorithm()
        {
            PhoneticAlgorithm underTest = new PhoneticAlgorithm()
            {
                Handler = typeof(PhoneticAlgorithm).AssemblyQualifiedName,
                Name = "A Phonetic Algorithm"
            };
            var afterTest = base.DoTestInsert(underTest);

            Assert.AreEqual("A Phonetic Algorithm", afterTest.Name);
            Assert.AreEqual(typeof(PhoneticAlgorithm).AssemblyQualifiedName, afterTest.Handler);
        }

        /// <summary>
        /// Tests the persistence layer successfully updates a phonetic algorithm
        /// </summary>
        [TestMethod]
        public void TestUpdatePhoneticAlgorithm()
        {
            PhoneticAlgorithm underTest = new PhoneticAlgorithm()
            {
                Handler = typeof(PhoneticAlgorithm).AssemblyQualifiedName,
                Name = "An algorithm to be updated"
            };
            var afterTest = base.DoTestUpdate(underTest, null, "Name");
            Assert.AreEqual(typeof(PhoneticAlgorithm).AssemblyQualifiedName, afterTest.Handler);
        }

        /// <summary>
        /// Tests the persistence layer successfully queries a phonetic algorithm
        /// </summary>
        [TestMethod]
        public void TestQueryPhoneticAlgorithm()
        {
            PhoneticAlgorithm underTest = new PhoneticAlgorithm()
            {
                Handler = typeof(PhoneticAlgorithm).AssemblyQualifiedName,
                Name = "An algorithm to be queried"
            };
            var afterTest = base.DoTestInsert(underTest);
            Assert.AreEqual(typeof(PhoneticAlgorithm).AssemblyQualifiedName, afterTest.Handler);
            var queryResults = base.DoTestQuery(o => o.Name == "An algorithm to be queried", afterTest.Key, null);
            Assert.AreEqual(1, queryResults.Count());
        }

    }
}
