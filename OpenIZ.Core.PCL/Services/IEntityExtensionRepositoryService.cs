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
 * Date: 2017-4-10
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;

namespace OpenIZ.Core.Services
{
	/// <summary>
	/// Represents an entity extension repository service.
	/// </summary>
	public interface IEntityExtensionRepositoryService

	{
		/// <summary>
		/// Finds an entity extension for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>Returns a list of entity extensions.</returns>
		IEnumerable<EntityExtension> Find(Expression<Func<EntityExtension, bool>> expression);

		/// <summary>
		/// Finds an entity extension for a specified expression.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalCount">The total count.</param>
		/// <returns>Returns a list of entity extensions.</returns>
		IEnumerable<EntityExtension> Find(Expression<Func<EntityExtension, bool>> expression, int offset, int? count, out int totalCount);

		/// <summary>
		/// Gets the entity extension.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="versionId">The version identifier.</param>
		/// <returns>Returns an entity extension or null of no entity extension is found.</returns>
		EntityExtension Get(Guid id, Guid versionId);

		/// <summary>
		/// Inserts the specified entity extension.
		/// </summary>
		/// <param name="entityExtension">The entity extension.</param>
		/// <returns>Returns the inserted entity extension.</returns>
		EntityExtension Insert(EntityExtension entityExtension);

		/// <summary>
		/// Obsoletes the specified entity extension.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Returns the obsoleted entity extension.</returns>
		EntityExtension Obsolete(Guid id);

		/// <summary>
		/// Saves the specified entity extension.
		/// </summary>
		/// <param name="entityExtension">The entity extension.</param>
		/// <returns>Returns the saved entity extension.</returns>
		EntityExtension Save(EntityExtension entityExtension);
	}
}
