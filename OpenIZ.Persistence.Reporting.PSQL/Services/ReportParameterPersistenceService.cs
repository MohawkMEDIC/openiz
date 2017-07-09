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

using OpenIZ.Core.Model.RISI;
using OpenIZ.Core.Model.RISI.Constants;
using OpenIZ.OrmLite;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.PSQL.Services
{
	/// <summary>
	/// Represents a report persistence service.
	/// </summary>
	public class ReportParameterPersistenceService : CorePersistenceService<ReportParameter, PSQL.Model.ReportParameter, PSQL.Model.ReportParameter>
	{
		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override object FromModelInstance(ReportParameter modelInstance, DataContext context, IPrincipal principal)
		{
			if (modelInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Model instance is null, exiting map");
				return null;
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(PSQL.Model.ReportParameter) } to { nameof(ReportParameter) }");

			var domainInstance = base.FromModelInstance(modelInstance, context, principal) as Model.ReportParameter;

			if (modelInstance.ReportDefinitionKey != Guid.Empty)
			{
				domainInstance.ReportId = modelInstance.ReportDefinitionKey;
			}
			else
			{
				throw new InvalidOperationException("Cannot insert report parameter without report id");
			}

			domainInstance.ParameterTypeId = modelInstance.ParameterType?.Key ?? ParameterTypeKeys.Object;

			return domainInstance;
		}

		/// <summary>
		/// Gets a report parameter by correlation id.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="correlationId">The correlation identifier.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns a report parameter for a given correlation id.</returns>
		public ReportParameter Get(DataContext context, string correlationId, IPrincipal principal)
		{
			int totalResults;
			return this.Query(context, r => r.CorrelationId == correlationId, 0, 1, out totalResults, false, principal).FirstOrDefault();
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override ReportParameter ToModelInstance(object domainInstance, DataContext context, IPrincipal principal)
		{
			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Domain instance is null, exiting mapper");
				return null;
			}

			if (!(domainInstance is PSQL.Model.ReportParameter))
			{
				throw new ArgumentException($"Invalid type: {nameof(domainInstance)} is not of type {nameof(PSQL.Model.ReportParameter)}");
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportParameter) } to { nameof(PSQL.Model.ReportParameter) }");

			var modelInstance = base.ToModelInstance(domainInstance, context, principal);

			modelInstance.ParameterType = new ParameterType(((Model.ReportParameter)domainInstance).ParameterTypeId);

			return modelInstance;
		}
	}
}