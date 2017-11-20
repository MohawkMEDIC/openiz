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
 * Date: 2017-4-16
 */

using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.RISI;
using OpenIZ.OrmLite;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.PSQL.Services
{
	/// <summary>
	/// Represents a data type persistence service.
	/// </summary>
	public class ParameterTypePersistenceService : CorePersistenceService<ParameterType, PSQL.Model.ParameterType, PSQL.Model.ParameterType>
	{
		/// <summary>
		/// Maps a <see cref="ParameterType" /> instance to a <see cref="PSQL.Model.ParameterType" /> instance.
		/// </summary>
		/// <param name="modelInstance">The model instance.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the mapped parameter type instance.</returns>
		public override object FromModelInstance(ParameterType modelInstance, DataContext context, IPrincipal principal)
		{
			if (modelInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Model instance is null, exiting map");
				return null;
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(PSQL.Model.ParameterType) } to { nameof(ParameterType) }");

			return base.FromModelInstance(modelInstance, context, principal);
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the obsoleted data.</returns>
		/// <exception cref="System.InvalidOperationException">Cannot obsolete report format which is currently in use</exception>
		public override ParameterType ObsoleteInternal(DataContext context, ParameterType model, IPrincipal principal)
		{
			var parameterTypeService = ApplicationContext.Current.GetService<IDataPersistenceService<ReportParameter>>();

			var results = parameterTypeService.Query(r => r.ParameterType.Key == model.Key, principal);

			if (!results.Any())
			{
				return base.ObsoleteInternal(context, model, principal);
			}

			throw new InvalidOperationException("Cannot obsolete parameter type which is currently in use");
		}

		/// <summary>
		/// Maps a <see cref="PSQL.Model.ParameterType" /> instance to an <see cref="ParameterType" /> instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the mapped parameter type instance.</returns>
		/// <exception cref="System.ArgumentException">If the domain instance is not of the correct type</exception>
		public override ParameterType ToModelInstance(object domainInstance, DataContext context, IPrincipal principal)
		{
			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Domain instance is null, exiting mapper");
				return null;
			}

			if (!(domainInstance is PSQL.Model.ParameterType))
			{
				throw new ArgumentException($"Invalid type: {nameof(domainInstance)} is not of type {nameof(PSQL.Model.ParameterType)}");
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ParameterType) } to { nameof(PSQL.Model.ParameterType) }");

			return base.ToModelInstance(domainInstance, context, principal);
		}
	}
}