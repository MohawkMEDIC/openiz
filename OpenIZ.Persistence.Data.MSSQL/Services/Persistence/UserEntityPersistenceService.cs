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
 * Date: 2016-8-2
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a persister that can read/write user entities
    /// </summary>
    public class UserEntityPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.UserEntity, Data.UserEntity>
    {

        // Entity persisters
        private PersonPersistenceService m_personPersister = new PersonPersistenceService();
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();

      
        /// <summary>
        /// To model instance
        /// </summary>
        public override Core.Model.Entities.UserEntity ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var userEntity = dataInstance as Data.UserEntity;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == userEntity.EntityVersionId).First();
            var dbp = context.GetTable<Data.Person>().Where(o => o.EntityVersionId == userEntity.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.UserEntity>(dbe, context, principal);
            retVal.SecurityUserKey = userEntity.UserId;
            retVal.DateOfBirth = dbp.DateOfBirth;
            // Reverse lookup
            if (dbp.DateOfBirthPrecision.HasValue)
                retVal.DateOfBirthPrecision = PersonPersistenceService.PrecisionMap.Where(o => o.Value == dbp.DateOfBirthPrecision).Select(o => o.Key).First();
            return retVal;
        }
        
        /// <summary>
        /// Inserts the user entity
        /// </summary>
        public override Core.Model.Entities.UserEntity Insert(ModelDataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            data.SecurityUser?.EnsureExists(context, principal);
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            var inserted = this.m_personPersister.Insert(context, data, principal);

            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified user entity
        /// </summary>
        public override Core.Model.Entities.UserEntity Update(ModelDataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            data.SecurityUser?.EnsureExists(context, principal);
            data.SecurityUserKey = data.SecurityUser?.Key ?? data.SecurityUserKey;
            this.m_personPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the specified user instance
        /// </summary>
        public override Core.Model.Entities.UserEntity Obsolete(ModelDataContext context, Core.Model.Entities.UserEntity data, IPrincipal principal)
        {
            var retVal = this.m_personPersister.Obsolete(context, data, principal);
            return data;
        }
    }
}
