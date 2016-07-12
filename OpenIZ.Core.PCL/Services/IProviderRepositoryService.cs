using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
	public interface IProviderRepositoryService
	{
		/// <summary>
		/// Inserts the specified provider.
		/// </summary>
		/// <param name="provider">The provider to insert.</param>
		/// <returns>Returns the inserted provider.</returns>
		Provider Insert(Provider provider);

		/// <summary>
		/// /// Saves the specified provider.
		/// </summary>
		/// <param name="provider">The provider to save.</param>
		/// <returns>Returns the saved provider.</returns>
		Provider Save(Provider provider);

		/// <summary>
		/// Obsoletes the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider to obsolete.</param>
		/// <returns>Returns the obsoleted provider.</returns>
		Provider Obsolete(Guid id);

		/// <summary>
		/// Gets the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider.</param>
		/// <param name="versionId">The version id of the provider.</param>
		/// <returns>Returns the specified provider.</returns>
		Provider Get(Guid id, Guid versionId);


		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate);

		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <param name="count">The count of the providers to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate, int offset, int? count, out int totalCount);
	}
}
