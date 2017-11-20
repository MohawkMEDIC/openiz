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
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Reporting.PSQL.Services
{
	/// <summary>
	/// Represents a report format persistence service.
	/// </summary>
	public class ReportFormatPersistenceService : CorePersistenceService<ReportFormat, PSQL.Model.ReportFormat, PSQL.Model.ReportFormat>
	{
		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override object FromModelInstance(ReportFormat modelInstance, DataContext context, IPrincipal principal)
		{
			if (modelInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Model instance is null, exiting map");
				return null;
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(PSQL.Model.ReportFormat) } to { nameof(ReportFormat) }");

			return base.FromModelInstance(modelInstance, context, principal);
		}

		/// <summary>
		/// Inserts the model.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the inserted model.</returns>
		/// <exception cref="DuplicateNameException">If the report format already exists</exception>
		public override ReportFormat InsertInternal(DataContext context, ReportFormat model, IPrincipal principal)
		{
			int totalResults;

			var result = this.QueryInternal(context, o => o.Format == model.Format, 0, null, out totalResults, false, principal).FirstOrDefault();

			if (result == null)
			{
				return base.InsertInternal(context, model, principal);
			}

			throw new DuplicateNameException($"Cannot insert report format: {model.Format} because it already exists");
		}

		/// <summary>
		/// Obsoletes the specified data.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the obsoleted data.</returns>
		/// <exception cref="System.InvalidOperationException">Cannot obsolete report format which is currently in use</exception>
		public override ReportFormat ObsoleteInternal(DataContext context, ReportFormat model, IPrincipal principal)
		{
			var reportDefinitionService = ApplicationContext.Current.GetService<IDataPersistenceService<ReportDefinition>>();

			var results = reportDefinitionService.Query(r => r.Formats.Any(f => f.Format == model.Format), principal);

			if (!results.Any())
			{
				return base.ObsoleteInternal(context, model, principal);
			}

			throw new InvalidOperationException("Cannot obsolete report format which is currently in use");
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override ReportFormat ToModelInstance(object domainInstance, DataContext context, IPrincipal principal)
		{
			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Domain instance is null, exiting mapper");
				return null;
			}

			if (!(domainInstance is PSQL.Model.ReportFormat))
			{
				throw new ArgumentException($"Invalid type: {nameof(domainInstance)} is not of type {nameof(PSQL.Model.ReportFormat)}");
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportFormat) } to { nameof(PSQL.Model.ReportFormat) }");

			return base.ToModelInstance(domainInstance, context, principal);
		}
	}
}