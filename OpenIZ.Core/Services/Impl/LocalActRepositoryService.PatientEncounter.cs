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
	/// <seealso cref="IActRepositoryService" />
	/// <seealso cref="Services.IRepositoryService{Act}" />
	/// <seealso cref="Services.IRepositoryService{SubstanceAdministration}" />
	/// <seealso cref="Services.IRepositoryService{QuantityObservation}" />
	/// <seealso cref="Services.IRepositoryService{PatientEncounter}" />
	/// <seealso cref="Services.IRepositoryService{CodedObservation}" />
	/// <seealso cref="Services.IRepositoryService{TextObservation}" />
	public partial class LocalActRepositoryService
	{
		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query)
		{
			int tr = 0;
			return this.Find<PatientEncounter>(query, 0, null, out tr);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<PatientEncounter> Find(Expression<Func<PatientEncounter, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<PatientEncounter>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		PatientEncounter IRepositoryService<PatientEncounter>.Get(Guid key)
		{
			return this.Get<PatientEncounter>(key, Guid.Empty);
		}

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        PatientEncounter IRepositoryService<PatientEncounter>.Get(Guid key, Guid versionKey)
        {
            return this.Get<PatientEncounter>(key, versionKey);
        }

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>TModel.</returns>
        public PatientEncounter Insert(PatientEncounter data)
		{
			return this.Insert<PatientEncounter>(data);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		PatientEncounter IRepositoryService<PatientEncounter>.Obsolete(Guid key)
		{
			return this.Obsolete<PatientEncounter>(key);
		}

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		public PatientEncounter Save(PatientEncounter data)
		{
			return this.Save<PatientEncounter>(data);
		}
    }
}