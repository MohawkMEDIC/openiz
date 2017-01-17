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
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for a device entity
    /// </summary>
    public class DeviceEntityPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.DeviceEntity, DbDeviceEntity>
    {

        /// <summary>
        /// Convert the database representation to a model instance
        /// </summary>
        public Core.Model.Entities.DeviceEntity ToModelInstance(DbDeviceEntity deviceEntity, DbEntityVersion entityVersion, DbEntity entity, DataContext context, IPrincipal principal)
        {
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.DeviceEntity>(entityVersion, entity, context, principal);
            retVal.SecurityDeviceKey = deviceEntity.SecurityDeviceKey;
            retVal.ManufacturedModelName = deviceEntity.ManufacturerModelName;
            retVal.OperatingSystemName = deviceEntity.OperatingSystemName;
            return retVal;
        }

        /// <summary>
        /// Insert the specified device entity
        /// </summary>
        public override Core.Model.Entities.DeviceEntity Insert(Data.DataContext context, Core.Model.Entities.DeviceEntity data, IPrincipal principal)
        {
            data.SecurityDevice?.EnsureExists(context, principal);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;

            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Updates the specified user
        /// </summary>
        public override Core.Model.Entities.DeviceEntity Update(DataContext context, Core.Model.Entities.DeviceEntity data, IPrincipal principal)
        {
            data.SecurityDevice?.EnsureExists(context, principal);
            data.SecurityDeviceKey = data.SecurityDevice?.Key ?? data.SecurityDeviceKey;
            return base.Update(context, data, principal);
        }
    }
}
