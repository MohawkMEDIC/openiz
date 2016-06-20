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
            var deviceEntity = dataInstance as Data.DeviceEntity;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == deviceEntity.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.DeviceEntity>(dbe, context, principal);
            retVal.SecurityDeviceKey = deviceEntity.DeviceId;
            retVal.ManufacturedModelName = deviceEntity.ManufacturedModelName;
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
