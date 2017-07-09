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
 * Date: 2016-8-14
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Represents an immunization recommendation handler.
	/// </summary>
	public class ImmunizationRecommendationResourceHandler : ResourceHandlerBase<ImmunizationRecommendation, SubstanceAdministration>
	{
		/// <summary>
		/// The repository.
		/// </summary>
		private IActRepositoryService repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImmunizationRecommendationResourceHandler"/> class.
		/// </summary>
		public ImmunizationRecommendationResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IActRepositoryService>();
		}

		/// <summary>
		/// Creates the specified model instance.
		/// </summary>
		/// <param name="modelInstance">The model instance.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the created model.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected override SubstanceAdministration Create(SubstanceAdministration modelInstance, List<IResultDetail> issues, TransactionMode mode)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Deletes the specified model identifier.
		/// </summary>
		/// <param name="modelId">The model identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the deleted model.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected override SubstanceAdministration Delete(Guid modelId, List<IResultDetail> details)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Maps the outbound resource to FHIR.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns the mapped FHIR resource.</returns>
		protected override ImmunizationRecommendation MapToFhir(SubstanceAdministration model, WebOperationContext webOperationContext)
		{
			ImmunizationRecommendation retVal = new ImmunizationRecommendation();

			retVal.Id = model.Key.ToString();
			retVal.Timestamp = DateTime.Now;
			retVal.Identifier = model.Identifiers.Select(o => DataTypeConverter.ToFhirIdentifier(o)).ToList();

			var rct = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget).PlayerEntity;
			if (rct != null)
				retVal.Patient = Reference.CreateResourceReference(new Patient() { Id = model.Key.ToString(), VersionId = model.VersionKey.ToString() }, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);

			var mat = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Product).PlayerEntity;

			// Recommend
			string status = (model.StopTime ?? model.ActTime) < DateTimeOffset.Now ? "overdue" : "due";
			var recommendation = new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendation()
			{
				Date = model.CreationTime.DateTime,
				DoseNumber = model.SequenceId,
				VaccineCode = DataTypeConverter.ToFhirCodeableConcept(mat?.TypeConcept),
				ForecastStatus = new FhirCodeableConcept(new Uri("http://hl7.org/fhir/conceptset/immunization-recommendation-status"), status),
				DateCriterion = new List<MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendationDateCriterion>()
				{
					new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendationDateCriterion()
					{
						Code = new FhirCodeableConcept(new Uri("http://hl7.org/fhir/conceptset/immunization-recommendation-date-criterion"), "recommended"),
						Value = model.ActTime.DateTime
					}
				}
			};
			if (model.StartTime.HasValue)
				recommendation.DateCriterion.Add(new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendationDateCriterion()
				{
					Code = new FhirCodeableConcept(new Uri("http://hl7.org/fhir/conceptset/immunization-recommendation-date-criterion"), "earliest"),
					Value = model.StartTime.Value.DateTime
				});
			if (model.StopTime.HasValue)
				recommendation.DateCriterion.Add(new MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendationDateCriterion()
				{
					Code = new FhirCodeableConcept(new Uri("http://hl7.org/fhir/conceptset/immunization-recommendation-date-criterion"), "overdue"),
					Value = model.StopTime.Value.DateTime
				});

			retVal.Recommendation = new List<MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone.ImmunizationRecommendation>() { recommendation };
			return retVal;
		}

		/// <summary>
		/// Maps a FHIR resource to a model instance.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <returns>Returns the mapped model.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected override SubstanceAdministration MapToModel(ImmunizationRecommendation resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Query for immunization recommendations.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns the list of models which match the given parameters.</returns>
		protected override IEnumerable<SubstanceAdministration> Query(Expression<Func<SubstanceAdministration, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
		{
			// TODO: Hook this up to the forecaster
			var obsoletionReference = Expression.MakeBinary(ExpressionType.NotEqual, Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(BaseEntityData.ObsoletionTime))), Expression.Constant(null));
			query = Expression.Lambda<Func<SubstanceAdministration, bool>>(Expression.AndAlso(obsoletionReference, query), query.Parameters);

			return this.repository.Find<SubstanceAdministration>(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Reads the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the model which matches the given id.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected override SubstanceAdministration Read(Identifier<Guid> id, List<IResultDetail> details)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates the specified model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="details">The details.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the updated model.</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		protected override SubstanceAdministration Update(SubstanceAdministration model, List<IResultDetail> details, TransactionMode mode)
		{
			throw new NotSupportedException();
		}
	}
}