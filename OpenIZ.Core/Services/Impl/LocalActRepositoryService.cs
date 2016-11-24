/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-8-3
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
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
		public IEnumerable<TAct> Find<TAct>(Expression<Func<TAct, bool>> filter, int offset, int? count, out int totalResults) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No concept persistence service found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			var results = persistenceService.Query(filter, offset, count, AuthenticationContext.Current.Principal, out totalResults);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
		}

		/// <summary>
		/// Get the specified act
		/// </summary>
		public TAct Get<TAct>(Guid key, Guid versionId) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			var result = persistenceService.Get<Guid>(new Identifier<Guid>(key, versionId), AuthenticationContext.Current.Principal, true);

			return businessRulesService != null ? businessRulesService.AfterRetrieve(result) : result;
		}

		/// <summary>
		/// Insert the specified act
		/// </summary>
		public TAct Insert<TAct>(TAct insert) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<TAct>>();

			insert = businessRulesService != null ? businessRulesService.BeforeInsert(insert) : insert;

			insert = persistenceService.Insert(insert, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService != null ? businessRulesService.AfterInsert(insert) : insert;
		}

		/// <summary>
		/// Obsolete the specified act
		/// </summary>
		public TAct Obsolete<TAct>(Guid key) where TAct : Act
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<TAct>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<TAct>)} not found");
			}

			var act = persistenceService.Get<Guid>(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (act == null)
			{
				throw new InvalidOperationException("Act not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			act = businessRulesService != null ? businessRulesService.BeforeObsolete(act) : act;

			act = persistenceService.Obsolete(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService != null ? businessRulesService.AfterObsolete(act) : act;
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

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<TAct>();

			try
			{
				act = businessRulesService != null ? businessRulesService.BeforeUpdate(act) : act;

				act = persistenceService.Update(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				act = businessRulesService != null ? businessRulesService.AfterUpdate(act) : act;
			}
			catch (DataPersistenceException)
			{
				act = businessRulesService != null ? businessRulesService.BeforeInsert(act) : act;

				act = persistenceService.Insert(act, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				act = businessRulesService != null ? businessRulesService.AfterInsert(act) : act;
			}

			return act;
		}

		/// <summary>
		/// Validates an act.
		/// </summary>
		public TAct Validate<TAct>(TAct data) where TAct : Act
		{
			// Correct author information and controlling act information
			data = data.Clean() as TAct;

			ISecurityRepositoryService userService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();

			var currentUserEntity = userService.GetUserEntity(AuthenticationContext.Current.Principal.Identity);

			if (!data.Participations.Any(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator))
			{
				data.Participations.Add(new ActParticipation(ActParticipationKey.Authororiginator, currentUserEntity));
			}

			return data;
		}
	}
}