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
 * Date: 2016-4-19
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Entities;
using System.Security.Principal;
using System.IO;
using OpenIZ.Core.Security;
using OpenIZ.Core.Model.Constants;

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
                TypeConceptKey = Guid.Parse("7F81B83E-0D78-4685-8BA4-224EB315CE54") // Some random concept for the "Type"
            };
            strawberry.Names.Add(new EntityName(NameUseKeys.Assigned, "Strawberries"));

            var afterTest = base.DoTestInsert(strawberry, s_authorization);
            Assert.AreEqual(1, afterTest.Names.Count);
            Assert.AreEqual(DeterminerKeys.Described, afterTest.DeterminerConceptKey);
            Assert.AreEqual(EntityClassKeys.Food, afterTest.ClassConceptKey);
            Assert.IsTrue(afterTest.Names.Exists(o => o.Component.Exists(c => c.Value == "Strawberries")));
        }
    }
}
