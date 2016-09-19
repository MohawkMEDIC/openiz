using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public class LocalActRepositoryService : IActRepositoryService
	{
		/// <summary>
		/// Finds acts based on a specific query.
		/// </summary>
		/// <param name="query">The query to use to find the acts.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalResults">The total results of the query.</param>
		/// <returns>Returns a list of acts.</returns>
		public IEnumerable<Act> FindActs(Expression<Func<Act, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Finds substance administrations based on a specific query.
		/// </summary>
		/// <param name="query">The query to use to find the substance administrations.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalResults">The total results of the query.</param>
		/// <returns>Returns a list of substance administrations.</returns>
		public IEnumerable<SubstanceAdministration> FindSubstanceAdministrations(Expression<Func<SubstanceAdministration, bool>> filter, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<SubstanceAdministration>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

		/// <summary>
		/// Gets a specific act by key and version key.
		/// </summary>
		/// <param name="key">The key of the act.</param>
		/// <param name="versionKey">The version key of the act.</param>
		/// <returns>Returns an act.</returns>
		public Act Get(Guid key, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);
		}

		/// <summary>
		/// Inserts a specific act.
		/// </summary>
		/// <param name="act">The act to be inserted.</param>
		/// <returns>Returns the inserted act.</returns>
		public Act Insert(Act insert)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Insert(insert, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes a specific act.
		/// </summary>
		/// <param name="key">The key of the act to obsolete.</param>
		/// <returns>Returns the obsoleted act.</returns>
		public Act Obsolete(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			var act = persistenceService.Get<Guid>(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (act == null)
			{
				throw new InvalidOperationException("Act not found");
			}

			return persistenceService.Obsolete(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Inserts or updates the specific act.
		/// </summary>
		/// <param name="act">The act to be inserted or saved.</param>
		/// <returns>Returns the inserted or saved act.</returns>
		public Act Save(Act act)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Validates an act.
		/// </summary>
		/// <param name="act">The act to be validated.</param>
		/// <returns>Returns the validated act.</returns>
		public Act Validate(Act data)
		{
			// Correct author information and controlling act information
			data = data.Clean() as Act;
			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var currentUserEntity = userService.GetUserEntity(AuthenticationContext.Current.Principal.Identity);
			if (!data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			return data;
		}
	}
}