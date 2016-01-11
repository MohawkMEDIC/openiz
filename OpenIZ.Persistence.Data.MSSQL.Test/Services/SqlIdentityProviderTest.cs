using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Security;
using System.Security.Cryptography;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using System.Security;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    [TestClass]
    public class SqlIdentityProviderTest : DataTest
    {

        /// <summary>
        /// Class startup
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
                           "DataDirectory",
                           Path.Combine(context.TestDeploymentDir, string.Empty));

            SHA256 hasher = SHA256.Create();
            using (var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>())
            {
                dataService.Insert(new SecurityUser()
                {
                    UserName = "admin@identitytest.com",
                    Email = "admin@identitytest.com",
                    PasswordHash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes("password"))),
                }, null, TransactionMode.None);
                dataService.Insert(new SecurityUser()
                {
                    UserName = "user@identitytest.com",
                    Email = "user@identitytest.com",
                    PasswordHash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes("password"))),
                }, null, TransactionMode.None);
            }
        }

        /// <summary>
        /// Tests that the authenticate method successfully authenticates a user
        /// </summary>
        [TestMethod]
        public void TestAuthenticateSuccess()
        {

            IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var principal = provider.Authenticate("admin@identitytest.com", "password");
            Assert.AreEqual("admin@identitytest.com", principal.Identity.Name);
            Assert.IsTrue(principal.Identity.IsAuthenticated);
            Assert.AreEqual("Password", principal.Identity.AuthenticationType);

        }

        /// <summary>
        /// Tests that the authenticate method successfully retrieves a non-authenticated identity
        /// </summary>
        [TestMethod]
        public void TestGetNonAuthenticatedPrincipal()
        {

            IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var identity = provider.GetIdentity("admin@identitytest.com");
            Assert.AreEqual("admin@identitytest.com", identity.Name);
            Assert.IsFalse(identity.IsAuthenticated);
            Assert.IsNull(identity.AuthenticationType);

        }
        /// <summary>
        /// Tests that the authenticate method successfully logs an invalid login attempt
        /// </summary>
        [TestMethod]
        public void TestInvalidLoginAttemptCount()
        {

            using (var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>())
            {
                IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();

                // Reset data for test
                var user = dataPersistence.Query(u => u.UserName == "user@identitytest.com", null).First();
                user.LockoutEnabled = false;
                user.LastLoginTime = default(DateTimeOffset);
                user.InvalidLoginAttempts = 0;
                dataPersistence.Update(user, provider.Authenticate("admin@identitytest.com","password"), TransactionMode.None);

                try
                {
                    
                    var principal = provider.Authenticate("user@identitytest.com", "passwordz");
                    Assert.Fail("Should throw SecurityException");
                }
                catch (SecurityException)
                {
                    // We should have a lockout
                    user = dataPersistence.Get(user.Id, null, false);
                    Assert.AreEqual(1, user.InvalidLoginAttempts);
                    Assert.AreEqual(default(DateTimeOffset), user.LastLoginTime);
                    Assert.IsFalse(user.LockoutEnabled);

                }

            }
        }


        /// <summary>
        /// Tests that the authenticate method successfully locks a user account after three attempts
        /// </summary>
        [TestMethod]
        public void TestAuthenticateLockout()
        {

            using (var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>())
            {
                IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();
                // Reset data for test
                var user = dataPersistence.Query(u => u.UserName == "user@identitytest.com", null).First();
                user.LockoutEnabled = false;
                user.LastLoginTime = default(DateTimeOffset);
                user.InvalidLoginAttempts = 0;
                dataPersistence.Update(user, provider.Authenticate("admin@identitytest.com", "password"), TransactionMode.None);


                // Try 4 times to log in
                for (int i = 0; i < 4; i++)
                    try
                    {
                        var principal = provider.Authenticate("user@identitytest.com", "passwordz");
                        Assert.Fail("Should throw SecurityException");
                    }
                    catch (SecurityException)
                    { }

                // We should have a lockout
                user = dataPersistence.Get(user.Id, null, false);
                Assert.AreEqual(4, user.InvalidLoginAttempts);
                Assert.AreEqual(default(DateTimeOffset), user.LastLoginTime);
                Assert.IsTrue(user.LockoutEnabled);

            }
        }

        /// <summary>
        /// Tests that the authenticate method successfully authenticates a user
        /// </summary>
        [TestMethod]
        public void TestAuthenticateFailure()
        {
            try
            {
                IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();
                var principal = provider.Authenticate("admin@identitytest.com", "passwordz");
                Assert.Fail("Should throw SecurityException");
            }
            catch(SecurityException)
            { }
        }


    }
}
