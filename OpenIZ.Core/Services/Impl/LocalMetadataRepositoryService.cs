using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Security;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a local metadata repository service
    /// </summary>
    public class LocalMetadataRepositoryService : IMetadataRepositoryService
    {
        /// <summary>
        /// Find an assigning authority
        /// </summary>
        public IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            int totalResults = 0;
            return persistenceService.Query(query, 0, 100, AuthenticationContext.Current.Principal, out totalResults);

        }

        /// <summary>
        /// Find assigning authority 
        /// </summary>
        public IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query, int offset, int count, out int totalCount)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
        }

        /// <summary>
        /// Get the assigning authority
        /// </summary>
        public IdentifiedData GetAssigningAuthority(Guid id)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            return persistenceService.Get<Guid>(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
        }
    }
}
