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
 * Date: 2016-9-2
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
using OpenIZ.Core.Exceptions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an entity repository service.
	/// </summary>
	public class LocalEntityRepositoryService : IEntityRepositoryService, IRepositoryService<Entity>, IRepositoryService<EntityRelationship>
	{
		/// <summary>
		/// Finds the specified data
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>IEnumerable&lt;TModel&gt;.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<EntityRelationship> Find(Expression<Func<EntityRelationship, bool>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Finds the specified data
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>IEnumerable&lt;TModel&gt;.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<EntityRelationship> Find(Expression<Func<EntityRelationship, bool>> query, int offset, int? count, out int totalResults)
		{
			throw new NotImplementedException();
		}

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
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			var results = persistenceService.Query(query, AuthenticationContext.Current.Principal);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
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
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			var results = persistenceService.Query(query, offSet, count, AuthenticationContext.Current.Principal, out totalCount);

			return businessRulesService != null ? businessRulesService.AfterQuery(results) : results;
		}

		/// <summary>
		/// Gets the specified data
		/// </summary>
		Entity IRepositoryService<Entity>.Get(Guid key)
		{
			return this.Get(key, Guid.Empty);
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
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			var result = persistenceService.Get(new Identifier<Guid>(key, versionKey), AuthenticationContext.Current.Principal, true);

			return businessRulesService?.AfterRetrieve(result) ?? result;
		}

		/// <summary>
		/// Gets the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>TModel.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the persistence service is not found.</exception>
		EntityRelationship IRepositoryService<EntityRelationship>.Get(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<EntityRelationship>)}");
			}

			return persistenceService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);
		}

		/// <summary>
		/// Inserts the specified data
		/// </summary>
		public EntityRelationship Insert(EntityRelationship data)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<EntityRelationship>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetService<IBusinessRulesService<EntityRelationship>>();

			data = businessRulesService?.BeforeInsert(data) ?? data;

			data = persistenceService.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService?.AfterInsert(data) ?? data;
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
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			entity = businessRulesService?.BeforeInsert(entity) ?? entity;

			entity = persistenceService.Insert(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService?.AfterInsert(entity) ?? entity;
		}

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		EntityRelationship IRepositoryService<EntityRelationship>.Obsolete(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<EntityRelationship>)}");
			}

			var entityRelationship = persistenceService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (entityRelationship == null)
			{
				throw new InvalidOperationException("Entity Relationship not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<EntityRelationship>();

			entityRelationship = businessRulesService?.BeforeObsolete(entityRelationship) ?? entityRelationship;

			entityRelationship = persistenceService.Obsolete(entityRelationship, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService?.AfterObsolete(entityRelationship) ?? entityRelationship;
		}

		/// <summary>
		/// Obsoletes an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be obsoleted.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the entity is not found.</exception>
		public Entity Obsolete(Guid key)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var entity = persistenceService.Get(new Identifier<Guid>(key), AuthenticationContext.Current.Principal, true);

			if (entity == null)
			{
				throw new InvalidOperationException("Entity not found");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			entity = businessRulesService?.BeforeObsolete(entity) ?? entity;

			entity = persistenceService.Obsolete(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

			return businessRulesService?.AfterObsolete(entity) ?? entity;
		}

		/// <summary>
		/// Saves the specified data
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>TModel.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the persistence service is not found.</exception>
		public EntityRelationship Save(EntityRelationship data)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<EntityRelationship>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			try
			{
				EntityRelationship old = null;

				if (data.Key.HasValue)
				{
					old = persistenceService.Get(new Identifier<Guid>(data.Key.Value), AuthenticationContext.Current.Principal, true);
				}

				if (old == null)
				{
					var tr = 0;
					old = persistenceService.Query(o => o.SourceEntityKey == data.SourceEntityKey && o.TargetEntityKey == data.TargetEntityKey, 0, 1, AuthenticationContext.Current.Principal, out tr).FirstOrDefault();
				}

				if (old == null)
				{
					throw new KeyNotFoundException(data.Key?.ToString());
				}

				return persistenceService.Update(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (DataPersistenceException)
			{
				return persistenceService.Insert(data, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
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
				throw new InvalidOperationException($"Unable to locate {nameof(IDataPersistenceService<Entity>)}");
			}

			var businessRulesService = ApplicationContext.Current.GetBusinessRulesService<Entity>();

			try
			{
				entity = businessRulesService?.BeforeUpdate(entity) ?? entity;

				entity = persistenceService.Update(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				entity = businessRulesService?.AfterUpdate(entity) ?? entity;
			}
			catch (DataPersistenceException)
			{
				entity = businessRulesService?.BeforeInsert(entity) ?? entity;

				entity = persistenceService.Insert(entity, AuthenticationContext.Current.Principal, TransactionMode.Commit);

				entity = businessRulesService?.AfterInsert(entity) ?? entity;
			}

			return entity;
		}
	}
}
