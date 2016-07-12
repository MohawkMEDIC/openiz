/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-12
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Roles;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Security;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
	internal class LocalProviderRepositoryService : IProviderRepositoryService
	{
		/// <summary>
		/// Searches for a provider using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the provider.</param>
		/// <returns>Returns a list of providers who match the specified predicate.</returns>
		public IEnumerable<Provider> Find(Expression<Func<Provider, bool>> predicate)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
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
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider.</param>
		/// <param name="versionId">The version id of the provider.</param>
		/// <returns>Returns the specified provider.</returns>
		public Provider Get(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Inserts the specified provider.
		/// </summary>
		/// <param name="provider">The provider to insert.</param>
		/// <returns>Returns the inserted provider.</returns>
		public Provider Insert(Provider provider)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Insert(provider, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes the specified provider.
		/// </summary>
		/// <param name="id">The id of the provider to obsolete.</param>
		/// <returns>Returns the obsoleted provider.</returns>
		public Provider Obsolete(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Obsolete(new Provider() { Key = id }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Saves the specified provider.
		/// </summary>
		/// <param name="provider">The provider to save.</param>
		/// <returns>Returns the saved provider.</returns>
		public Provider Save(Provider provider)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Provider>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			try
			{
				return persistenceService.Update(provider, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (KeyNotFoundException)
			{
				return persistenceService.Insert(provider, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}
