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
    /// Represents a persister which persists places
    /// </summary>
    public class PlacePersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Place, Data.Place>
    {
        /// <summary>
        /// Load to a model instance
        /// </summary>
        public override Core.Model.Entities.Place ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var place = dataInstance as Data.Place;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == place.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.Place>(dbe, context, principal);
            retVal.IsMobile = place.MobileInd;
            retVal.Lat = place.Lat;
            retVal.Lng = place.Lng;
            return retVal;
        }

        /// <summary>
        /// Insert 
        /// </summary>
        public override Core.Model.Entities.Place Insert(ModelDataContext context, Core.Model.Entities.Place data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            if (data.Services != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PlaceService, Data.PlaceService>(
                    data.Services,
                    data,
                    context, 
                    principal);

            return retVal;
        }

        /// <summary>
        /// Update the place
        /// </summary>
        public override Core.Model.Entities.Place Update(ModelDataContext context, Core.Model.Entities.Place data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);

            if (data.Services != null)
                this.m_entityPersister.UpdateVersionedAssociatedItems<Core.Model.Entities.PlaceService, Data.PlaceService>(
                    data.Services,
                    data,
                    context,
                    principal);

            return retVal;
        }
    }
}
