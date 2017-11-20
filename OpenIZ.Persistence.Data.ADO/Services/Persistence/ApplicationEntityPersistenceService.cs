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
    /// Represents the persistence service for application eneities
    /// </summary>
    public class ApplicationEntityPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.ApplicationEntity, DbApplicationEntity>
    {
        /// <summary>
        /// To model instance
        /// </summary>
        public Core.Model.Entities.ApplicationEntity ToModelInstance(DbApplicationEntity applicationEntity, DbEntityVersion entityVersion, DbEntity entity, DataContext context, IPrincipal principal)
        {

            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.ApplicationEntity>(entityVersion, entity, context, principal);

            if (retVal == null) return null;

            retVal.SecurityApplicationKey = applicationEntity.SecurityApplicationKey;
            retVal.SoftwareName = applicationEntity.SoftwareName;
            retVal.VersionName = applicationEntity.VersionName;
            retVal.VendorName = applicationEntity.VendorName;
            return retVal;
        }

        /// <summary>
        /// Insert the application entity
        /// </summary>
        public override Core.Model.Entities.ApplicationEntity InsertInternal(DataContext context, Core.Model.Entities.ApplicationEntity data, IPrincipal principal)
        {
            if(data.SecurityApplication != null) data.SecurityApplication = data.SecurityApplication?.EnsureExists(context, principal) as SecurityApplication;
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.InsertInternal(context, data, principal);
        }
        
        /// <summary>
        /// Update the application entity
        /// </summary>
        public override Core.Model.Entities.ApplicationEntity UpdateInternal(DataContext context, Core.Model.Entities.ApplicationEntity data, IPrincipal principal)
        {
            if(data.SecurityApplication != null) data.SecurityApplication = data.SecurityApplication?.EnsureExists(context, principal) as SecurityApplication;
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.UpdateInternal(context, data, principal);
        }
    }
}
