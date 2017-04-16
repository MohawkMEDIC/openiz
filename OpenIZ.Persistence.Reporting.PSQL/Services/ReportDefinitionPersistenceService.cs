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
 * User: Nityan
 * Date: 2017-4-15
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Entities;
using OpenIZ.OrmLite;
using OpenIZ.Persistence.Reporting.PSQL.Model;
using ReportDefinition = OpenIZ.Core.Model.RISI.ReportDefinition;

namespace OpenIZ.Persistence.Reporting.PSQL.Services
{
	/// <summary>
	/// Represents a ReportDefinition persistence service.
	/// </summary>
	public class ReportDefinitionPersistenceService : CorePersistenceService<ReportDefinition, Model.ReportDefinition, Model.ReportDefinition>
	{
		/// <summary>
		/// Converts a model instance to a domain instance.
		/// </summary>
		/// <param name="modelInstance">The model instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override object FromModelInstance(ReportDefinition modelInstance, DataContext context, IPrincipal principal)
		{
			if (modelInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Model instance is null, exiting map");
				return null;
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(PSQL.Model.ReportDefinition) } to { nameof(ReportDefinition) }");

			return ModelMapper.MapModelInstance<ReportDefinition, PSQL.Model.ReportDefinition>(modelInstance);
		}

		/// <summary>
		/// Inserts the model.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the inserted model.</returns>
		/// <exception cref="System.InvalidOperationException">Domain instance must not be null</exception>
		public override ReportDefinition InsertInternal(DataContext context, ReportDefinition model, IPrincipal principal)
		{
			var domainInstance = this.FromModelInstance(model, context, principal) as Model.ReportDefinition;

			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, 0, "Domain instance must not be null");
				throw new InvalidOperationException("Domain instance must not be null");
			}

			if (domainInstance.Author == null)
			{
				domainInstance.Author = "SYSTEM";
			}

			domainInstance = context.Insert(domainInstance);

			UpdateReportFormats(context, principal, model);
			UpdateReportParameters(context, principal, model);

			return this.ToModelInstance(domainInstance, context, principal);
		}

		/// <summary>
		/// Converts a domain instance to a model instance.
		/// </summary>
		/// <param name="domainInstance">The domain instance to convert.</param>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the converted model instance.</returns>
		public override ReportDefinition ToModelInstance(object domainInstance, DataContext context, IPrincipal principal)
		{
			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Warning, 0, "Domain instance is null, exiting mapper");
				return null;
			}

			if (!(domainInstance is PSQL.Model.ReportDefinition))
			{
				throw new ArgumentException($"Invalid type: {nameof(domainInstance)} is not of type {nameof(PSQL.Model.ReportDefinition)}");
			}

			this.traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Mapping { nameof(ReportDefinition) } to { nameof(PSQL.Model.ReportDefinition) }");

			return ModelMapper.MapDomainInstance<PSQL.Model.ReportDefinition, ReportDefinition>((PSQL.Model.ReportDefinition)domainInstance);
		}

		/// <summary>
		/// Updates the specified storage data.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="model">The model.</param>
		/// <param name="principal">The principal.</param>
		/// <returns>Returns the updated model instance.</returns>
		/// <exception cref="System.InvalidOperationException">Domain instance must not be null</exception>
		public override ReportDefinition UpdateInternal(DataContext context, ReportDefinition model, IPrincipal principal)
		{
			var domainInstance = this.FromModelInstance(model, context, principal) as Model.ReportDefinition;

			if (domainInstance == null)
			{
				this.traceSource.TraceEvent(TraceEventType.Error, 0, "Domain instance must not be null");
				throw new InvalidOperationException("Domain instance must not be null");
			}

			if (domainInstance.Author == null)
			{
				domainInstance.Author = "SYSTEM";
			}

			domainInstance = context.Update(domainInstance);

			UpdateReportFormats(context, principal, model);
			UpdateReportParameters(context, principal, model);

			return this.ToModelInstance(domainInstance, context, principal);
		}

		/// <summary>
		/// Updates the report formats.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <param name="reportDefinition">The report definition.</param>
		private static void UpdateReportFormats(DataContext context, IPrincipal principal, ReportDefinition reportDefinition)
		{
			var reportDefinitionFormatService = ApplicationContext.Current.GetService<IDataPersistenceService<ReportDefinitionFormatAssociation>>();

			foreach (var reportFormat in reportDefinition.Formats)
			{
				var existing = context.Query<ReportDefinitionFormatAssociation>(c => c.Key == reportFormat.Key.Value).FirstOrDefault();

				if (existing == null)
				{
					reportDefinitionFormatService.Insert(new ReportDefinitionFormatAssociation(reportFormat.Key.Value, reportDefinition.Key.Value), principal, TransactionMode.Commit);
				}
				else
				{
					reportDefinitionFormatService.Update(new ReportDefinitionFormatAssociation(reportFormat.Key.Value, reportDefinition.Key.Value), principal, TransactionMode.Commit);
				}
			}
		}

		/// <summary>
		/// Updates the report parameters.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="principal">The principal.</param>
		/// <param name="reportDefinition">The report definition.</param>
		private static void UpdateReportParameters(DataContext context, IPrincipal principal, ReportDefinition reportDefinition)
		{
			var reportParameterPersistenceService = new ReportParameterPersistenceService();

			foreach (var reportParameter in reportDefinition.Parameters)
			{
				var existingReportParameter = reportParameterPersistenceService.Get(context, reportParameter.CorrelationId, principal);

				if (existingReportParameter == null)
				{
					reportParameterPersistenceService.Insert(context, reportParameter, principal);
				}
				else
				{
					reportParameterPersistenceService.Update(context, reportParameter, principal);
				}
			}
		}
	}
}