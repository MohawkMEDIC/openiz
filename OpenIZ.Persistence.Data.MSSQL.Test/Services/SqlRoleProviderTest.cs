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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System.Security.Principal;
using System.IO;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.MSSQL.Services;
using OpenIZ.Core.Model;
using OpenIZ.Core.Security;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Unit test for the SQL role provider
    /// </summary>
    [TestClass]
    public class SqlRoleProviderTest : DataTest
    {

        private static IPrincipal s_authorization;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
            IIdentityProviderService identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var identity = identityProvider.CreateIdentity(nameof(SqlRoleProviderTest), "password", AuthenticationContext.SystemPrincipal);

            // Give this identity the administrative functions group
            IRoleProviderService roleProvider = ApplicationContext.Current.GetService<IRoleProviderService>();
            roleProvider.AddUsersToRoles(new string[] { identity.Name }, new string[] { "ADMINISTRATORS" }, AuthenticationContext.SystemPrincipal);

            // Authorize
            s_authorization = identityProvider.Authenticate(nameof(SqlRoleProviderTest), "password");

        }

        /// <summary>
        /// Test the creation of a role
        /// </summary>
        [TestMethod]
        public void TestCreateRole()
        {

            var roleProvider = ApplicationContext.Current.GetService<IRoleProviderService>();
            Assert.IsNotNull(roleProvider);
            Assert.IsInstanceOfType(roleProvider, typeof(SqlRoleProvider));
            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            Assert.AreEqual(0, dataPersistence.Count(r => r.Name == "TestCreateRole", s_authorization));

            // Create the role
            roleProvider.CreateRole("TestCreateRole", s_authorization);
            Assert.AreEqual(1, dataPersistence.Count(r => r.Name == "TestCreateRole", s_authorization));

        }


        /// <summary>
        /// Test the adding of users to a role
        /// </summary>
        [TestMethod]
        public void TestAddUsersToRole()
        {

            var roleProvider = ApplicationContext.Current.GetService<IRoleProviderService>();
            Assert.IsNotNull(roleProvider);
            Assert.IsInstanceOfType(roleProvider, typeof(SqlRoleProvider));
            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();
            Assert.AreEqual(0, dataPersistence.Count(r => r.Name == "TestAddUsersToRole", s_authorization));

            // Create the role
            roleProvider.CreateRole("TestAddUsersToRole", s_authorization);
            Assert.AreEqual(1, dataPersistence.Count(r => r.Name == "TestAddUsersToRole", s_authorization));
            var modelRole = dataPersistence.Query(r => r.Name == "TestAddUsersToRole", s_authorization).First();
            Assert.AreEqual(0, modelRole.Users.Count);

            // Add users to the role
            var userProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            userProvider.CreateIdentity("UserInRole1", "role1password", s_authorization);
            userProvider.CreateIdentity("UserInRole2", "role2password", s_authorization);
            roleProvider.AddUsersToRoles(new String[] { "UserInRole1", "UserInRole2" }, new String[] { "TestAddUsersToRole" }, s_authorization);
            modelRole = dataPersistence.Get(modelRole.Id(), s_authorization, true);

            // Role provider
            Assert.AreEqual(2, modelRole.Users.Count);
            Assert.IsTrue(modelRole.Users.Exists(u => u.UserName == "UserInRole1"));
            Assert.IsTrue(modelRole.Users.Exists(u => u.UserName == "UserInRole2"));

            // Find users in role
            var usersInRole = roleProvider.FindUsersInRole("TestAddUsersToRole");
            Assert.IsTrue(usersInRole.Contains("UserInRole1"));
            Assert.IsTrue(usersInRole.Contains("UserInRole2"));

            // Is user in role test
            Assert.IsTrue(roleProvider.IsUserInRole("UserInRole1", "TestAddUsersToRole"));
        }

    }
}
