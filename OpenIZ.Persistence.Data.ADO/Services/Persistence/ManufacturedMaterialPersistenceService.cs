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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.OrmLite;


namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Manufactured material persistence service
    /// </summary>
    public class ManufacturedMaterialPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.ManufacturedMaterial, DbManufacturedMaterial, CompositeResult<DbManufacturedMaterial, DbMaterial, DbEntityVersion, DbEntity>>
    {
        // Material persister
        private MaterialPersistenceService m_materialPersister = new MaterialPersistenceService();

        /// <summary>
        /// Material persister
        /// </summary>
        /// <param name="dataInstance"></param>
        /// <param name="context"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public Core.Model.Entities.ManufacturedMaterial ToModelInstance(DbManufacturedMaterial dbMmat, DbMaterial dbMat, DbEntityVersion dbEntityVersion, DbEntity dbEntity, DataContext context, IPrincipal principal)
        {

            var retVal = this.m_materialPersister.ToModelInstance<Core.Model.Entities.ManufacturedMaterial>(dbMat, dbEntityVersion, dbEntity, context, principal);
            if (retVal == null) return null;

            retVal.LotNumber = dbMmat.LotNumber;
            return retVal;

        }

        /// <summary>
        /// Insert the specified manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial InsertInternal(DataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var retVal = this.m_materialPersister.InsertInternal(context, data, principal);
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Updates the manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial UpdateInternal(DataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var updated = this.m_materialPersister.UpdateInternal(context, data, principal);
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Obsolete the specified manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial ObsoleteInternal(DataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var obsoleted = this.m_materialPersister.ObsoleteInternal(context, data, principal);
            return base.InsertInternal(context, data, principal) ;
        }
    }
}
