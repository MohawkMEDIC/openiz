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
using OpenIZ.Core.Model.Security;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
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
    /// Represents a persister that can read/write user entities
    /// </summary>
    public class UserEntityPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.UserEntity, DbUserEntity, CompositeResult<DbUserEntity, DbPerson, DbEntityVersion, DbEntity>>
    {

        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
      
        /// <summary>
        /// To model instance
        /// </summary>
        public Core.Model.Entities.UserEntity ToModelInstance(DbUserEntity userEntityInstance, DbPerson personInstance, DbEntityVersion entityVersionInstance, DbEntity entityInstance, DataContext context, IPrincipal principal)
        {

            var retVal = this.m_entityPersister.ToModelInstance<UserEntity>(entityVersionInstance, entityInstance, context, principal);
            if (retVal == null) return null;

            // Copy from person 
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

            retVal.SecurityUserKey = userEntityInstance.SecurityUserKey;
            return retVal;
        }
        
        /// <summary>
        /// Inserts the user entity
        /// </summary>
        public override Core.Model.Entities.UserEntity InsertInternal(DataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            if(data.SecurityUser != null) data.SecurityUser = data.SecurityUser?.EnsureExists(context, principal) as SecurityUser;
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            var inserted = this.m_personPersister.InsertInternal(context, data, principal);

            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the specified user entity
        /// </summary>
        public override Core.Model.Entities.UserEntity UpdateInternal(DataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            if (data.SecurityUser != null) data.SecurityUser = data.SecurityUser?.EnsureExists(context, principal) as SecurityUser;
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            this.m_personPersister.UpdateInternal(context, data, principal);
            return base.UpdateInternal(context, data, principal);
        }

        /// <summary>
        /// Obsolete the specified user instance
        /// </summary>
        public override Core.Model.Entities.UserEntity ObsoleteInternal(DataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            var retVal = this.m_personPersister.ObsoleteInternal(context, data, principal);
            return data;
        }
    }
}
