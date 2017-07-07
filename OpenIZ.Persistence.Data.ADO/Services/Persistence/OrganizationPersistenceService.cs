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
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Entities;
using OpenIZ.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents an organization persistence service
    /// </summary>
    public class OrganizationPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Organization, DbOrganization>
    {
       
        /// <summary>
        /// Model instance
        /// </summary>
        public Core.Model.Entities.Organization ToModelInstance(DbOrganization orgInstance, DbEntityVersion dbEntityVersion, DbEntity dbEntity, DataContext context, IPrincipal principal)
        {
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.Organization>(dbEntityVersion, dbEntity, context, principal);
            if (retVal == null) return null;
            retVal.IndustryConceptKey = orgInstance?.IndustryConceptKey;
            return retVal;
        }

        /// <summary>
        /// Insert the organization
        /// </summary>
        public override Core.Model.Entities.Organization InsertInternal(DataContext context, Core.Model.Entities.Organization data, IPrincipal principal)
        {
            // ensure industry concept exists
            if(data.IndustryConcept != null) data.IndustryConcept = data.IndustryConcept?.EnsureExists(context, principal) as Concept;
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;

            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Update the organization
        /// </summary>
        public override Core.Model.Entities.Organization UpdateInternal(DataContext context, Core.Model.Entities.Organization data, IPrincipal principal)
        {
            if (data.IndustryConcept != null) data.IndustryConcept = data.IndustryConcept?.EnsureExists(context, principal) as Concept;
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;
            return base.UpdateInternal(context, data, principal);
        }

    }
}
