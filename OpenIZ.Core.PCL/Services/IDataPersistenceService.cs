using System;
using System.Collections;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Non-generic form of the data persistene service
	/// </summary>
	public interface IDataPersistenceService
	{
		/// <summary>
		/// Inserts the specified object
		/// </summary>
		Object Insert(Object data);

		/// <summary>
		/// Updates the specified data
		/// </summary>
		Object Update(Object data);

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		Object Obsolete(Object data);

		/// <summary>
		/// Gets the specified data
		/// </summary>
		Object Get(Guid id);

		/// <summary>
		/// Query based on the expression given
		/// </summary>
		IEnumerable Query(Expression query, int offset, int? count, out int totalResults);
	}
}