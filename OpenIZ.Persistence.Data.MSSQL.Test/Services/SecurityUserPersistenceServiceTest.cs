using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using MARC.HI.EHRS.SVC.Core.Services.Security;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Summary description for SecurityUserPersistenceServiceTest
    /// </summary>
    [TestClass]
    public class SecurityUserPersistenceServiceTest : DataTest
    {

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));
        }

        /// <summary>
        /// Test the insertion of a valid security user
        /// </summary>
        [TestMethod]
        public void TestInsertValidSecurityUser()
        {
            
            SecurityUser userUnderTest = new SecurityUser()
            {
                Email = "admin@test.com",
                EmailConfirmed = true,
                PasswordHash = "test_user_hash_store",
                SecurityHash = "test_security_hash",
                UserName = "admin"                
            };

            // Store user
            using (IDataPersistenceService<SecurityUser> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>())
            {
                var userAfterTest = persistenceService.Insert(userUnderTest, null, DataPersistenceMode.Production);

                // Key should be set
                Assert.AreNotEqual(Guid.Empty, userAfterTest.Key);
                Assert.AreNotEqual(default(DateTimeOffset), userAfterTest.CreationTime);
                Assert.AreEqual(userUnderTest.UserName, userAfterTest.UserName);
            }
        }

        /// <summary>
        /// Test the updating of a valid security user
        /// </summary>
        [TestMethod]
        public void TestUpdateValidSecurityUser()
        {

            SHA256 hasher = SHA256.Create();

            SecurityUser userUnderTest = new SecurityUser()
            {
                Email = "update@test.com",
                EmailConfirmed = false,
                PasswordHash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes("password"))),
                SecurityHash = "cert",
                UserName = "updateTest"
            };

            SecurityRole administrators = new SecurityRole()
            {
                Name = "Administrators"
            },
            users = new SecurityRole() {
                Name = "Users"
            };

            // Store user
            using (IDataPersistenceService<SecurityUser> userService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>())
            {
                IIdentityProviderService identityService = ApplicationContext.Current.GetService<IIdentityProviderService>();
                IDataPersistenceService<SecurityRole> roleService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityRole>>();

                roleService.DataContext = userService.DataContext;

                // Insert the user
                var userAfterInsert = userService.Insert(userUnderTest, null, DataPersistenceMode.Production);

                var authContext = identityService.Authenticate("updateTest", "password");

                administrators = roleService.Insert(administrators, authContext, DataPersistenceMode.Production);

                // Keys should be set
                Assert.AreNotEqual(Guid.Empty, userAfterInsert.Key);
                Assert.IsFalse(userAfterInsert.EmailConfirmed);
                Assert.IsNotNull(authContext);
                Assert.IsNull(userAfterInsert.UpdatedTime);

                // Update
                userAfterInsert.EmailConfirmed = true;
                userAfterInsert.Roles.Add(administrators);
                userAfterInsert.Roles.Add(users);
                var userAfterUpdate = userService.Update(userAfterInsert, authContext, DataPersistenceMode.Production);

                // Update attributes should be set
                Assert.AreEqual(userAfterInsert.Key, userAfterUpdate.Key);
                Assert.IsNotNull(userAfterUpdate.UpdatedTime);
                Assert.IsTrue(userAfterUpdate.EmailConfirmed);
                Assert.AreEqual(authContext.Identity.Name, userAfterUpdate.UpdatedBy.UserName);
            }
        }

    }
}
