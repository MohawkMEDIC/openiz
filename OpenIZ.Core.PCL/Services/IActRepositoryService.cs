using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents the act repository service
    /// </summary>
    public interface IActRepositoryService
    {

        /// <summary>
        /// Validate the act 
        /// </summary>
        Act Validate(Act act);

        /// <summary>
        /// Insert the specified act
        /// </summary>
        Act Insert(Act insert);

        /// <summary>
        /// Insert or update the specified act
        /// </summary>
        Act Save(Act act);

        /// <summary>
        /// Obsolete the specified act
        /// </summary>
        Act Obsolete(Guid key);

        /// <summary>
        /// Get the specified act
        /// </summary>
        Act Get(Guid key, Guid versionId);

        /// <summary>
        /// Find all acts
        /// </summary>
        IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults);

        /// <summary>
        /// Find the substance administrations
        /// </summary>
        IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> filter, int offset, int? count, out int totalResults); 
    }
}
