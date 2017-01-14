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
 * Date: 2017-1-12
 */

using OpenIZ.Core.Model.Map;
using OpenIZ.Persistence.Reporting.Context;

namespace OpenIZ.Persistence.Reporting.Services
{
	/// <summary>
	/// Represents a base class for report persistence.
	/// </summary>
	internal abstract class ReportPersistenceServiceBase<TModel, TDomain>
	{
		/// <summary>
		/// Gets the model mapper.
		/// </summary>
		protected static ModelMapper ModelMapper => ReportingService.ModelMapper;

		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal abstract TDomain FromModelInstance(TModel modelInstance);

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <returns>Returns the converted model instance.</returns>
		internal abstract TModel ToModelInstance(TDomain domainInstance);

		/// <summary>
		/// Loads the relations for a given domain instance.
		/// </summary>
		/// <param name="context">The application database context.</param>
		/// <param name="domainInstance">The domain instance for which the load the relations.</param>
		/// <returns>Returns the updated domain instance.</returns>
		protected abstract TDomain LoadRelations(ApplicationDbContext context, TDomain domainInstance);
	}
}