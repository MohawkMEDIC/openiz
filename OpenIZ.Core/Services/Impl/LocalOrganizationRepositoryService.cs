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
 * Date: 2016-8-27
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Provides operations for managing organizations.
	/// </summary>
	public class LocalOrganizationRepositoryService : IOrganizationRepositoryService
	{
		/// <summary>
		/// Searches for a organization using a given query.
		/// </summary>
		/// <param name="query">The predicate to use for searching for the organization.</param>
		/// <returns>Returns a list of organizations who match the specified query.</returns>
		public IEnumerable<Organization> Find(Expression<Func<Organization, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(query, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Searches for a Organization using a given query.
		/// </summary>
		/// <param name="query">The predicate to use for searching for the organization.</param>
		/// <param name="count">The count of the organizations to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of organizations who match the specified query.</returns>
		public IEnumerable<Organization> Find(Expression<Func<Organization, bool>> query, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization.</param>
		/// <param name="versionId">The version id of the organization.</param>
		/// <returns>Returns the specified organization.</returns>
		public Organization Get(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Inserts the specified organization.
		/// </summary>
		/// <param name="organization">The organization to insert.</param>
		/// <returns>Returns the inserted organization.</returns>
		public Organization Insert(Organization organization)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Insert(organization, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes the specified organization.
		/// </summary>
		/// <param name="id">The id of the organization to obsolete.</param>
		/// <returns>Returns the obsoleted organization.</returns>
		public Organization Obsolete(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Obsolete(new Organization() { Key = id }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Saves the specified organization.
		/// </summary>
		/// <param name="organization">The organization to save.</param>
		/// <returns>Returns the saved organization.</returns>
		public Organization Save(Organization organization)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Organization>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			try
			{
				return persistenceService.Update(organization, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (KeyNotFoundException)
			{
				return persistenceService.Insert(organization, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}