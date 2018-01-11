/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace OpenIZ.Core.Services.Impl
{
	public class LocalProviderRepositoryService : LocalEntityRepositoryServiceBase, IProviderRepositoryService, IRepositoryService<Provider>
    {
		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate)
		{
            int t = 0;
            return this.Find(predicate, 0, null, out t);
		}

		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <param name="count">The count of the providers to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate, int offset, int? count, out int totalCount)
		{
            return base.Find(predicate, offset, count, out totalCount, Guid.Empty);
		}

		/// <summary>
		/// Get the provider based on the identity
		/// </summary>
		public Provider Get(IIdentity identity)
		{
            int t = 0;
			return base.Find<Provider>(o=>o.Relationships.Any(g=>(g.TargetEntity as UserEntity).SecurityUser.UserName == identity.Name), 0, 1, out t, Guid.Empty).FirstOrDefault();
		}

		/// <summary>
		/// Gets the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider.</param>
		/// <param name="versionId">The version id of the provider.</param>
		/// <returns>Returns the specified provider.</returns>
		public Provider Get(Guid id, Guid versionId)
		{
            return base.Get<Provider>(id, versionId);
		}

        /// <summary>
        /// Gets the specified provider.
        /// </summary>
        /// <param name="id">The id of the provider.</param>
        /// <returns>Returns the specified provider.</returns>
        public Provider Get(Guid id)
        {
            return base.Get<Provider>(id, Guid.Empty);
        }

        /// <summary>
        /// Inserts the specified provider.
        /// </summary>
        /// <param name="provider">The provider to insert.</param>
        /// <returns>Returns the inserted provider.</returns>
        public Provider Insert(Provider provider)
		{
            return base.Insert(provider);
		}

		/// <summary>
		/// Obsoletes the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider to obsolete.</param>
		/// <returns>Returns the obsoleted provider.</returns>
		public Provider Obsolete(Guid id)
		{
			return base.Obsolete<Provider>(id);
		}

		/// <summary>
		/// Saves the specified provider.
		/// </summary>
		/// <param name="provider">The provider to save.</param>
		/// <returns>Returns the saved provider.</returns>
		public Provider Save(Provider provider)
		{
			return base.Save(provider);
		}
	}
}