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
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query)
		{
			int tr = 0;
			return this.Find<QuantityObservation>(query, 0, null, out tr);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<QuantityObservation> Find(Expression<Func<QuantityObservation, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<QuantityObservation>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query)
		{
			int tr = 0;
			return this.Find<CodedObservation>(query, 0, null, out tr);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<CodedObservation> Find(Expression<Func<CodedObservation, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<CodedObservation>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query)
		{
			int tr = 0;
			return this.Find<TextObservation>(query, 0, null, out tr);
		}

		/// <summary>
		/// Finds the specified data.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns a list of identified data.</returns>
		public IEnumerable<TextObservation> Find(Expression<Func<TextObservation, bool>> query, int offset, int? count, out int totalResults)
		{
			return this.Find<TextObservation>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		QuantityObservation IRepositoryService<QuantityObservation>.Get(Guid key)
		{
			return this.Get<QuantityObservation>(key, Guid.Empty);
		}

		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		CodedObservation IRepositoryService<CodedObservation>.Get(Guid key)
		{
			return this.Get<CodedObservation>(key, Guid.Empty);
		}

		/// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		TextObservation IRepositoryService<TextObservation>.Get(Guid key)
		{
			return this.Get<TextObservation>(key, Guid.Empty);
		}

        /// <summary>
		/// Gets the specified model.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		QuantityObservation IRepositoryService<QuantityObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<QuantityObservation>(key, versionKey);
        }

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        CodedObservation IRepositoryService<CodedObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<CodedObservation>(key, versionKey);
        }

        /// <summary>
        /// Gets the specified model.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the model.</returns>
        TextObservation IRepositoryService<TextObservation>.Get(Guid key, Guid versionKey)
        {
            return this.Get<TextObservation>(key, versionKey);
        }

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>TModel.</returns>
        public QuantityObservation Insert(QuantityObservation data)
		{
			return this.Insert<QuantityObservation>(data);
		}

		/// <summary>
		/// Inserts the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>TModel.</returns>
		public CodedObservation Insert(CodedObservation data)
		{
			return this.Insert<CodedObservation>(data);
		}

		/// <summary>
		/// Inserts the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>TModel.</returns>
		public TextObservation Insert(TextObservation data)
		{
			return this.Insert<TextObservation>(data);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		QuantityObservation IRepositoryService<QuantityObservation>.Obsolete(Guid key)
		{
			return this.Obsolete<QuantityObservation>(key);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		CodedObservation IRepositoryService<CodedObservation>.Obsolete(Guid key)
		{
			return this.Obsolete<CodedObservation>(key);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>Returns the model.</returns>
		TextObservation IRepositoryService<TextObservation>.Obsolete(Guid key)
		{
			return this.Obsolete<TextObservation>(key);
		}

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		public QuantityObservation Save(QuantityObservation data)
		{
			return this.Save<QuantityObservation>(data);
		}

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		public CodedObservation Save(CodedObservation data)
		{
			return this.Save<CodedObservation>(data);
		}

		/// <summary>
		/// Saves the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Returns the model.</returns>
		public TextObservation Save(TextObservation data)
		{
			return this.Save<TextObservation>(data);
		}
    }
}