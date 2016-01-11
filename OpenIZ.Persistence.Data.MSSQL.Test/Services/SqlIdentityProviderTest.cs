using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    [TestClass]
    public class SqlIdentityProviderTest
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
        }

        /// <summary>
        /// Tests that the authenticate method successfully authenticates a user
        /// </summary>
        [TestMethod]
        public void TestAuthenticateSuccess()
        {



        }
    }
}
