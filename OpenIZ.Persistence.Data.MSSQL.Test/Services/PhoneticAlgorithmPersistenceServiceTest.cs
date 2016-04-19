/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2016-1-20
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.DataTypes;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Phonetic algorithm test
    /// </summary>
    [TestClass]
    public class PhoneticAlgorithmPersistenceServiceTest : PersistenceTest<PhoneticAlgorithm>
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
        }

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
