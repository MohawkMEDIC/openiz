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
    /// Represents the persistence service for application eneities
    /// </summary>
    public class ApplicationEntityPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.ApplicationEntity, Data.ApplicationEntity>
    {
        /// <summary>
        /// To model instance
        /// </summary>
        public override Core.Model.Entities.ApplicationEntity ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {

            var idp = dataInstance as IDbVersionedData;
            var applicationEntity = dataInstance as Data.ApplicationEntity ?? context.GetTable<Data.ApplicationEntity>().Where(o => o.EntityVersionId == idp.VersionId).First();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == applicationEntity.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.ApplicationEntity>(dbe, context, principal);
            retVal.SecurityApplicationKey = applicationEntity.ApplicationId;
            retVal.SoftwareName = applicationEntity.SoftwareName;
            retVal.VersionName = applicationEntity.VersionName;
            retVal.VendorName = applicationEntity.VendorName;
            return retVal;
        }

        /// <summary>
        /// Insert the application entity
        /// </summary>
        public override Core.Model.Entities.ApplicationEntity Insert(ModelDataContext context, Core.Model.Entities.ApplicationEntity data, IPrincipal principal)
        {
            data.SecurityApplication?.EnsureExists(context, principal);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.Insert(context, data, principal);
        }
        
        /// <summary>
        /// Update the application entity
        /// </summary>
        public override Core.Model.Entities.ApplicationEntity Update(ModelDataContext context, Core.Model.Entities.ApplicationEntity data, IPrincipal principal)
        {
            data.SecurityApplication?.EnsureExists(context, principal);
            data.SecurityApplicationKey = data.SecurityApplication?.Key ?? data.SecurityApplicationKey;
            return base.Update(context, data, principal);
        }
    }
}
