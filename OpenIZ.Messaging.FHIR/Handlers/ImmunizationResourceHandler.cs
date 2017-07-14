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
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
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
	/// Resource handler for immunization classes.
	/// </summary>
	public class ImmunizationResourceHandler : RepositoryResourceHandlerBase<Immunization, SubstanceAdministration>
	{
		/// <summary>
		/// Maps the substance administration to FHIR.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns the mapped FHIR resource.</returns>
		protected override Immunization MapToFhir(SubstanceAdministration model, WebOperationContext webOperationContext)
		{
			var retVal = DataTypeConverter.CreateResource<Immunization>(model);

			retVal.DoseQuantity = new FhirQuantity()
			{
				Units = model.DoseUnit?.Mnemonic,
				Value = new FhirDecimal(model.DoseQuantity)
			};
			retVal.Date = (FhirDate)model.ActTime.DateTime;
			retVal.Route = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>(nameof(SubstanceAdministration.Route)));
			retVal.Site = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>(nameof(SubstanceAdministration.Site)));
			retVal.Status = "completed";
			//retVal.SelfReported = model.Tags.Any(o => o.TagKey == "selfReported" && Convert.ToBoolean(o.Value));
			retVal.WasNotGiven = model.IsNegated;

			// Material
			var matPtcpt = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Consumable) ??
				model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Product);
			if (matPtcpt != null)
			{
				var matl = matPtcpt.LoadProperty<Material>(nameof(ActParticipation.PlayerEntity));
				retVal.VaccineCode = DataTypeConverter.ToFhirCodeableConcept(matl.LoadProperty<Concept>(nameof(Act.TypeConcept)));
				retVal.ExpirationDate = matl.ExpiryDate.HasValue ? (FhirDate)matl.ExpiryDate : null;
				retVal.LotNumber = (matl as ManufacturedMaterial)?.LotNumber;
			}
			else
				retVal.ExpirationDate = null;

			// RCT
			var rct = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
			if (rct != null)
			{
				retVal.Patient = DataTypeConverter.CreateReference<Patient>(rct.LoadProperty<Entity>("PlayerEntity"), webOperationContext);
			}

			// Performer
			var prf = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Performer);
			if (prf != null)
				retVal.Performer = DataTypeConverter.CreateReference<Practitioner>(rct.LoadProperty<Entity>("PlayerEntity"), webOperationContext);

			// Protocol
			foreach (var itm in model.Protocols)
			{
				ImmunizationProtocol protocol = new ImmunizationProtocol();
				var dbProtocol = itm.LoadProperty<Protocol>(nameof(ActProtocol.Protocol));
				protocol.DoseSequence = new FhirInt((int)model.SequenceId);

				// Protocol lookup
				protocol.Series = dbProtocol?.Name;
				retVal.VaccinationProtocol.Add(protocol);
			}
			if (retVal.VaccinationProtocol.Count == 0)
				retVal.VaccinationProtocol.Add(new ImmunizationProtocol() { DoseSequence = (int)model.SequenceId });

			var loc = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Location);
			if (loc != null)
				retVal.Extension.Add(new Extension()
				{
					Url = "http://openiz.org/extensions/act/fhir/location",
					Value = new FhirString(loc.PlayerEntityKey.ToString())
				});

			return retVal;
		}

		/// <summary>
		/// Map an immunization FHIR resource to a substance administration.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <returns>Returns the mapped model.</returns>
		protected override SubstanceAdministration MapToModel(Immunization resource, WebOperationContext webOperationContext)
		{
			var substanceAdministration = new SubstanceAdministration
			{
				ActTime = resource.Date.DateValue.Value,
				DoseQuantity = resource.DoseQuantity.Value.Value.Value,
				Extensions = resource.Extension?.Select(DataTypeConverter.ToActExtension).ToList(),
				Identifiers = resource.Identifier?.Select(DataTypeConverter.ToActIdentifier).ToList(),
				Key = Guid.NewGuid(),
				MoodConceptKey = DataTypeConverter.ToConcept<string>(resource.Status, "http://hl7.org/fhir/medication-admin-status")?.Key,
				ReasonConceptKey = DataTypeConverter.ToConcept<string>(resource.Status, "http://snomed.info/sct")?.Key,
				RouteKey = DataTypeConverter.ToConcept<string>(resource.Status, "http://hl7.org/fhir/v3/RouteOfAdministration")?.Key,
				SiteKey = DataTypeConverter.ToConcept(resource.Site.GetPrimaryCode(), resource.Site.GetPrimaryCode().System)?.Key,
			};

			Guid key;

			if (Guid.TryParse(resource.Id, out key))
			{
				substanceAdministration.Key = key;
			}

			if (resource.WasNotGiven.Value == true)
			{
				substanceAdministration.IsNegated = true;
			}

			return substanceAdministration;
		}

		/// <summary>
		/// Query for substance administrations.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		/// <param name="totalResults">The total results.</param>
		/// <returns>Returns the list of models which match the given parameters.</returns>
		protected override IEnumerable<SubstanceAdministration> Query(Expression<Func<SubstanceAdministration, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
		{
			Guid initialImmunization = Guid.Parse("f3be6b88-bc8f-4263-a779-86f21ea10a47"),
				immunization = Guid.Parse("6e7a3521-2967-4c0a-80ec-6c5c197b2178"),
				boosterImmunization = Guid.Parse("0331e13f-f471-4fbd-92dc-66e0a46239d5");

			var obsoletionReference = Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.StatusConceptKey))), typeof(Guid)), Expression.Constant(StatusKeys.Completed));
			var typeReference = Expression.MakeBinary(ExpressionType.Or,
				Expression.MakeBinary(ExpressionType.Or,
					Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(initialImmunization)),
					Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(immunization))
				),
				Expression.MakeBinary(ExpressionType.Equal, Expression.Convert(Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(SubstanceAdministration.TypeConceptKey))), typeof(Guid)), Expression.Constant(boosterImmunization))
			);

			query = Expression.Lambda<Func<SubstanceAdministration, bool>>(Expression.AndAlso(Expression.AndAlso(obsoletionReference, query.Body), typeReference), query.Parameters);

			if (queryId == Guid.Empty)
				return this.m_repository.Find(query, offset, count, out totalResults);
			else
				return (this.m_repository as IPersistableQueryRepositoryService).Find<SubstanceAdministration>(query, offset, count, out totalResults, queryId);
		}
	}
}