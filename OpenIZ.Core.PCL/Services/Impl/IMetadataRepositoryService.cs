using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Represents a repository which deals with metadata such as assigning authorities,
    /// concept classes, etc.
    /// </summary>
    public interface IMetadataRepositoryService
    {
        /// <summary>
        /// Gets an assigning authority
        /// </summary>
        IdentifiedData GetAssigningAuthority(Guid id);

        /// <summary>
        /// Finds the specified assigning authority 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression);

        /// <summary>
        /// Finds the specified assigning authority with restrictions
        /// </summary>
        IEnumerable<IdentifiedData> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> expression, int offset, int count, out int totalCount);
    }
}
