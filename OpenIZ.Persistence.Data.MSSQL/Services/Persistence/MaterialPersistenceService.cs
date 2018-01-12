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
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Persistence service for matrials
    /// </summary>
    public class MaterialPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Material, Data.Material>
    {

        /// <summary>
        /// Convert persistence model to business objects
        /// </summary>
        public override Core.Model.Entities.Material ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            return this.ToModelInstance<Core.Model.Entities.Material>(dataInstance, context, principal);
        }

        /// <summary>
        /// Creates the specified model instance
        /// </summary>
        internal TModel ToModelInstance<TModel>(Object rawInstance, ModelDataContext context, IPrincipal principal)
            where TModel : Core.Model.Entities.Material, new()
        {
            var iddat = rawInstance as IDbVersionedData;
            var dataInstance = rawInstance as Data.Material ?? context.GetTable<Data.Material>().Where(o => o.EntityVersionId == iddat.VersionId).First();
            var dbe = rawInstance as Data.EntityVersion ?? context.GetTable<Data.EntityVersion>().FirstOrDefault(o => o.EntityVersionId == dataInstance.EntityVersionId);
            var retVal = this.m_entityPersister.ToModelInstance<TModel>(dbe, context, principal);
            retVal.ExpiryDate = dataInstance.ExpiryDate;
            retVal.IsAdministrative = dataInstance.IsAdministrative;
            retVal.Quantity = dataInstance.Quantity;
            retVal.QuantityConceptKey = dataInstance.QuantityConceptId;
            retVal.FormConceptKey = dataInstance.FormConceptId;
            return retVal;

        }

        /// <summary>
        /// Insert the material
        /// </summary>
        public override Core.Model.Entities.Material Insert(ModelDataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            data.FormConcept?.EnsureExists(context, principal);
            data.QuantityConcept?.EnsureExists(context, principal);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified material
        /// </summary>
        public override Core.Model.Entities.Material Update(ModelDataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            data.FormConcept?.EnsureExists(context, principal);
            data.QuantityConcept?.EnsureExists(context, principal);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Update(context, data, principal);
        }

    }
}
