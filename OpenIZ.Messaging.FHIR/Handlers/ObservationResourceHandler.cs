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
 * Date: 2017-6-15
 */

using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Messaging.FHIR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;

namespace OpenIZ.Messaging.FHIR.Handlers
{
	/// <summary>
	/// Observation handler
	/// </summary>
	public class ObservationResourceHandler : RepositoryResourceHandlerBase<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation, Core.Model.Acts.Observation>
	{
		/// <summary>
		/// Map to FHIR
		/// </summary>
		protected override MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation MapToFhir(Core.Model.Acts.Observation model, WebOperationContext webOperationContext)
		{
			var retVal = DataTypeConverter.CreateResource<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation>(model);

			retVal.EffectiveDateTime = (FhirDate)model.ActTime.DateTime;

			if (model.StatusConceptKey == StatusKeys.Completed)
				retVal.Status = new FhirCode<ObservationStatus>(ObservationStatus.Final);
			else if (model.StatusConceptKey == StatusKeys.Active)
				retVal.Status = new FhirCode<ObservationStatus>(ObservationStatus.Preliminary);
			else if (model.StatusConceptKey == StatusKeys.Nullified)
				retVal.Status = new FhirCode<ObservationStatus>(ObservationStatus.EnteredInError);

			if (model.Relationships.Any(o => o.RelationshipTypeKey == ActRelationshipTypeKeys.Replaces))
				retVal.Status = new FhirCode<ObservationStatus>(ObservationStatus.Corrected);

			// RCT
			var rct = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget);
			if (rct != null)
			{
				retVal.Subject = Reference.CreateResourceReference(new Patient() { Id = rct.PlayerEntityKey.ToString() }, webOperationContext.IncomingRequest.UriTemplateMatch.BaseUri);
			}

			// Performer
			var prf = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Performer);
			if (prf != null)
				retVal.Performer = Reference.CreateResourceReference(new Practitioner() { Id = rct.PlayerEntityKey.ToString() }, webOperationContext.IncomingRequest.UriTemplateMatch.BaseUri);

			retVal.Issued = (FhirInstant)model.CreationTime.DateTime;

			// Value

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
		/// Map to model
		/// </summary>
		protected override Core.Model.Acts.Observation MapToModel(MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation resource, WebOperationContext webOperationContext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Query
		/// </summary>
		protected override IEnumerable<Core.Model.Acts.Observation> Query(Expression<Func<Core.Model.Acts.Observation, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
		{
			var anyRef = base.CreateConceptSetFilter(ConceptSetKeys.VitalSigns, query.Parameters[0]);
			query = Expression.Lambda<Func<Core.Model.Acts.Observation, bool>>(Expression.AndAlso(query.Body, anyRef), query.Parameters);

			return base.Query(query, issues, queryId, offset, count, out totalResults);
		}
	}
}