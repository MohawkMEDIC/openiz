using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Security;
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
    public class PersistenceTest<TModel> : DataTest where TModel : BaseData
    {

        /// <summary>
        /// Test the insertion of a valid security user
        /// </summary>
        public TModel DoTestInsert(TModel objectUnderTest, IPrincipal authContext = null)
        {

            // Store user
            IDataPersistenceService<TModel> persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TModel>>();
            Assert.IsNotNull(persistenceService);

            var objectAfterTest = persistenceService.Insert(objectUnderTest, authContext, TransactionMode.Commit);
            // Key should be set
            Assert.AreNotEqual(Guid.Empty, objectAfterTest.Key);

            // Verify
            objectAfterTest = persistenceService.Get(objectAfterTest.Id, authContext, false);
            Assert.AreNotEqual(default(DateTimeOffset), objectAfterTest.CreationTime);

            return objectAfterTest;
        }

        /// <summary>
        /// Do a test step for an update
        /// </summary>
        public TModel DoTestUpdate(TModel objectUnderTest, IPrincipal authContext, String propertyToChange)
        {

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
            objectAfterUpdate = persistenceService.Get(objectAfterUpdate.Id, authContext, false);
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
