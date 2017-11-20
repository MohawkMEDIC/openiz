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
using System.Data.Linq;
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

            var iddat = dataInstance as IDbVersionedData;
            var place = dataInstance as Data.Place ?? context.GetTable<Data.Place>().Where(o => o.EntityVersionId == iddat.VersionId).FirstOrDefault();
            var dbe = dataInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == place.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.Place>(dbe, context, principal);
            retVal.IsMobile = place?.MobileInd == true;
            retVal.Lat = place?.Lat;
            retVal.Lng = place?.Lng;
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

        /// <summary>
        /// Get data load options
        /// </summary>
        /// <returns></returns>
        internal override DataLoadOptions GetDataLoadOptions()
        {
            var loadOptions = m_entityPersister.GetDataLoadOptions();
           // loadOptions.LoadWith<Data.Entity>(o=>o.PlaceServicesPlaceEntityId);

            return loadOptions;
        }
    }
}
