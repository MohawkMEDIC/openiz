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
 * User: khannan
 * Date: 2017-1-3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Security;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a repository service for managing persons.
	/// </summary>
	public class LocalPersonRepositoryService : IPersonRepositoryService
	{
		/// <summary>
		/// Searches for a person using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the person.</param>
		/// <returns>Returns a list of persons who match the specified predicate.</returns>
		public IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate)
		{
			int totalCount = 0;
			return this.Find(predicate, 0, null, out totalCount);
		}

		/// <summary>
		/// Searches for a person using a given predicate.
		/// </summary>
		/// <param name="predicate">The predicate to use for searching for the person.</param>
		/// <param name="count">The count of the persons to return.</param>
		/// <param name="offset">The offset for the search results.</param>
		/// <param name="totalCount">The total count of the search results.</param>
		/// <returns>Returns a list of persons who match the specified predicate.</returns>
		public IEnumerable<Person> Find(Expression<Func<Person, bool>> predicate, int offset, int? count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Person>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Person>)} not found");
			}

			return persistenceService.Query(predicate, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Gets the specified person.
		/// </summary>
		/// <param name="id">The id of the person.</param>
		/// <param name="versionId">The version id of the person.</param>
		/// <returns>Returns the specified person.</returns>
		public Person Get(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Person>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Person>)} not found");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id, versionId), AuthenticationContext.Current.Principal, true);
		}

		/// <summary>
		/// Get the person based off the user identity
		/// </summary>
		public Person Get(IIdentity identity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Inserts the specified person.
		/// </summary>
		/// <param name="person">The person to insert.</param>
		/// <returns>Returns the inserted person.</returns>
		public Person Insert(Person person)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Person>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Person>)} not found");
			}

			return persistenceService.Insert(person, AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// Obsoletes the specified person.
		/// </summary>
		/// <param name="id">The id of the person to obsolete.</param>
		/// <returns>Returns the obsoleted person.</returns>
		public Person Obsolete(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Person>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Person>)} not found");
			}

			return persistenceService.Obsolete(this.Get(id, Guid.Empty), AuthenticationContext.Current.Principal, TransactionMode.Commit);
		}

		/// <summary>
		/// /// Saves the specified person.
		/// </summary>
		/// <param name="person">The person to save.</param>
		/// <returns>Returns the saved person.</returns>
		public Person Save(Person person)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Person>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"{nameof(IDataPersistenceService<Person>)} not found");
			}

			try
			{
				return persistenceService.Update(person, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
			catch (DataPersistenceException)
			{
				return persistenceService.Insert(person, AuthenticationContext.Current.Principal, TransactionMode.Commit);
			}
		}
	}
}
