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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data.Model.Roles;
using OpenIZ.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Provider persistence service
    /// </summary>
    public class ProviderPersistenceService : EntityDerivedPersistenceService<Core.Model.Roles.Provider, DbProvider>
    {
        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();

        /// <summary>
        /// Model instance
        /// </summary>
        public Core.Model.Roles.Provider ToModelInstance(DbProvider providerInstance, DbPerson personInstance, DbEntityVersion entityVersionInstance, DbEntity entityInstance, DataContext context, IPrincipal principal)
        {
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Roles.Provider>(entityVersionInstance, entityInstance, context, principal);
            if (retVal == null) return null;

            retVal.DateOfBirth = personInstance?.DateOfBirth;

            // Reverse lookup
            if (!String.IsNullOrEmpty(personInstance?.DateOfBirthPrecision))
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == personInstance.DateOfBirthPrecision).Select(o => o.Key).First();

            retVal.LanguageCommunication = context.Query<DbPersonLanguageCommunication>(v => v.SourceKey == entityInstance.Key && v.EffectiveVersionSequenceId <= entityVersionInstance.VersionSequenceId && (v.ObsoleteVersionSequenceId == null || v.ObsoleteVersionSequenceId >= entityVersionInstance.VersionSequenceId))
                    .Select(o => new Core.Model.Entities.PersonLanguageCommunication(o.LanguageCode, o.IsPreferred)
                    {
                        Key = o.Key
                    })
                    .ToList();
            retVal.ProviderSpecialtyKey = providerInstance?.Specialty;

            return retVal;
        }

        /// <summary>
        /// Insert the specified person into the database
        /// </summary>
        public override Core.Model.Roles.Provider InsertInternal(DataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            if(data.ProviderSpecialty != null) data.ProviderSpecialty = data.ProviderSpecialty?.EnsureExists(context, principal) as Concept;
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            var inserted = this.m_personPersister.InsertInternal(context, data, principal);
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the specified person
        /// </summary>
        public override Core.Model.Roles.Provider UpdateInternal(DataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            // Ensure exists
            if (data.ProviderSpecialty != null) data.ProviderSpecialty = data.ProviderSpecialty?.EnsureExists(context, principal) as Concept;
            data.ProviderSpecialtyKey = data.ProviderSpecialty?.Key ?? data.ProviderSpecialtyKey;

            this.m_personPersister.UpdateInternal(context, data, principal);
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Core.Model.Roles.Provider ObsoleteInternal(DataContext context, Core.Model.Roles.Provider data, IPrincipal principal)
        {
            var retVal = this.m_personPersister.ObsoleteInternal(context, data, principal);
            return data;
        }

    }
}
