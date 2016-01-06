using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Security;
using System.Linq.Expressions;
using OpenIZ.Persistence.Data.MSSQL.Linq;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Linq
{
    [TestClass]
    public class ModelConversionVisitorTest
    {
        [TestMethod]
        public void TestConvertSecurityUserSimple()
        {
            
            Guid keyId = Guid.NewGuid();
            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.Key == keyId);
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.UserId == keyId);

            ModelConversionVisitor visitor = new ModelConversionVisitor();
            Expression testValue = visitor.Convert<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr, testValue);

        }
    }
}
