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
 * User: khannan
 * Date: 2016-8-30
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// Represents a resource handler for entities.
	/// </summary>
	public class EntityResourceHandler : IResourceHandler
	{
		private IEntityRepositoryService repositoryService;

		public EntityResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repositoryService = ApplicationContext.Current.GetService<IEntityRepositoryService>();
		}
		/// <summary>
		/// Gets the resource name of the resource handler.
		/// </summary>
		public string ResourceName
		{
			get
			{
				return "Entity";
			}
		}

		/// <summary>
		/// Gets the type of the resource handler.
		/// </summary>
		public Type Type
		{
			get
			{
				return typeof(Entity);
			}
		}

		/// <summary>
		/// Creates an entity.
		/// </summary>
		/// <param name="data">The entity to be created.</param>
		/// <param name="updateIfExists">Whether to update the entity if it exits.</param>
		/// <returns>Returns the created entity.s</returns>
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Bundle bundle = data as Bundle;

			bundle?.Reconstitute();

			var processData = bundle?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have an entry point");
			}
			else if (processData is Entity)
			{
				var entity = processData as Entity;

				if (updateIfExists)
				{
					return this.repositoryService.Save(entity);
				}
				else
				{
					return this.repositoryService.Insert(entity);
				}
			}
			else
			{
				throw new ArgumentException(nameof(data), "Invalid data type");
			}
		}

		/// <summary>
		/// Gets an entity by id and version id.
		/// </summary>
		/// <param name="id">The id of the entity.</param>
		/// <param name="versionId">The version id of the entity.</param>
		/// <returns>Returns the entity.</returns>
		public IdentifiedData Get(Guid id, Guid versionId)
		{
			return this.repositoryService.Get(id, versionId);
		}

		/// <summary>
		/// Obsoletes an entity.
		/// </summary>
		/// <param name="key">The key of the entity to be obsoleted.</param>
		/// <returns>Returns the obsoleted entity.</returns>
		public IdentifiedData Obsolete(Guid key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for an entity.
		/// </summary>
		/// <param name="queryParameters">The query parameters to use to search for the entity.</param>
		/// <returns>Returns a list of entities.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries for an entity.
		/// </summary>
		/// <param name="queryParameters">The query parameters to use to search for the entity.</param>
		/// <param name="offset">The offset of the query.</param>
		/// <param name="count">The count of the query.</param>
		/// <param name="totalCount">The total count of the query.</param>
		/// <returns>Returns a list of entities.</returns>
		public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates an entity.
		/// </summary>
		/// <param name="data">The entity to be updated.</param>
		/// <returns>Returns the updated entity.</returns>
		public IdentifiedData Update(IdentifiedData data)
		{
			throw new NotImplementedException();
		}
	}
}
