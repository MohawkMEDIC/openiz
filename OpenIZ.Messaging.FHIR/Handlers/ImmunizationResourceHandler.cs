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
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
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
	public class ImmunizationResourceHandler : ResourceHandlerBase<Immunization, SubstanceAdministration>
	{
		/// <summary>
		/// The repository.
		/// </summary>
		private IActRepositoryService repository;

		/// <summary>
		/// Place resource handler subscription
		/// </summary>
		public ImmunizationResourceHandler()
		{
			ApplicationContext.Current.Started += (o, e) => this.repository = ApplicationContext.Current.GetService<IActRepositoryService>();
		}

		/// <summary>
		/// Create the specified substance administration.
		/// </summary>
		/// <param name="modelInstance">The model instance.</param>
		/// <param name="issues">The issues.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the created model.</returns>
		protected override SubstanceAdministration Create(SubstanceAdministration modelInstance, List<IResultDetail> issues, MARC.HI.EHRS.SVC.Core.Services.TransactionMode mode)
		{
			return this.repository.Insert(modelInstance);
		}

		/// <summary>
		/// Delete a substance administration.
		/// </summary>
		/// <param name="modelId">The model identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the deleted model.</returns>
		protected override SubstanceAdministration Delete(Guid modelId, List<IResultDetail> details)
		{
			return this.repository.Obsolete<SubstanceAdministration>(modelId);
		}

		/// <summary>
		/// Maps the substance administration to FHIR.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Returns the mapped FHIR resource.</returns>
		protected override Immunization MapToFhir(SubstanceAdministration model)
		{
			var retVal = DataTypeConverter.CreateResource<Immunization>(model);
			retVal.DoseQuantity = new FhirQuantity()
			{
				Units = model.DoseUnit.Mnemonic,
				Value = new FhirDecimal(model.DoseQuantity)
			};
			retVal.Date = (FhirDate)model.ActTime.DateTime;
			retVal.Route = DataTypeConverter.ToFhirCodeableConcept(model.Route);
			retVal.Site = DataTypeConverter.ToFhirCodeableConcept(model.Site);
			retVal.Status = "completed";
			//retVal.SelfReported = model.Tags.Any(o => o.TagKey == "selfReported" && Convert.ToBoolean(o.Value));
			retVal.WasNotGiven = model.IsNegated;

			// Material
			var matl = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Consumable)?.PlayerEntity as Material;
			if (matl != null)
			{
				retVal.VaccineCode = DataTypeConverter.ToFhirCodeableConcept(matl.TypeConcept);
				retVal.ExpirationDate = (FhirDate)matl.ExpiryDate;
				retVal.LotNumber = (matl as ManufacturedMaterial)?.LotNumber;
			}

			// RCT
			var rct = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
			if (rct != null)
			{
				retVal.Patient = Reference.CreateLocalResourceReference(new Patient() { Id = rct.PlayerEntityKey.ToString() });
			}

			// Performer
			var prf = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Performer);
			if (prf != null)
				retVal.Performer = Reference.CreateResourceReference(new Practictioner() { Id = rct.PlayerEntityKey.ToString() }, WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri);

			// Protocol
			foreach (var itm in model.Protocols)
			{
				ImmunizationProtocol protocol = new ImmunizationProtocol();
				protocol.DoseSequence = new FhirInt((int)model.SequenceId);
				protocol.Series = itm.Protocol.Name;
				retVal.VaccinationProtocol.Add(protocol);
			}
			if (retVal.VaccinationProtocol.Count == 0)
				retVal.VaccinationProtocol.Add(new ImmunizationProtocol() { DoseSequence = (int)model.SequenceId });

			return retVal;
		}

		/// <summary>
		/// Map an immunization FHIR resource to a substance administration.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <returns>Returns the mapped model.</returns>
		protected override SubstanceAdministration MapToModel(Immunization resource)
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
		protected override IEnumerable<SubstanceAdministration> Query(Expression<Func<SubstanceAdministration, bool>> query, List<IResultDetail> issues, int offset, int count, out int totalResults)
		{
            var obsoletionReference = Expression.MakeBinary(ExpressionType.NotEqual, Expression.MakeMemberAccess(query.Parameters[0], typeof(SubstanceAdministration).GetProperty(nameof(BaseEntityData.ObsoletionTime))), Expression.Constant(null));
            query = Expression.Lambda<Func<SubstanceAdministration, bool>>(Expression.AndAlso(obsoletionReference, query), query.Parameters);
			return this.repository.Find(query, offset, count, out totalResults);
		}

		/// <summary>
		/// Return a substance administration.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="details">The details.</param>
		/// <returns>Returns the model which matches the given id.</returns>
		protected override SubstanceAdministration Read(Identifier<Guid> id, List<IResultDetail> details)
		{
			return this.repository.Get<SubstanceAdministration>(id.Id, id.VersionId);
		}

		/// <summary>
		/// Update the specified substance administration.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="details">The details.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>Returns the updated model.</returns>
		protected override SubstanceAdministration Update(SubstanceAdministration model, List<IResultDetail> details, MARC.HI.EHRS.SVC.Core.Services.TransactionMode mode)
		{
			return this.repository.Save(model);
		}
	}
}