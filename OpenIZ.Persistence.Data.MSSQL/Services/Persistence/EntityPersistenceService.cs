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
 * Date: 2016-6-19
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using System.Security.Principal;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Data.Linq;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Entity persistence service
    /// </summary>
    public class EntityPersistenceService : VersionedDataPersistenceService<Core.Model.Entities.Entity, Data.EntityVersion, Data.Entity>
    {



        private const String Entity = "E29FCFAD-EC1D-4C60-A055-039A494248AE";
        private const String ManufacturedMaterial = "FAFEC286-89D5-420B-9085-054ACA9D1EEF";
        private const String Animal = "61FCBF42-B5E0-4FB5-9392-108A5C6DBEC7";
        private const String Place = "21AB7873-8EF3-4D78-9C19-4582B3C40631";
        private const String Device = "1373FF04-A6EF-420A-B1D0-4A07465FE8E8";
        private const String Organization = "7C08BD55-4D42-49CD-92F8-6388D6C4183F";
        private const String Food = "E5A09CC2-5AE5-40C2-8E32-687DBA06715D";
        private const String Material = "D39073BE-0F8F-440E-B8C8-7034CC138A95";
        private const String Person = "9DE2A846-DDF2-4EBC-902E-84508C5089EA";
        private const String CityOrTown = "79DD4F75-68E8-4722-A7F5-8BC2E08F5CD6";
        private const String ChemicalSubstance = "2E9FA332-9391-48C6-9FC8-920A750B25D3";
        private const String State = "8CF4B0B0-84E5-4122-85FE-6AFA8240C218";
        private const String Container = "B76FF324-B174-40B7-A6AC-D1FDF8E23967";
        private const String LivingSubject = "8BA5E5C9-693B-49D4-973C-D7010F3A23EE";
        private const String Patient = "BACD9C6F-3FA9-481E-9636-37457962804D";
        private const String ServiceDeliveryLocation = "FF34DFA7-C6D3-4F8B-BC9F-14BCDC13BA6C";
        private const String Provider = "6B04FED8-C164-469C-910B-F824C2BDA4F0";
        private const String CountyOrParish = "D9489D56-DDAC-4596-B5C6-8F41D73D8DC5";
        private const String Country = "48B2FFB3-07DB-47BA-AD73-FC8FB8502471";
        private const String NonLivingSubject = "9025E5C9-693B-49D4-973C-D7010F3A23EE";


        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TEntityType ToModelInstance<TEntityType>(Data.EntityVersion dbInstance, Data.ModelDataContext context, IPrincipal principal) where TEntityType : Core.Model.Entities.Entity, new()
        {
            var retVal = m_mapper.MapDomainInstance<Data.EntityVersion, TEntityType>(dbInstance);

            retVal.ClassConceptKey = dbInstance.Entity.ClassConceptId;
            retVal.DeterminerConceptKey = dbInstance.Entity.DeterminerConceptId;

            // Inversion relationships
            //if (retVal.Relationships != null)
            //{
            //    retVal.Relationships.RemoveAll(o => o.InversionIndicator);
            //    retVal.Relationships.AddRange(context.EntityAssociations.Where(o => o.TargetEntityId == retVal.Key.Value).Distinct().Select(o => new EntityRelationship(o.AssociationTypeConceptId, o.TargetEntityId)
            //    {
            //        SourceEntityKey = o.SourceEntityId,
            //        Key = o.EntityAssociationId,
            //        InversionIndicator = true
            //    }));
            //}
            
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Entities.Entity ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            // Alright first, which type am I mapping to?
            var dbEntity = dataInstance as Data.EntityVersion;
            
            switch(dbEntity.Entity.ClassConceptId.ToString().ToUpper())
            {
                case Device:
                    return new DeviceEntityPersistenceService().ToModelInstance(dataInstance, context, principal);
                case NonLivingSubject:
                    return new ApplicationEntityPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Person:
                    return new PersonPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Patient:
                    return new PatientPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Provider:
                    return new ProviderPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Place:
                case CityOrTown:
                case Country:
                case CountyOrParish:
                case State:
                case ServiceDeliveryLocation:
                    return new PlacePersistenceService().ToModelInstance(dataInstance, context, principal);
                case Organization:
                    return new OrganizationPersistenceService().ToModelInstance(dataInstance, context, principal);
                case Material:
                    return new MaterialPersistenceService().ToModelInstance(dataInstance, context, principal);
                case ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().ToModelInstance(dataInstance, context, principal);
                default:
                    return base.ToModelInstance(dataInstance, context, principal);

            }
        }

        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        public override Core.Model.Entities.Entity Insert(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {

            // Ensure FK exists
            data.ClassConcept?.EnsureExists(context,principal);
            data.DeterminerConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.StatusConceptKey = data.StatusConceptKey == Guid.Empty || data.StatusConceptKey == null ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.Insert(context, data, principal);

            // Identifiers
            if (data.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, Data.EntityIdentifier>(
                    data.Identifiers,
                    retVal,
                    context,
                    principal);

            // Relationships
            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, Data.EntityAssociation>(
                    data.Relationships.Where(o=>!o.InversionIndicator).ToList(),
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, Data.EntityTelecomAddress>(
                    data.Telecoms,
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, Data.EntityExtension>(
                    data.Extensions,
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, Data.EntityName>(
                    data.Names,
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, Data.EntityAddress>(
                    data.Addresses,
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, Data.EntityNote>(
                    data.Notes,
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, Data.EntityTag>(
                    data.Tags,
                    retVal,
                    context,
                    principal);


            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        public override Core.Model.Entities.Entity Update(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            // Esnure exists
            data.ClassConcept?.EnsureExists(context, principal);
            data.DeterminerConcept?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);
            data.TypeConcept?.EnsureExists(context, principal);
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;

            var retVal = base.Update(context, data, principal);


            // Identifiers
            if (data.Identifiers != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, Data.EntityIdentifier>(
                    data.Identifiers,
                    retVal,
                    context,
                    principal);

            // Relationships
            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, Data.EntityAssociation>(
                    data.Relationships.Where(o => !o.InversionIndicator).ToList(),
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, Data.EntityTelecomAddress>(
                    data.Telecoms,
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, Data.EntityExtension>(
                    data.Extensions,
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, Data.EntityName>(
                    data.Names,
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, Data.EntityAddress>(
                    data.Addresses,
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, Data.EntityNote>(
                    data.Notes,
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, Data.EntityTag>(
                    data.Tags,
                    retVal,
                    context,
                    principal);

      
            return retVal;
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        public override Core.Model.Entities.Entity Obsolete(ModelDataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = base.GetDataLoadOptions();
            loadOptions.LoadWith<Data.EntityVersion>(cs => cs.StatusConcept);
            loadOptions.LoadWith<Data.EntityVersion>(cs => cs.TypeConcept);
            loadOptions.LoadWith<Data.Entity>(cs => cs.ClassConcept);
            loadOptions.LoadWith<Data.Entity>(cs => cs.DeterminerConcept);

            /*            loadOptions.LoadWith<Data.Entity>(cs => cs.EntityTags);
                        loadOptions.LoadWith<Data.Entity>(cs => cs.EntityNames);
                        loadOptions.LoadWith<Data.Entity>(cs => cs.EntityIdentifiers);
                        loadOptions.LoadWith<Data.Entity>(cs => cs.EntityAddresses);
                        loadOptions.LoadWith<Data.Entity>(cs => cs.EntityTelecomAddresses);
                        loadOptions.LoadWith<Data.Entity>(cs => cs.EntityNotes);
                        loadOptions.LoadWith<Data.EntityName>(cs => cs.EntityNameComponents);*/
            loadOptions.LoadWith<Data.EntityName>(cs => cs.NameUseConcept);
            //loadOptions.LoadWith<Data.EntityAddress>(cs => cs.EntityAddressComponents);
            loadOptions.LoadWith<Data.EntityAddress>(cs => cs.AddressUseConcept);
            loadOptions.LoadWith<Data.EntityNameComponent>(cs => cs.PhoneticValue);
            loadOptions.LoadWith<Data.EntityNameComponent>(cs => cs.ComponentTypeConcept);
            loadOptions.LoadWith<Data.EntityAddressComponent>(cs => cs.EntityAddressComponentValue);
            loadOptions.LoadWith<Data.EntityAddressComponent>(cs => cs.ComponentTypeConcept);
            loadOptions.LoadWith<Data.EntityTelecomAddress>(cs => cs.TelecomUseConcept);

            loadOptions.LoadWith<Data.EntityIdentifier>(cs => cs.AssigningAuthority);
            loadOptions.LoadWith<Data.EntityAssociation>(cs => cs.AssociationTypeConcept);
            loadOptions.LoadWith<Data.EntityExtension>(cs => cs.ExtensionType);

            // CS Version
            loadOptions.LoadWith<Data.ConceptVersion>(cs => cs.Concept);
            loadOptions.LoadWith<Data.ConceptVersion>(cs => cs.StatusConcept);
            loadOptions.LoadWith<Data.ConceptVersion>(cs => cs.ConceptClass);

            loadOptions.LoadWith<Data.Material>(m => m.EntityVersion);
            loadOptions.LoadWith<Data.ManufacturedMaterial>(m => m.Material);
            loadOptions.LoadWith<Data.Patient>(p => p.Person);
            loadOptions.LoadWith<Data.Provider>(p => p.Person);
            loadOptions.LoadWith<Data.Person>(p => p.EntityVersion);
            loadOptions.LoadWith<Data.Place>(p => p.EntityVersion);

            return loadOptions;
        }
    }
}
