﻿/*
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
 * User: Nityan
 * Date: 2016-8-30
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an entity repository service.
	/// </summary>
	public class LocalEntityRepositoryService : IEntityRepositoryService
	{
		/// <summary>
		/// Finds a list of entities.
		/// </summary>
		/// <param name="query">The query to use to find the entities.</param>
		/// <returns>Returns a list of entities.</returns>
		public IEnumerable<Entity> Find(Expression<Func<Entity, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Query(query, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Finds a list of entities.
		/// </summary>
		/// <param name="query">The query to use to find the entities.</param>
		/// <param name="offSet">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of entities.</returns>
		public IEnumerable<Entity> Find(Expression<Func<Entity, bool>> query, int offSet, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Query(query, offSet, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be retrieved.</param>
		/// <param name="versionKey">The version key of the entity to be retrieved.</param>
		/// <returns>Returns an entity.</returns>
		public Entity Get(Guid key, Guid versionKey)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(key, versionKey), AuthenticationContext.Current.Principal, true); 
		}

		/// <summary>
		/// Inserts an entity.
		/// </summary>
		/// <param name="entity">The entity to be inserted.</param>
		/// <returns>Returns the inserted entity.</returns>
		public Entity Insert(Entity entity)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			return persistenceService.Insert(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be obsoleted.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public Entity Obsolete(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			var entity = persistenceService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (entity == null)
			{
				throw new InvalidOperationException("Entity not found");
			}

			return persistenceService.Obsolete(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Saves or inserts an entity.
		/// </summary>
		/// <param name="entity">The entity to be saved.</param>
		/// <returns>Returns the saved entity.</returns>
		public Entity Save(Entity entity)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException(string.Format("Unable to locate {0}", nameof(IDataPersistenceService<Entity>)));
			}

			try
			{
				return persistenceService.Update(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (KeyNotFoundException)
			{
				return persistenceService.Insert(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}