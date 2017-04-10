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
 * Date: 2016-8-2
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MARC.HI.EHRS.SVC.Core.Data;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents a local metadata repository service
	/// </summary>
	public class LocalMetadataRepositoryService : IMetadataRepositoryService
	{
		/// <summary>
		/// Find an assigning authority
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public IEnumerable<AssigningAuthority> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query)
		{
			var totalResults = 0;
			return this.FindAssigningAuthority(query, 0, 100, out totalResults);
		}

		/// <summary>
		/// Find assigning authority
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of assigning authorities.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public IEnumerable<AssigningAuthority> FindAssigningAuthority(Expression<Func<AssigningAuthority, bool>> query, int offset, int count, out int totalCount)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<AssigningAuthority>)}");
			}

			return persistenceService.Query(query, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Finds an extension type for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of extension types.</returns>
		public IEnumerable<ExtensionType> FindExtensionType(Expression<Func<ExtensionType, bool>> expression)
		{
			var totalCount = 0;
			return this.FindExtensionType(expression, 0, null, out totalCount);
		}

		/// <summary>
		/// Finds an extension type for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of extension types.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public IEnumerable<ExtensionType> FindExtensionType(Expression<Func<ExtensionType, bool>> expression, int offset, int? count, out int totalCount)
		{
			var extensionTypePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ExtensionType>>();

			if (extensionTypePersistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<ExtensionType>)}");
			}

			return extensionTypePersistenceService.Query(expression, offset, count, AuthenticationContext.Current.Principal, out totalCount);
		}

		/// <summary>
		/// Get the assigning authority
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>AssigningAuthority.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public AssigningAuthority GetAssigningAuthority(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<AssigningAuthority>)}");
			}

			return persistenceService.Get<Guid>(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, false);
		}

		/// <summary>
		/// Get the assigning authority
		/// </summary>
		/// <param name="assigningAutUri">The assigning aut URI.</param>
		/// <returns>AssigningAuthority.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public AssigningAuthority GetAssigningAuthority(Uri assigningAutUri)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();

			if (persistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<AssigningAuthority>)}");
			}

			if (assigningAutUri.Scheme == "urn" && assigningAutUri.LocalPath.StartsWith("oid:"))
			{
				var aaOid = assigningAutUri.LocalPath.Substring(4);
				return persistenceService.Query(o => o.Oid == aaOid, AuthenticationContext.Current.Principal).FirstOrDefault();
			}

			return persistenceService.Query(o => o.Url == assigningAutUri.OriginalString, AuthenticationContext.Current.Principal).FirstOrDefault();
		}

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns an extension type or null of no extension type is found.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public ExtensionType GetExtensionType(Guid id)
		{
			var extensionTypePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ExtensionType>>();

			if (extensionTypePersistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<ExtensionType>)}");
			}

			return extensionTypePersistenceService.Get(new Identifier<Guid>(id), AuthenticationContext.Current.Principal, true);
		}

		/// <summary>
		/// Gets the extension type.
		/// </summary>
		/// <param name="value">The URI of the extension.</param>
		/// <returns>Returns an extension type or null of no extension type is found.</returns>
		/// <exception cref="System.InvalidOperationException">Unable to locate persistence service</exception>
		public ExtensionType GetExtensionType(Uri value)
		{
			var extensionTypePersistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ExtensionType>>();

			if (extensionTypePersistenceService == null)
			{
				throw new InvalidOperationException($"Unable to locate persistence service:{nameof(IDataPersistenceService<ExtensionType>)}");
			}

			return extensionTypePersistenceService.Query(e => e.Name == value.ToString(), AuthenticationContext.Current.Principal).FirstOrDefault();
		}
	}
}