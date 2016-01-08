using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Security;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Summary description for SecurityUserPersistenceServiceTest
    /// </summary>
    [TestClass]
    public class SecurityUserPersistenceServiceTest : DataTest
    {
        /// <summary>
        /// Test the insertion of a valid security user
        /// </summary>
        [TestMethod]
        public void TestInsertValidSecurityUser()
        {
            
            SecurityUser userUnderTest = new SecurityUser()
            {
                CreationTime = DateTime.Now,
                Email = "admin@test.com",
                EmailConfirmed = true,
                PasswordHash = "test_user_hash_store",
                SecurityHash = "test_security_hash",
                UserName = "admin"                
            };

            // Store user
            IDataPersistenceService<SecurityUser> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SecurityUser>>();
            persistenceService.Insert(userUnderTest, null, DataPersistenceMode.Production);
            
        }
    }
}
