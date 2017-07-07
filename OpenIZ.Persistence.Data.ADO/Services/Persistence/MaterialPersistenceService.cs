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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.OrmLite;


namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Persistence service for matrials
    /// </summary>
    public class MaterialPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Material, DbMaterial>
    {

        /// <summary>
        /// Creates the specified model instance
        /// </summary>
        public TModel ToModelInstance<TModel>(DbMaterial dbMat, DbEntityVersion dbEntVersion, DbEntity dbEnt, DataContext context, IPrincipal principal)
            where TModel : Core.Model.Entities.Material, new()
        {
            var retVal = this.m_entityPersister.ToModelInstance<TModel>(dbEntVersion, dbEnt, context, principal);
            if (retVal == null) return null;

            retVal.ExpiryDate = dbMat.ExpiryDate;
            retVal.IsAdministrative = dbMat.IsAdministrative;
            retVal.Quantity = dbMat.Quantity;
            retVal.QuantityConceptKey = dbMat.QuantityConceptKey;
            retVal.FormConceptKey = dbMat.FormConceptKey;
            return retVal;

        }

        /// <summary>
        /// Insert the material
        /// </summary>
        public override Core.Model.Entities.Material InsertInternal(DataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            if(data.FormConcept != null) data.FormConcept = data.FormConcept?.EnsureExists(context, principal) as Concept;
            if(data.QuantityConcept != null) data.QuantityConcept = data.QuantityConcept?.EnsureExists(context, principal) as Concept;
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the specified material
        /// </summary>
        public override Core.Model.Entities.Material UpdateInternal(DataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            if (data.FormConcept != null) data.FormConcept = data.FormConcept?.EnsureExists(context, principal) as Concept;
            if (data.QuantityConcept != null) data.QuantityConcept = data.QuantityConcept?.EnsureExists(context, principal) as Concept;
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.UpdateInternal(context, data, principal);
        }

    }
}
