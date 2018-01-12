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
 * Date: 2018-1-12
 */
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Backbone;
using System.ServiceModel.Web;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.DataTypes;
using MARC.HI.EHRS.SVC.Messaging.FHIR.DataTypes;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Practitioner resource handler
    /// </summary>
    public class PractitionerResourceHandler : RepositoryResourceHandlerBase<Practitioner, UserEntity>
    {
        /// <summary>
        /// Get the interactions that this resource handler supports
        /// </summary>
        protected override IEnumerable<InteractionDefinition> GetInteractions()
        {
            return new TypeRestfulInteraction[]
            {
                TypeRestfulInteraction.InstanceHistory,
                TypeRestfulInteraction.Read,
                TypeRestfulInteraction.Search,
                TypeRestfulInteraction.VersionRead,
                TypeRestfulInteraction.Delete
            }.Select(o => new InteractionDefinition() { Type = o });
        }

        /// <summary>
        /// Map a user entity to a practitioner
        /// </summary>
        protected override Practitioner MapToFhir(UserEntity model, WebOperationContext webOperationContext)
        {
            // Is there a provider that matches this user?
            var provider = model.LoadCollection<EntityRelationship>("Relationships").FirstOrDefault(o => o.RelationshipTypeKey == EntityRelationshipTypeKeys.AssignedEntity)?.LoadProperty<Provider>("TargetEntity") ;
            var retVal = DataTypeConverter.CreateResource<Practitioner>(model);

            // Identifiers
            retVal.Identifier = (provider?.Identifiers ?? model.Identifiers)?.Select(o => DataTypeConverter.ToFhirIdentifier(o)).ToList();

            // ACtive
            retVal.Active = model.StatusConceptKey == StatusKeys.Active;

            // Names
            retVal.Name = (provider?.LoadCollection<EntityName>("Names") ?? model.LoadCollection<EntityName>("Names"))?.Select(o => DataTypeConverter.ToFhirHumanName(o)).ToList();

            // Telecoms
            retVal.Telecom = (provider?.LoadCollection<EntityTelecomAddress>("Telecom") ?? model.LoadCollection<EntityTelecomAddress>("Telecom"))?.Select(o => DataTypeConverter.ToFhirTelecom(o)).ToList();

            // Address
            retVal.Address = (provider?.LoadCollection<EntityAddress>("Addresses") ?? model.LoadCollection<EntityAddress>("Addresses"))?.Select(o => DataTypeConverter.ToFhirAddress(o)).ToList();

            // Birthdate
            retVal.BirthDate = (provider?.DateOfBirth ?? model.DateOfBirth);

            var photo = (provider?.LoadCollection<EntityExtension>("Extensions") ?? model.LoadCollection<EntityExtension>("Extensions"))?.FirstOrDefault(o => o.ExtensionTypeKey == ExtensionTypeKeys.JpegPhotoExtension);
            if (photo != null)
                retVal.Photo = new List<Attachment>() {
                    new Attachment()
                    {
                        ContentType = "image/jpg",
                        Data = photo.ExtensionValueXml
                    }
                };

            // Load the koala-fications 
            retVal.Qualification = provider?.LoadCollection<Concept>("ProviderSpecialty").Select(o => new Qualification()
            {
                Code = DataTypeConverter.ToFhirCodeableConcept(o)
            }).ToList();

            // Language of communication
            retVal.Communication = (provider?.LoadCollection<PersonLanguageCommunication>("LanguageCommunication") ?? model.LoadCollection<PersonLanguageCommunication>("LanguageCommunication"))?.Select(o => new FhirCodeableConcept(new Uri("http://tools.ietf.org/html/bcp47"), o.LanguageCode)).ToList();

            return retVal;
        }

        /// <summary>
        /// Map a practitioner to a user entity
        /// </summary>
        protected override UserEntity MapToModel(Practitioner resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }
    }
}
