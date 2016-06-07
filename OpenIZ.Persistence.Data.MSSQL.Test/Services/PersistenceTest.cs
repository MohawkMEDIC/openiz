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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Test.Services
{
    /// <summary>
    /// Persistence test
    /// </summary>
    public class PersistenceTest<TModel> : DataTest where TModel : IdentifiedData
    {

        /// <summary>
        /// Test the insertion of a valid security user
        /// </summary>
        public TModel DoTestInsert(TModel objectUnderTest, IPrincipal authContext = null)
        {

            // Auth context
            if (authContext == null)
                authContext = AuthenticationContext.AnonymousPrincipal;

            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            var objectAfterTest = persistenceService.Insert(objectUnderTest, authContext, TransactionMode.Commit);
            // Key should be set
            Assert.AreNotEqual(Guid.Empty, objectAfterTest.Key);

            // Verify
            objectAfterTest = persistenceService.Get(objectAfterTest.Id(), authContext, false);
            if(objectAfterTest is BaseEntityData)
                Assert.AreNotEqual(default(DateTimeOffset), (objectAfterTest as BaseEntityData).CreationTime);

            return objectAfterTest;
        }

        /// <summary>
        /// Do a test step for an update
        /// </summary>
        public TModel DoTestUpdate(TModel objectUnderTest, IPrincipal authContext, String propertyToChange)
        {

            // Auth context
            if (authContext == null)
                authContext = AuthenticationContext.AnonymousPrincipal;

            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            // Update the user
            var objectAfterInsert = persistenceService.Insert(objectUnderTest, authContext, TransactionMode.Commit);

            // Update
            var propertyInfo = typeof(TModel).GetProperty(propertyToChange);
            if (propertyInfo.PropertyType == typeof(String))
                propertyInfo.SetValue(objectAfterInsert, "NEW_VALUE");
            else if (propertyInfo.PropertyType == typeof(Nullable<DateTimeOffset>) ||
                propertyInfo.PropertyType == typeof(DateTimeOffset))
                propertyInfo.SetValue(objectAfterInsert, DateTimeOffset.MaxValue);
            else if (propertyInfo.PropertyType == typeof(Boolean) ||
                propertyInfo.PropertyType == typeof(Nullable<Boolean>))
                propertyInfo.SetValue(objectAfterInsert, true);

            var objectAfterUpdate = persistenceService.Update(objectAfterInsert, authContext, TransactionMode.Commit);
            Assert.AreEqual(objectAfterInsert.Key, objectAfterUpdate.Key);
            objectAfterUpdate = persistenceService.Get(objectAfterUpdate.Id(), authContext, false);
            // Update attributes should be set
            Assert.AreNotEqual(propertyInfo.GetValue(objectUnderTest), propertyInfo.GetValue(objectAfterUpdate));
            Assert.AreEqual(objectAfterInsert.Key, objectAfterUpdate.Key);

            return objectAfterUpdate;
        }

        /// <summary>
        /// Perform a query
        /// </summary>
        public IEnumerable<TModel> DoTestQuery(Expression<Func<TModel, bool>> predicate, Guid knownResultKey, IPrincipal authContext)
        {

            // Auth context
            if (authContext == null)
                authContext = AuthenticationContext.AnonymousPrincipal;

            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            // Perform query
            var results = persistenceService.Query(predicate, authContext);

            // Look for the known key
            Assert.IsTrue(results.Any(p => p.Key == knownResultKey), "Result doesn't contain known key");

            return results;
        }

    }
}
