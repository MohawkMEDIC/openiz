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
 * User: fyfej
 * Date: 2017-7-9
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Security;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Represents a handler for condition observations
	/// </summary>
	public class ConditionResourceHandler : RepositoryResourceHandlerBase<Condition, CodedObservation>
	{
		/// <summary>
		/// Map to FHIR
		/// </summary>
		protected override Condition MapToFhir(CodedObservation model, WebOperationContext webOperationContext)
		{
			var retVal = DataTypeConverter.CreateResource<Condition>(model);

			retVal.Identifier = model.LoadCollection<ActIdentifier>("Identifiers").Select(o => DataTypeConverter.ToFhirIdentifier<Act>(o)).ToList();

			// Clinical status of the condition
			if (model.StatusConceptKey == StatusKeys.Active)
				retVal.ClinicalStatus = ConditionClinicalStatus.Active;
			else if (model.StatusConceptKey == StatusKeys.Completed)
				retVal.ClinicalStatus = ConditionClinicalStatus.Resolved;
			else if (model.StatusConceptKey == StatusKeys.Nullified)
				retVal.VerificationStatus = ConditionVerificationStatus.EnteredInError;
			else if (model.StatusConceptKey == StatusKeys.Obsolete)
				retVal.ClinicalStatus = ConditionClinicalStatus.Inactive;

			// Category
			retVal.Category.Add(new MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes.FhirCodeableConcept(new Uri("http://hl7.org/fhir/condition-category"), "encounter-diagnosis"));

			// Severity?
			var actRelationshipService = ApplicationContext.Current.GetService<IDataPersistenceService<ActRelationship>>();

			var severity = actRelationshipService.Query(o => o.SourceEntityKey == model.Key && o.RelationshipTypeKey == ActRelationshipTypeKeys.HasComponent && o.TargetAct.TypeConceptKey == ObservationTypeKeys.Severity, AuthenticationContext.Current.Principal);
			if (severity == null) // Perhaps we should get from neighbor if this is in an encounter
			{
				var contextAct = actRelationshipService.Query(o => o.TargetActKey == model.Key, AuthenticationContext.Current.Principal).FirstOrDefault();
				if (contextAct != null)
					severity = actRelationshipService.Query(o => o.SourceEntityKey == contextAct.SourceEntityKey && o.RelationshipTypeKey == ActRelationshipTypeKeys.HasComponent && o.TargetAct.TypeConceptKey == ObservationTypeKeys.Severity, AuthenticationContext.Current.Principal);
			}

			// Severity
			if (severity != null)
				retVal.Severity = DataTypeConverter.ToFhirCodeableConcept((severity as CodedObservation).LoadProperty<Concept>("Value"));

			retVal.Code = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("Value"));

			// body sites?
			var sites = actRelationshipService.Query(o => o.SourceEntityKey == model.Key && o.RelationshipTypeKey == ActRelationshipTypeKeys.HasComponent && o.TargetAct.TypeConceptKey == ObservationTypeKeys.FindingSite, AuthenticationContext.Current.Principal);
			retVal.BodySite = sites.Select(o => DataTypeConverter.ToFhirCodeableConcept(o.LoadProperty<CodedObservation>("TargetAct").LoadProperty<Concept>("Value"))).ToList();

			// Subject
			var recordTarget = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
			if (recordTarget != null)
				retVal.Subject = DataTypeConverter.CreateReference<Patient>(recordTarget.LoadProperty<Entity>("PlayerEntity"), webOperationContext);

			// Onset
			if (model.StartTime.HasValue || model.StopTime.HasValue)
				retVal.Onset = new FhirPeriod()
				{
					Start = model.StartTime?.DateTime,
					Stop = model.StopTime?.DateTime
				};
			else
				retVal.Onset = new FhirDateTime(model.ActTime.DateTime);

			retVal.AssertionDate = model.CreationTime.LocalDateTime;
			var author = model.LoadCollection<ActParticipation>("Participations").FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Authororiginator);
			if (author != null)
				retVal.Asserter = DataTypeConverter.CreatePlainReference<Practitioner>(author.LoadProperty<Entity>("PlayerEntity"), webOperationContext);

			return retVal;
		}

		protected override CodedObservation MapToModel(Condition resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Query filter
		/// </summary>
		protected override IEnumerable<CodedObservation> Query(Expression<Func<CodedObservation, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
		{
			var anyRef = base.CreateConceptSetFilter(ConceptSetKeys.ProblemObservations, query.Parameters[0]);
			query = Expression.Lambda<Func<CodedObservation, bool>>(Expression.AndAlso(query.Body, anyRef), query.Parameters);

			return base.Query(query, issues, queryId, offset, count, out totalResults);
		}
	}
}