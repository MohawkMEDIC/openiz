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
 * Date: 2016-1-13
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model.Security;
using System.Linq.Expressions;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model.Map;
using System.Text.RegularExpressions;
using System.Diagnostics;
using OpenIZ.Persistence.Data.MSSQL.Services;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Map
{
    [TestClass]
    public class ModelMapTest : DataTest
    {
        // Mapper
        private ModelMapper m_mapper = new ModelMapper(typeof(SqlServerPersistenceService).Assembly.GetManifestResourceStream("OpenIZ.Persistence.Data.MSSQL.Data.ModelMap.xml"));

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a simple key lookup expression
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserKey()
        {
            
            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.Key == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}"));
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.UserId == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}"));

            Expression testValue = this.m_mapper.MapModelExpression<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a property lookup expression with key
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserBinary()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.Key == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}") && u.UserName == "jdoe");
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.UserId == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}") && u.UserName == "jdoe");

            Expression testValue = this.m_mapper.MapModelExpression<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a chained parameter by key
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserNullable()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.ObsoletionTime.HasValue);
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.ObsoletionTime.HasValue);

            Expression testValue = this.m_mapper.MapModelExpression<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a chained parameter by key
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserChainedKey()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.CreatedBy.Key == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}"));
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.CreatedBy == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}"));

            Expression testValue = this.m_mapper.MapModelExpression<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a chained non-key parameter
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserChainedNonKey()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityUser, bool>> modelExpr = (u => u.CreatedBy.UserName == "jdoe");
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityUser, bool>> domainExpr = (u => u.CreatedByEntity.UserName == "jdoe");

            Expression testValue = this.m_mapper.MapModelExpression<SecurityUser, Data.SecurityUser>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }

        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a chained non-key parameter
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserChainedContainsKey()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityRole, bool>> modelExpr = (r => r.Users.Any(u => u.Key == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}")));
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityRole, bool>> domainExpr = (r => r.SecurityUserRoles.Any(u => u.SecurityUser.UserId == new Guid("{0f882233-318e-434a-9209-4e242c163fa2}")));

            Expression testValue = this.m_mapper.MapModelExpression<SecurityRole, Data.SecurityRole>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }


        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a chained non-key parameter
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserChainedContainsComplex()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityRole, bool>> modelExpr = (r => r.Users.Any(u => u.UserName == "jdoe"));
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityRole, bool>> domainExpr = (r => r.SecurityUserRoles.Any(u => u.SecurityUser.UserName == "jdoe"));

            Expression testValue = this.m_mapper.MapModelExpression<SecurityRole, Data.SecurityRole>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }


        /// <summary>
        /// This test ensures that the model conversion visitor is capable of converting 
        /// a deeply chained parameter
        /// </summary>
        [TestMethod]
        public void TestConvertSecurityUserDoubleChainedContainsComplex()
        {

            Expression<Func<OpenIZ.Core.Model.Security.SecurityRole, bool>> modelExpr = (r => r.Users.Any(u => u.CreatedBy.UserName == "jdoe" && u.CreationTime < DateTimeOffset.Now && u.Email.EndsWith(r.Name)) && !r.ObsoletionTime.HasValue);
            Expression<Func<OpenIZ.Persistence.Data.MSSQL.Data.SecurityRole, bool>> domainExpr = (r => r.SecurityUserRoles.Any(u => u.SecurityUser.CreatedByEntity.UserName == "jdoe" && u.SecurityUser.CreationTime < DateTimeOffset.Now && u.SecurityUser.Email.EndsWith(r.Name)) && !r.ObsoletionTime.HasValue);

            Expression testValue = this.m_mapper.MapModelExpression<SecurityRole, Data.SecurityRole>(modelExpr);
            Assert.AreEqual(domainExpr.ToString(), testValue.ToString());

        }
    }
}
