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
 * Date: 2017-9-1
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
    /// Represents a persistence service for a device entity
    /// </summary>
    public class DeviceEntityPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.DeviceEntity, Data.DeviceEntity>
    {
        /// <summary>
        /// Convert the database representation to a model instance
        /// </summary>
        public override Core.Model.Entities.DeviceEntity ToModelInstance(object dataInstance, Data.ModelDataContext context, IPrincipal principal)
        {
            var idp = dataInstance as IDbVersionedData;
            var deviceEntity = dataInstance as Data.DeviceEntity ?? context.GetTable<Data.DeviceEntity>().Where(o => o.EntityVersionId == idp.VersionId).First();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == deviceEntity.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.DeviceEntity>(dbe, context, principal);
            retVal.SecurityDeviceKey = deviceEntity.DeviceId;
            retVal.ManufacturerModelName = deviceEntity.ManufacturedModelName;
            retVal.OperatingSystemName = deviceEntity.OperatingSystemName;
            return retVal;
        }

        /// <summary>
        /// Insert the specified device entity
        /// </summary>
        public override Core.Model.Entities.DeviceEntity Insert(Data.ModelDataContext context, Core.Model.Entities.DeviceEntity data, IPrincipal principal)
        {
            data.SecurityDevice?.EnsureExists(context, principal);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;

            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Updates the specified user
        /// </summary>
        public override Core.Model.Entities.DeviceEntity Update(ModelDataContext context, Core.Model.Entities.DeviceEntity data, IPrincipal principal)
        {
            data.SecurityDevice?.EnsureExists(context, principal);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;
            return base.Update(context, data, principal);
        }
    }
}
