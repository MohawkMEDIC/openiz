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

            IPasswordHashingService hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();
            var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            dataService.Insert(new SecurityUser()
            {
                UserName = "admin@identitytest.com",
                Email = "admin@identitytest.com",
                PasswordHash = hashingService.EncodePassword("password"),
            }, null, TransactionMode.Commit);
            dataService.Insert(new SecurityUser()
            {
                UserName = "user@identitytest.com",
                Email = "user@identitytest.com",
                PasswordHash = hashingService.EncodePassword("password"),
            }, null, TransactionMode.Commit);
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

            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();

            // Reset data for test
            var user = dataPersistence.Query(u => u.UserName == "user@identitytest.com", null).First();
            user.LockoutEnabled = false;
            user.LastLoginTime = default(DateTimeOffset);
            user.InvalidLoginAttempts = 0;
            dataPersistence.Update(user, provider.Authenticate("admin@identitytest.com", "password"), TransactionMode.Commit);

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


        /// <summary>
        /// Tests that the authenticate method successfully locks a user account after three attempts
        /// </summary>
        [TestMethod]
        public void TestAuthenticateLockout()
        {

            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            IIdentityProviderService provider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            // Reset data for test
            var user = dataPersistence.Query(u => u.UserName == "user@identitytest.com", null).First();
            user.LockoutEnabled = false;
            user.LastLoginTime = default(DateTimeOffset);
            user.InvalidLoginAttempts = 0;
            dataPersistence.Update(user, provider.Authenticate("admin@identitytest.com", "password"), TransactionMode.Commit);


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
            Assert.IsTrue(user.InvalidLoginAttempts >= 4);
            Assert.AreEqual(default(DateTimeOffset), user.LastLoginTime);
            Assert.IsTrue(user.LockoutEnabled);


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

        /// <summary>
        /// Tests that the identity provider can change passwords
        /// </summary>
        [TestMethod]
        public void TestChangePassword()
        {

            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            IIdentityProviderService identityProvider = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var user = dataPersistence.Query(u => u.UserName == "admin@identitytest.com", null).First();
            var existingPassword = user.PasswordHash;

            // Now change the password
            var principal = identityProvider.Authenticate("admin@identitytest.com", "password");
            identityProvider.ChangePassword("admin@identitytest.com", "newpassword", principal);
            user = dataPersistence.Get(user.Id, principal, false);
            Assert.AreNotEqual(existingPassword, user.PasswordHash);

            // Change the password back 
            user.PasswordHash = existingPassword;
            dataPersistence.Update(user, principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Tests that anonymous user creation works
        /// </summary>
        [TestMethod]
        public void TestAnonymousUserCreation()
        {

            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            var identityService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();

            var identity = identityService.CreateIdentity("anonymous@identitytest.com", "mypassword", null);
            Assert.IsNotNull(identity);
            Assert.IsFalse(identity.IsAuthenticated);

            // Now verify with data persistence
            var dataUser = dataPersistence.Query(u => u.UserName == "anonymous@identitytest.com", null).First();
            Assert.AreEqual(hashingService.EncodePassword("mypassword"), dataUser.PasswordHash);
        }

        /// <summary>
        /// Tests that administrative user creation works
        /// </summary>
        [TestMethod]
        public void TestAdministrativeUserCreation()
        {

            var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            var identityService = ApplicationContext.Current.GetService<IIdentityProviderService>();
            var hashingService = ApplicationContext.Current.GetService<IPasswordHashingService>();

            var authContext = identityService.Authenticate("admin@identitytest.com", "password");
            var identity = identityService.CreateIdentity("admincreated@identitytest.com", "mypassword", authContext);
            Assert.IsNotNull(identity);
            Assert.IsFalse(identity.IsAuthenticated);

            // Now verify with data persistence
            var dataUser = dataPersistence.Query(u => u.UserName == "admincreated@identitytest.com", null).First();
            Assert.AreEqual(hashingService.EncodePassword("mypassword"), dataUser.PasswordHash);
            Assert.IsFalse(dataUser.LockoutEnabled);
            Assert.AreEqual(authContext.Identity.Name, dataUser.CreatedBy.UserName);

            // Now authenticate
            var authUser = identityService.Authenticate("admincreated@identitytest.com", "mypassword");

        }
    }
}
