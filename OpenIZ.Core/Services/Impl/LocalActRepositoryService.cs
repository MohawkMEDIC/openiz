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
	/// Local act repository service
	/// </summary>
	public class LocalActRepositoryService : IActRepositoryService
	{
		/// <summary>
		/// Find acts matching the predicate
		/// </summary>
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
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);
		}

		public Act Insert(Act insert)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Act>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<Act>)));
			}

			return persistenceService.Insert(insert, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

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
		/// Validate the act and prepare for storage
		/// </summary>
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