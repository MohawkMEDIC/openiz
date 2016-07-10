using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a repository service for managing places
    /// </summary>
    public interface IPlaceRepositoryService
    {

        /// <summary>
        /// Inserts the specified place
        /// </summary>
        Place Insert(Place plc);

        /// <summary>
        /// Saves the specified place
        /// </summary>
        Place Save(Place plc);

        /// <summary>
        /// Obsoletes the specified place
        /// </summary>
        Place Obsolete(Guid id);

        /// <summary>
        /// Gets the specified place
        /// </summary>
        Place Get(Guid id, Guid versionId);


        /// <summary>
        /// Searches the patient service for the specified place matching the 
        /// given predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate);

        /// <summary>
        /// Searches the database for the specified place
        /// </summary>
        IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount);


    }
}
