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
 * Date: 2017-2-28
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Exceptions;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Core.Services.Impl
{
	/// <summary>
	/// Represents an act repository service.
	/// </summary>
	public partial class LocalActRepositoryService
	{

		/// <summary>
		/// Finds a list of substance administrations.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query)
		{
			int tr = 0;
			return this.Find<SubstanceAdministration>(query, 0, null, out tr);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<SubstanceAdministration> Find(Expression<Func<SubstanceAdministration, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<SubstanceAdministration>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		SubstanceAdministration IRepositoryService<SubstanceAdministration>.Get(Guid key)
		{
			return this.Get<SubstanceAdministration>(key, Guid.Empty);
		}

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        SubstanceAdministration IRepositoryService<SubstanceAdministration>.Get(Guid key, Guid versionKey)
        {
            return this.Get<SubstanceAdministration>(key, versionKey);
        }

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>TModel.</returns>
        public SubstanceAdministration Insert(SubstanceAdministration data)
		{
			return this.Insert<SubstanceAdministration>(data);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		SubstanceAdministration IRepositoryService<SubstanceAdministration>.Obsolete(Guid key)
		{
			return this.Obsolete<SubstanceAdministration>(key);
		}

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		public SubstanceAdministration Save(SubstanceAdministration data)
		{
			return this.Save<SubstanceAdministration>(data);
		}
    }
}