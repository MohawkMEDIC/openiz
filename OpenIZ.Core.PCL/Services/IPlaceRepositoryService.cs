using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
	public interface IPlaceRepositoryService
	{
		/// <summary>
		/// Searches the place service for the specified place matching the 
		/// given predicate
		/// </summary>
		/// <param name="predicate">The predicate function to search by.</param>
		/// <returns>Returns a list of places.</returns>
		IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate);

		/// <summary>
		/// Searches the place service for the specified place matching the 
		/// given predicate
		/// </summary>
		/// <param name="predicate">The predicate function to search by.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of places.</returns>
		IEnumerable<Place> Find(Expression<Func<Place, bool>> predicate, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the specified place by key version key.
		/// </summary>
		/// <param name="key">The key of the place.</param>
		/// <param name="versionKey">The version key of the place.</param>
		/// <returns>Returns a place.</returns>
		IdentifiedData Get(Guid key, Guid versionKey);
	}
}
