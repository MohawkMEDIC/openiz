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
		public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalResults);
		}

        /// <summary>
        /// Get the specified act
        /// </summary>
		public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);
		}

        /// <summary>
        /// Insert the specified act
        /// </summary>
		public TAct Insert<TAct>(TAct insert) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			return persistenceService.Insert(insert, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Obsolete the specified act
        /// </summary>
		public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

			var act = persistenceService.Get<Guid>(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (act == null)
			{
				throw new InvalidOperationException("Act not found");
			}

			return persistenceService.Obsolete(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

        /// <summary>
        /// Insert or update the specified act
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
		public TAct Save<TAct>(TAct act) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("{0} not found", nameof(IDataPersistenceService<TAct>)));
			}

            try
            {
                return persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
            catch(KeyNotFoundException)
            {
                return persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);
            }
		}

		/// <summary>
		/// Validate the act and prepare for storage
		/// </summary>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
			// Correct author information and controlling act information
			data = data.Clean() as TAct;
			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
			var currentUserEntity = userService.GetUserEntity(AuthenticationContext.Current.Principal.Identity);
			if (!data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			return data;
		}
	}
}