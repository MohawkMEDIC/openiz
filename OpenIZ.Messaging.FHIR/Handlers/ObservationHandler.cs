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
 * Date: 2017-6-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Messaging.FHIR.Util;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Security;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Observation handler
    /// </summary>
    public class ObservationHandler : RepositoryResourceHandlerBase<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation, Core.Model.Acts.Observation>
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
                retVal.Performer = Reference.CreateResourceReference(new Practictioner() { Id = rct.PlayerEntityKey.ToString() }, webOperationContext.IncomingRequest.UriTemplateMatch.BaseUri);

            retVal.Issued = (FhirInstant)model.CreationTime.DateTime;

            // Value
            
            retVal.Extension = model.Extensions.Select(o => DataTypeConverter.ToExtension(o)).ToList();

            var loc = model.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.Location);
            if (loc != null)
                retVal.Extension.Add(new Extension()
                {
                    Url = "http://openiz.org/extensions/act/fhir/location",
                    Value = new FhirString(loc.PlayerEntityKey.ToString())
                });

            // metadata
            retVal.Meta = new ResourceMetadata()
            {
                LastUpdated = model.ModifiedOn.DateTime,
                VersionId = model.VersionKey?.ToString(),
                Profile = new Uri("http://openiz.org/fhir")
            };
            retVal.Meta.Tags = model.Tags.Select(o => new FhirCoding(new Uri("http://openiz.org/tags/fhir/" + o.TagKey), o.Value)).ToList();
            // TODO: Configure this namespace / coding scheme
            retVal.Meta.Security = model.Policies.Where(o => o.GrantType == Core.Model.Security.PolicyGrantType.Grant).Select(o => new FhirCoding(new Uri("http://openiz.org/security/policy"), o.Policy.Oid)).ToList();
            retVal.Meta.Security.Add(new FhirCoding(new Uri("http://openiz.org/security/policy"), PermissionPolicyIdentifiers.ReadClinicalData));

            return retVal;
        }

        /// <summary>
        /// Map to model
        /// </summary>
        protected override Core.Model.Acts.Observation MapToModel(MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Observation resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }
    }
}
