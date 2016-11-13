/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Constants;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using OpenIZ.Messaging.FHIR.Util;
using System.ServiceModel.Web;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Immunization recommendation
    /// </summary>
    public class ImmunizationRecommendationResourceHandler : ResourceHandlerBase<ImmunizationRecommendation, SubstanceAdministration>
    {
        // Repository
        private IActRepositoryService m_repository;

        /// <summary>
        /// Place resource handler subscription
        /// </summary>
        public ImmunizationRecommendationResourceHandler()
        {
            ApplicationContext.Current.Started += (o, e) => this.m_repository = ApplicationContext.Current.GetService<IActRepositoryService>();
        }

        protected override SubstanceAdministration Create(SubstanceAdministration modelInstance, List<IResultDetail> issues, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        protected override SubstanceAdministration Delete(Guid modelId, List<IResultDetail> details)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maps the outbound resource to FHIR
        /// </summary>
        protected override ImmunizationRecommendation MapToFhir(SubstanceAdministration model)
        {
            ImmunizationRecommendation retVal = new ImmunizationRecommendation();

            retVal.Id = model.Key.ToString();
            retVal.Timestamp = DateTime.Now;
            retVal.Identifier = model.Identifiers.Select(o => DatatypeConverter.Convert(o)).ToList();

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
                VaccineCode = DatatypeConverter.Convert(mat?.TypeConcept),
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

        protected override SubstanceAdministration MapToModel(ImmunizationRecommendation resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query for immunization recommendatations
        /// </summary>
        protected override IEnumerable<SubstanceAdministration> Query(Expression<Func<SubstanceAdministration, bool>> query, List<IResultDetail> issues, int offset, int count, out int totalResults)
        {
            Expression<Func<SubstanceAdministration, bool>> filter = o => o.ClassConceptKey == ActClassKeys.SubstanceAdministration && o.ObsoletionTime == null && o.MoodConceptKey == ActMoodKeys.Propose;
            var parm = Expression.Parameter(typeof(SubstanceAdministration));
            query = Expression.Lambda<Func<SubstanceAdministration, bool>>(Expression.AndAlso(Expression.Invoke(filter, parm), Expression.Invoke(query, parm)), parm);
            return this.m_repository.Find<SubstanceAdministration>(query, offset, count, out totalResults);
        }

        protected override SubstanceAdministration Read(Identifier<Guid> id, List<IResultDetail> details)
        {
            throw new NotImplementedException();
        }

        protected override SubstanceAdministration Update(SubstanceAdministration model, List<IResultDetail> details, TransactionMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
