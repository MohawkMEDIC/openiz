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
 * Date: 2016-8-3
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.OrmLite;
using System;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for substance administrations
    /// </summary>
    public class SubstanceAdministrationPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.SubstanceAdministration,DbSubstanceAdministration>
    {
        /// <summary>
        /// Convert databased model to model
        /// </summary>
        public Core.Model.Acts.SubstanceAdministration ToModelInstance(DbSubstanceAdministration sbadmInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = m_actPersister.ToModelInstance<Core.Model.Acts.SubstanceAdministration>(actVersionInstance, actInstance, context, principal);

            if (sbadmInstance.DoseUnitConceptKey != null)
                retVal.DoseUnitKey = sbadmInstance.DoseUnitConceptKey;
            if (sbadmInstance.RouteConceptKey != null)
                retVal.RouteKey = sbadmInstance.RouteConceptKey;
            retVal.DoseQuantity = sbadmInstance.DoseQuantity;
            retVal.SequenceId = (int)sbadmInstance.SequenceId;
            
            return retVal;
        }

        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration Insert(DataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            if(data.DoseUnit != null) data.DoseUnit = data.DoseUnit?.EnsureExists(context, principal) as Concept;
            if(data.Route != null) data.Route = data.Route?.EnsureExists(context, principal) as Concept;
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Insert(context, data, principal);
        }


        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration Update(DataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            if (data.DoseUnit != null) data.DoseUnit = data.DoseUnit?.EnsureExists(context, principal) as Concept;
            if (data.Route != null) data.Route = data.Route?.EnsureExists(context, principal) as Concept;
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Update(context, data, principal);
        }
    }
}