/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2018-1-9
 */
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;
using System.ServiceModel.Web;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Entities;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;
using OpenIZ.Core.Model.Roles;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Encounter resource handler for loading and disclosing of patient encounters
    /// </summary>
    public class EncounterResourceHandler : RepositoryResourceHandlerBase<Encounter, PatientEncounter>
    {
        /// <summary>
        /// Get the interactions supported
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<InteractionDefinition> GetInteractions()
        {
            return new List<TypeRestfulInteraction>()
            {
                TypeRestfulInteraction.Search,
                TypeRestfulInteraction.Read,
                TypeRestfulInteraction.VersionRead,
                TypeRestfulInteraction.InstanceHistory
            }.Select(o => new InteractionDefinition() { Type = o });
        }

        /// <summary>
        /// Map the specified patient encounter to a FHIR based encounter
        /// </summary>
        protected override Encounter MapToFhir(PatientEncounter model, WebOperationContext webOperationContext)
        {
            var retVal = DataTypeConverter.CreateResource<Encounter>(model);

            // Map the identifier
            retVal.Identifier = model.LoadCollection<ActIdentifier>("Identifiers").Select(o => DataTypeConverter.ToFhirIdentifier<Act>(o)).ToList();

            // Map status keys
            switch(model.StatusConceptKey.ToString().ToUpper())
            {
                case StatusKeyStrings.Active:
                    retVal.Status = EncounterStatus.InProgress;
                    break;
                case StatusKeyStrings.Cancelled:
                case StatusKeyStrings.Nullified:
                    retVal.Status = EncounterStatus.Cancelled;
                    break;
                case StatusKeyStrings.Completed:
                    retVal.Status = EncounterStatus.Finished;
                    break;
            }

            if (model.StartTime.HasValue || model.StopTime.HasValue)
                retVal.Period = new FhirPeriod()
                {
                    Start = model.StartTime?.DateTime,
                    Stop = model.StopTime?.DateTime
                };
            else
                retVal.Period = new FhirPeriod()
                {
                    Start = model.ActTime.DateTime,
                    Stop = model.ActTime.DateTime
                };

            retVal.Reason = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("ReasonConcept"));
            retVal.Type = DataTypeConverter.ToFhirCodeableConcept(model.LoadProperty<Concept>("TypeConcept"));
            
            // Map associated
            var associated = model.LoadCollection<ActParticipation>("Participations");

            // Subject of encounter
            retVal.Subject = DataTypeConverter.CreateReference<MARC.HI.EHRS.SVC.Messaging.FHIR.Resources.Patient>(associated.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget)?.LoadProperty<Entity>("PlayerEntity"), webOperationContext);

            // Locations
            retVal.Location = associated.Where(o => o.LoadProperty<Entity>("PlayerEntity") is Place).Select(o => new EncounterLocation()
            {
                Period = new FhirPeriod() { Start = model.CreationTime.DateTime },
                Location = DataTypeConverter.CreateReference<Location>(o.PlayerEntity, webOperationContext)
            }).ToList();


            // Participants
            retVal.Participant = associated.Where(o => o.LoadProperty<Entity>("PlayerEntity") is Provider || o.LoadProperty<Entity>("PlayerEntity") is UserEntity).Select(o => new EncounterParticipant()
            {
                Period = new FhirPeriod() { Start = model.CreationTime.DateTime },
                Type = new List<FhirCodeableConcept>() { DataTypeConverter.ToFhirCodeableConcept(o.LoadProperty<Concept>("ParticipationRole")) },
                Individual = DataTypeConverter.CreateReference<Practitioner>(o.PlayerEntity, webOperationContext)
            }).ToList();


            return retVal; 
        }

        protected override PatientEncounter MapToModel(Encounter resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }
    }
}
