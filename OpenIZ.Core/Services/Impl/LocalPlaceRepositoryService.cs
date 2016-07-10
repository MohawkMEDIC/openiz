using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Place repository that uses local persistence
    /// </summary>
    public class LocalPlaceRepositoryService : IPlaceRepositoryService
    {
        /// <summary>
        /// Find the specified place
        /// </summary>
        public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
        }

        /// <summary>
        /// Find the specified data with the controles
        /// </summary>
        public IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Gets the specified place
        /// </summary>
        public Place Get(Guid id, Guid versionId)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
        }

        /// <summary>
        /// Inserts the specified place
        /// </summary>
        public Place Insert(Place plc)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Insert(plc, AuthenticationContext.Current.Principal, TransactionMode.Commit);
        }

        /// <summary>
        /// Obsoletes the specified data
        /// </summary>
        public Place Obsolete(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            return persistenceService.Obsolete(new Place() { Key = id }, AuthenticationContext.Current.Principal, TransactionMode.Commit);

        }

        /// <summary>
        /// Inserts or updates the specified place
        /// </summary>
        public Place Save(Place plc)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Place>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No persistence service found");
            try
            {
                return persistenceService.Update(plc, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch (KeyNotFoundException)
            {
                return persistenceService.Insert(plc, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
        }
    }
}
