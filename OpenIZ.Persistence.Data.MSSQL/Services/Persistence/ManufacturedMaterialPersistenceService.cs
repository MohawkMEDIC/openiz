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
 * Date: 2016-6-28
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Manufactured material persistence service
    /// </summary>
    public class ManufacturedMaterialPersistenceService : IdentifiedPersistenceService<Core.Model.Entities.ManufacturedMaterial, Data.ManufacturedMaterial>
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
        public override Core.Model.Entities.ManufacturedMaterial ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {

            var idp = dataInstance as IDbVersionedData;
            var domainMat = dataInstance as Data.ManufacturedMaterial ?? context.GetTable<Data.ManufacturedMaterial>().Where(o => o.EntityVersionId == idp.VersionId).First();
            var dbm = context.GetTable<Data.Material>().FirstOrDefault(o => o.EntityVersionId == domainMat.EntityVersionId);
            var retVal = this.m_materialPersister.ToModelInstance<Core.Model.Entities.ManufacturedMaterial>(dbm, context, principal);
            retVal.LotNumber = domainMat.LotNumber;
            return retVal;

        }

        /// <summary>
        /// Insert the specified manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial Insert(ModelDataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var retVal = this.m_materialPersister.Insert(context, data, principal);
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Updates the manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial Update(ModelDataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var updated = this.m_materialPersister.Update(context, data, principal);
            return base.Update(context, data, principal);
        }

        /// <summary>
        /// Obsolete the specified manufactured material
        /// </summary>
        public override Core.Model.Entities.ManufacturedMaterial Obsolete(ModelDataContext context, Core.Model.Entities.ManufacturedMaterial data, IPrincipal principal)
        {
            var obsoleted = this.m_materialPersister.Obsolete(context, data, principal);
            return data;
        }
    }
}
