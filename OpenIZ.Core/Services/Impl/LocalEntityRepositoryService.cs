/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Model;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an entity repository service.
	/// </summary>
	public class LocalEntityRepositoryService : LocalEntityRepositoryServiceBase, IEntityRepositoryService, IRepositoryService<Entity>, IRepositoryService<EntityRelationship>
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
            int t = 0;
            return this.Find(query, 0, null, out t);
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
            return this.Find<Entity>(query, offSet, count, out totalCount, Guid.Empty);
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
            return base.Get<Entity>(key, versionKey);
		}

		/// <summary>
		/// Gets the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>TModel.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the persistence service is not found.</exception>
		EntityRelationship IRepositoryService<EntityRelationship>.Get(Guid key)
		{
            return base.Get<EntityRelationship>(key, Guid.Empty);
		}

		/// <summary>
		/// Inserts the specified data
		/// </summary>
		public EntityRelationship Insert(EntityRelationship data)
		{
            return base.Insert(data);
		}

		/// <summary>
		/// Inserts an entity.
		/// </summary>
		/// <param name="entity">The entity to be inserted.</param>
		/// <returns>Returns the inserted entity.</returns>
		public Entity Insert(Entity entity)
		{
            return base.Insert(entity);
		}

		/// <summary>
		/// Obsoletes the specified data
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		EntityRelationship IRepositoryService<EntityRelationship>.Obsolete(Guid key)
		{
            return base.Obsolete<EntityRelationship>(key);
		}

		/// <summary>
		/// Obsoletes an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be obsoleted.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the entity is not found.</exception>
		public Entity Obsolete(Guid key)
		{
            return base.Obsolete<Entity>(key);
		}

		/// <summary>
		/// Saves the specified data
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>TModel.</returns>
		/// <exception cref="System.InvalidOperationException">Thrown if the persistence service is not found.</exception>
		public EntityRelationship Save(EntityRelationship data)
		{
            return base.Save(data);
		}

		/// <summary>
		/// Saves or inserts an entity.
		/// </summary>
		/// <param name="entity">The entity to be saved.</param>
		/// <returns>Returns the saved entity.</returns>
		public Entity Save(Entity entity)
		{
            return base.Save(entity);
		}
    }
}
