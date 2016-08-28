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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Provides operations for managing identifier types.
	/// </summary>
	public class LocalIdentifierTypeRepositoryService : IIdentifierTypeRepositoryService
	{
		/// <summary>
		/// Searches for an Identifier Type using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the identifier type.</param>
		/// <returns>Returns a list of identifier types who match the specified predicate.</returns>
		public IEnumerable<IdentifierType> Find(Expression<Func<IdentifierType, bool>> predicate)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, AuthenticationContext.Current.Principal);
		}

		/// <summary>
		/// Searches for an Identifier Type using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the organization.</param>
		/// <param name="count">The count of the organizations to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of identifier types who match the specified predicate.</returns>
		public IEnumerable<IdentifierType> Find(Expression<Func<IdentifierType, bool>> predicate, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets the specified identifier type.
		/// </summary>
		/// <param name="id">The id of the identifier type.</param>
		/// <param name="versionId">The version id of the identifier type.</param>
		/// <returns>Returns the specified identifier type.</returns>
		public IdentifierType Get(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Inserts the specified identifier type.
		/// </summary>
		/// <param name="identifierType">The identifier type to insert.</param>
		/// <returns>Returns the inserted organization.</returns>
		public IdentifierType Insert(IdentifierType identifierType)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Insert(identifierType, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes the specified identifier type.
		/// </summary>
		/// <param name="id">The id of the identifier type to obsolete.</param>
		/// <returns>Returns the obsoleted identifier type.</returns>
		public IdentifierType Obsolete(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			return persistenceService.Obsolete(new IdentifierType() { Key = id }, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Saves the specified identifier type.
		/// </summary>
		/// <param name="identifierType">The identifier type to save.</param>
		/// <returns>Returns the saved identifier type.</returns>
		public IdentifierType Save(IdentifierType identifierType)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException("No persistence service found");
			}

			try
			{
				return persistenceService.Update(identifierType, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (KeyNotFoundException)
			{
				return persistenceService.Insert(identifierType, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}