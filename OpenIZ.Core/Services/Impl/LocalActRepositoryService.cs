using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Acts;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Security;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Local act repository service
    /// </summary>
    public class LocalActRepositoryService : IActRepositoryService
    {
        /// <summary>
        /// Find acts matching the predicate
        /// </summary>
        public IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform the search
        /// </summary>
        public IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> filter, int offset, int? count, out int totalResults)
        {
            var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();
            if (persistenceService == null)
                throw new InvalidOperationException("No concept persistence service found");

            return persistenceService.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalResults);

        }

        public Act Get(Guid key, Guid versionId)
        {
            throw new NotImplementedException();
        }

        public Act Insert(Act insert)
        {
            throw new NotImplementedException();
        }

        public Act Obsolete(Guid key)
        {
            throw new NotImplementedException();
        }

        public Act Save(Act act)
        {
            throw new NotImplementedException();
        }
    }
}
