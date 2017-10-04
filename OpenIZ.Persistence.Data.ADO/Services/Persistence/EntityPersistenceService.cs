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
 * Date: 2017-1-21
 */
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Constants;
using System.Security.Principal;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model.Extensibility;
using OpenIZ.Persistence.Data.ADO.Data.Model.Roles;
using OpenIZ.Persistence.Data.ADO.Data.Model.DataType;
using OpenIZ.OrmLite;
using System.Diagnostics;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Entity persistence service
    /// </summary>
    public class EntityPersistenceService : VersionedDataPersistenceService<Core.Model.Entities.Entity, DbEntityVersion, DbEntity>
    {

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TEntityType ToModelInstance<TEntityType>(DbEntityVersion dbVersionInstance, DbEntity entInstance, DataContext context, IPrincipal principal) where TEntityType : Core.Model.Entities.Entity, new()
        {
            var retVal = m_mapper.MapDomainInstance<DbEntityVersion, TEntityType>(dbVersionInstance);

            if (retVal == null) return null;

            retVal.ClassConceptKey = entInstance.ClassConceptKey;
            retVal.DeterminerConceptKey = entInstance.DeterminerConceptKey;
            retVal.TemplateKey = entInstance.TemplateKey;
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
        public override Core.Model.Entities.Entity ToModelInstance(object dataInstance, DataContext context, IPrincipal principal)
        {

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif 
            if (dataInstance == null)
                return null;
            // Alright first, which type am I mapping to?
            var dbEntityVersion = (dataInstance as CompositeResult)?.Values.OfType<DbEntityVersion>().FirstOrDefault() ?? dataInstance as DbEntityVersion ?? context.FirstOrDefault<DbEntityVersion>(o => o.VersionKey == (dataInstance as DbEntitySubTable).ParentKey);
            var dbEntity = (dataInstance as CompositeResult)?.Values.OfType<DbEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbEntity>(o => o.Key == dbEntityVersion.Key);
            Entity retVal = null;

            switch (dbEntity.ClassConceptKey.ToString().ToLower())
            {
                case EntityClassKeyStrings.Device:
                    retVal = new DeviceEntityPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbDeviceEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbDeviceEntity>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.NonLivingSubject:
                    retVal = new ApplicationEntityPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbApplicationEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbApplicationEntity>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.Person:
                    var ue = (dataInstance as CompositeResult)?.Values.OfType<DbUserEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbUserEntity>(o => o.ParentKey == dbEntityVersion.VersionKey);

                    if (ue != null)
                        retVal = new UserEntityPersistenceService().ToModelInstance(
                            ue,
                            (dataInstance as CompositeResult)?.Values.OfType<DbPerson>().FirstOrDefault() ?? context.FirstOrDefault<DbPerson>(o => o.ParentKey == dbEntityVersion.VersionKey),
                            dbEntityVersion,
                            dbEntity,
                            context,
                            principal);
                    else
                        retVal = new PersonPersistenceService().ToModelInstance(
                            (dataInstance as CompositeResult)?.Values.OfType<DbPerson>().FirstOrDefault() ?? context.FirstOrDefault<DbPerson>(o => o.ParentKey == dbEntityVersion.VersionKey),
                            dbEntityVersion,
                            dbEntity,
                            context,
                            principal);
                    break;
                case EntityClassKeyStrings.Patient:
                    retVal = new PatientPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbPatient>().FirstOrDefault() ?? context.FirstOrDefault<DbPatient>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        (dataInstance as CompositeResult)?.Values.OfType<DbPerson>().FirstOrDefault() ?? context.FirstOrDefault<DbPerson>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.Provider:
                    retVal = new ProviderPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbProvider>().FirstOrDefault() ?? context.FirstOrDefault<DbProvider>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        (dataInstance as CompositeResult)?.Values.OfType<DbPerson>().FirstOrDefault() ?? context.FirstOrDefault<DbPerson>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.Place:
                case EntityClassKeyStrings.CityOrTown:
                case EntityClassKeyStrings.Country:
                case EntityClassKeyStrings.CountyOrParish:
                case EntityClassKeyStrings.State:
                case EntityClassKeyStrings.ServiceDeliveryLocation:
                    retVal = new PlacePersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbPlace>().FirstOrDefault() ?? context.FirstOrDefault<DbPlace>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.Organization:
                    retVal = new OrganizationPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbOrganization>().FirstOrDefault() ?? context.FirstOrDefault<DbOrganization>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.Material:
                    retVal = new MaterialPersistenceService().ToModelInstance<Material>(
                        (dataInstance as CompositeResult)?.Values.OfType<DbMaterial>().FirstOrDefault() ?? context.FirstOrDefault<DbMaterial>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                case EntityClassKeyStrings.ManufacturedMaterial:
                    retVal = new ManufacturedMaterialPersistenceService().ToModelInstance(
                        (dataInstance as CompositeResult)?.Values.OfType<DbManufacturedMaterial>().FirstOrDefault() ?? context.FirstOrDefault<DbManufacturedMaterial>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        (dataInstance as CompositeResult)?.Values.OfType<DbMaterial>().FirstOrDefault() ?? context.FirstOrDefault<DbMaterial>(o => o.ParentKey == dbEntityVersion.VersionKey),
                        dbEntityVersion,
                        dbEntity,
                        context,
                        principal);
                    break;
                default:
                    retVal = this.ToModelInstance<Entity>(dbEntityVersion, dbEntity, context, principal);
                    break;

            }

#if DEBUG
            sw.Stop();
            this.m_tracer.TraceEvent(TraceEventType.Verbose, 0, "Basic conversion took: {0}", sw.ElapsedMilliseconds);
#endif 
            retVal.LoadAssociations(context, principal);
            return retVal;
        }

        /// <summary>
        /// Conversion based on type
        /// </summary>
        protected override Entity CacheConvert(object dataInstance, DataContext context, IPrincipal principal)
        {
            return this.DoCacheConvert(dataInstance, context, principal);
        }

        /// <summary>
        /// Perform the cache convert
        /// </summary>
        internal Entity DoCacheConvert(object dataInstance, DataContext context, IPrincipal principal) { 
            if (dataInstance == null)
                return null;
            // Alright first, which type am I mapping to?
            var dbEntityVersion = (dataInstance as CompositeResult)?.Values.OfType<DbEntityVersion>().FirstOrDefault() ?? dataInstance as DbEntityVersion ?? context.FirstOrDefault<DbEntityVersion>(o => o.VersionKey == (dataInstance as DbEntitySubTable).ParentKey);
            var dbEntity = (dataInstance as CompositeResult)?.Values.OfType<DbEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbEntity>(o => o.Key == dbEntityVersion.Key);
            Entity retVal = null;
            var cache = new AdoPersistenceCache(context);

            if (!dbEntityVersion.ObsoletionTime.HasValue)
                switch (dbEntity.ClassConceptKey.ToString().ToUpper())
                {
                    case EntityClassKeyStrings.Device:
                        retVal = cache?.GetCacheItem<DeviceEntity>(dbEntity.Key);
                        break;
                    case EntityClassKeyStrings.NonLivingSubject:
                        retVal = cache?.GetCacheItem<ApplicationEntity>(dbEntity.Key);
                        break;
                    case EntityClassKeyStrings.Person:
                        var ue = (dataInstance as CompositeResult)?.Values.OfType<DbUserEntity>().FirstOrDefault() ?? context.FirstOrDefault<DbUserEntity>(o => o.ParentKey == dbEntityVersion.VersionKey);
                        if (ue != null)
                            retVal = cache?.GetCacheItem<UserEntity>(dbEntity.Key);

                        else
                            retVal = cache?.GetCacheItem<Person>(dbEntity.Key);
                        break;
                    case EntityClassKeyStrings.Patient:
                        retVal = cache?.GetCacheItem<Patient>(dbEntity.Key);
                        break;
                    case EntityClassKeyStrings.Provider:
                        retVal = cache?.GetCacheItem<Provider>(dbEntity.Key);

                        break;
                    case EntityClassKeyStrings.Place:
                    case EntityClassKeyStrings.CityOrTown:
                    case EntityClassKeyStrings.Country:
                    case EntityClassKeyStrings.CountyOrParish:
                    case EntityClassKeyStrings.State:
                    case EntityClassKeyStrings.ServiceDeliveryLocation:
                        retVal = cache?.GetCacheItem<Place>(dbEntity.Key);

                        break;
                    case EntityClassKeyStrings.Organization:
                        retVal = cache?.GetCacheItem<Organization>(dbEntity.Key);

                        break;
                    case EntityClassKeyStrings.Material:
                        retVal = cache?.GetCacheItem<Material>(dbEntity.Key);

                        break;
                    case EntityClassKeyStrings.ManufacturedMaterial:
                        retVal = cache?.GetCacheItem<ManufacturedMaterial>(dbEntity.Key);

                        break;
                    default:
                        retVal = cache?.GetCacheItem<Entity>(dbEntity.Key);
                        break;
                }

            // Return cache value
            if (retVal != null)
                return retVal;
            else
                return base.CacheConvert(dataInstance, context, principal);
        }

        /// <summary>
        /// Insert the specified entity into the data context
        /// </summary>
        public Core.Model.Entities.Entity InsertCoreProperties(DataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {

            // Ensure FK exists
            if (data.ClassConcept != null) data.ClassConcept = data.ClassConcept?.EnsureExists(context, principal) as Concept;
            if (data.DeterminerConcept != null) data.DeterminerConcept = data.DeterminerConcept?.EnsureExists(context, principal) as Concept;
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept?.EnsureExists(context, principal) as Concept;
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept?.EnsureExists(context, principal) as Concept;
            if (data.Template != null) data.Template = data.Template?.EnsureExists(context, principal) as TemplateDefinition;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.StatusConceptKey = data.StatusConceptKey == Guid.Empty || data.StatusConceptKey == null ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.InsertInternal(context, data, principal);

            // Identifiers
	        if (data.Identifiers != null)
	        {
		        // Validate unique values for IDs
		        var uniqueIds = data.Identifiers.Where(o => o.AuthorityKey.HasValue).Where(o => ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>().Get(new Identifier<Guid>(o.AuthorityKey.Value), principal, true)?.IsUnique == true);

		        foreach (var entityIdentifier in uniqueIds)
		        {
			        if (context.Query<DbEntityIdentifier>(c => c.SourceKey != data.Key && c.AuthorityKey == entityIdentifier.AuthorityKey && c.Value == entityIdentifier.Value && !c.ObsoleteVersionSequenceId.HasValue).Any())
			        {
						throw new DuplicateNameException(entityIdentifier.Value);
					}
				}

				base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, DbEntityIdentifier>(
					data.Identifiers.Where(o => o != null && !o.IsEmpty()),
					retVal,
					context,
					principal);
			}

            // Relationships
            if (data.Relationships != null) 
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, DbEntityRelationship>(
                   data.Relationships.Where(o => o != null && !o.InversionIndicator && !o.IsEmpty()).ToList(),
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, DbTelecomAddress>(
                   data.Telecoms.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, DbEntityExtension>(
                   data.Extensions.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, DbEntityName>(
                   data.Names.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, DbEntityAddress>(
                   data.Addresses.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, DbEntityNote>(
                   data.Notes.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, DbEntityTag>(
                   data.Tags.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);


            return retVal;
        }

        /// <summary>
        /// Update the specified entity
        /// </summary>
        public Core.Model.Entities.Entity UpdateCoreProperties(DataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            // Esnure exists
            if (data.ClassConcept != null) data.ClassConcept = data.ClassConcept?.EnsureExists(context, principal) as Concept;
            if (data.DeterminerConcept != null) data.DeterminerConcept = data.DeterminerConcept?.EnsureExists(context, principal) as Concept;
            if (data.StatusConcept != null) data.StatusConcept = data.StatusConcept?.EnsureExists(context, principal) as Concept;
            if (data.Template != null) data.Template = data.Template?.EnsureExists(context, principal) as TemplateDefinition;
            if (data.TypeConcept != null) data.TypeConcept = data.TypeConcept?.EnsureExists(context, principal) as Concept;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;
            data.DeterminerConceptKey = data.DeterminerConcept?.Key ?? data.DeterminerConceptKey;
            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey;
            data.StatusConceptKey = data.StatusConceptKey == Guid.Empty || data.StatusConceptKey == null ? StatusKeys.New : data.StatusConceptKey;

            var retVal = base.UpdateInternal(context, data, principal);


            // Identifiers
	        if (data.Identifiers != null)
	        {
				// Validate unique values for IDs
		        var uniqueIds = data.Identifiers.Where(o => o.AuthorityKey.HasValue).Where(o => ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>().Get(new Identifier<Guid>(o.AuthorityKey.Value), principal, true)?.IsUnique == true);

		        foreach (var entityIdentifier in uniqueIds)
		        {
			        if (context.Query<DbEntityIdentifier>(c => c.SourceKey != data.Key && c.AuthorityKey == entityIdentifier.AuthorityKey && c.Value == entityIdentifier.Value && !c.ObsoleteVersionSequenceId.HasValue).Any())
			        {
				        throw new DuplicateNameException(entityIdentifier.Value);
			        }
		        }

		        base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityIdentifier, DbEntityIdentifier>(
			        data.Identifiers.Where(o => !o.IsEmpty()),
			        retVal,
			        context,
			        principal);
			}

            // Relationships
            if (data.Relationships != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityRelationship, DbEntityRelationship>(
                   data.Relationships.Where(o => o != null && !o.InversionIndicator && !o.IsEmpty() && (o.SourceEntityKey == data.Key || !o.SourceEntityKey.HasValue)).ToList(),
                    retVal,
                    context,
                    principal);

            // Telecoms
            if (data.Telecoms != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityTelecomAddress, DbTelecomAddress>(
                   data.Telecoms.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Extensions
            if (data.Extensions != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityExtension, DbEntityExtension>(
                   data.Extensions.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Names
            if (data.Names != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityName, DbEntityName>(
                   data.Names.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Addresses
            if (data.Addresses != null)
                base.UpdateVersionedAssociatedItems<Core.Model.Entities.EntityAddress, DbEntityAddress>(
                   data.Addresses.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Notes
            if (data.Notes != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.EntityNote, DbEntityNote>(
                   data.Notes.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);

            // Tags
            if (data.Tags != null)
                base.UpdateAssociatedItems<Core.Model.DataTypes.EntityTag, DbEntityTag>(
                   data.Tags.Where(o => o != null && !o.IsEmpty()),
                    retVal,
                    context,
                    principal);


            return retVal;
        }

        /// <summary>
        /// Obsoleted status key
        /// </summary>
        public override Core.Model.Entities.Entity ObsoleteInternal(DataContext context, Core.Model.Entities.Entity data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Insert the entity
        /// </summary>
        public override Entity InsertInternal(DataContext context, Entity data, IPrincipal principal)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case EntityClassKeyStrings.Device:
                    return new DeviceEntityPersistenceService().InsertInternal(context, data.Convert<DeviceEntity>(), principal);
                case EntityClassKeyStrings.NonLivingSubject:
                    return new ApplicationEntityPersistenceService().InsertInternal(context, data.Convert<ApplicationEntity>(), principal);
                case EntityClassKeyStrings.Person:
                    return new PersonPersistenceService().InsertInternal(context, data.Convert<Person>(), principal);
                case EntityClassKeyStrings.Patient:
                    return new PatientPersistenceService().InsertInternal(context, data.Convert<Patient>(), principal);
                case EntityClassKeyStrings.Provider:
                    return new ProviderPersistenceService().InsertInternal(context, data.Convert<Provider>(), principal);
                case EntityClassKeyStrings.Place:
                case EntityClassKeyStrings.CityOrTown:
                case EntityClassKeyStrings.Country:
                case EntityClassKeyStrings.CountyOrParish:
                case EntityClassKeyStrings.State:
                case EntityClassKeyStrings.ServiceDeliveryLocation:
                    return new PlacePersistenceService().InsertInternal(context, data.Convert<Place>(), principal);
                case EntityClassKeyStrings.Organization:
                    return new OrganizationPersistenceService().InsertInternal(context, data.Convert<Organization>(), principal);
                case EntityClassKeyStrings.Material:
                    return new MaterialPersistenceService().InsertInternal(context, data.Convert<Material>(), principal);
                case EntityClassKeyStrings.ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().InsertInternal(context, data.Convert<ManufacturedMaterial>(), principal);
                default:
                    return this.InsertCoreProperties(context, data, principal);

            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        public override Entity UpdateInternal(DataContext context, Entity data, IPrincipal principal)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case EntityClassKeyStrings.Device:
                    return new DeviceEntityPersistenceService().UpdateInternal(context, data.Convert<DeviceEntity>(), principal);
                case EntityClassKeyStrings.NonLivingSubject:
                    return new ApplicationEntityPersistenceService().UpdateInternal(context, data.Convert<ApplicationEntity>(), principal);
                case EntityClassKeyStrings.Person:
                    return new PersonPersistenceService().UpdateInternal(context, data.Convert<Person>(), principal);
                case EntityClassKeyStrings.Patient:
                    return new PatientPersistenceService().UpdateInternal(context, data.Convert<Patient>(), principal);
                case EntityClassKeyStrings.Provider:
                    return new ProviderPersistenceService().UpdateInternal(context, data.Convert<Provider>(), principal);
                case EntityClassKeyStrings.Place:
                case EntityClassKeyStrings.CityOrTown:
                case EntityClassKeyStrings.Country:
                case EntityClassKeyStrings.CountyOrParish:
                case EntityClassKeyStrings.State:
                case EntityClassKeyStrings.ServiceDeliveryLocation:
                    return new PlacePersistenceService().UpdateInternal(context, data.Convert<Place>(), principal);
                case EntityClassKeyStrings.Organization:
                    return new OrganizationPersistenceService().UpdateInternal(context, data.Convert<Organization>(), principal);
                case EntityClassKeyStrings.Material:
                    return new MaterialPersistenceService().UpdateInternal(context, data.Convert<Material>(), principal);
                case EntityClassKeyStrings.ManufacturedMaterial:
                    return new ManufacturedMaterialPersistenceService().UpdateInternal(context, data.Convert<ManufacturedMaterial>(), principal);
                default:
                    return this.UpdateCoreProperties(context, data, principal);

            }
        }
        
    }
}
