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
