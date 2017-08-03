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
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.ADO.Data;
using OpenIZ.Persistence.Data.ADO.Data.Model;
using OpenIZ.Persistence.Data.ADO.Data.Model.Acts;
using OpenIZ.OrmLite;
using System;
using System.Linq;
using System.Security.Principal;
using OpenIZ.Core.Model.Constants;

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
            if (retVal == null) return null;
            else if(sbadmInstance == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Warning, -0493043, "SBADM is missing SBADM data: {0}", actInstance.Key);
                return null;
            }

            if (sbadmInstance.DoseUnitConceptKey != null)
                retVal.DoseUnitKey = sbadmInstance.DoseUnitConceptKey;
            if (sbadmInstance.RouteConceptKey != null)
                retVal.RouteKey = sbadmInstance.RouteConceptKey;
            if (sbadmInstance.SiteConceptKey != null)
                retVal.SiteKey = sbadmInstance.SiteConceptKey;

            retVal.DoseQuantity = sbadmInstance.DoseQuantity;
            retVal.SequenceId = (int)sbadmInstance.SequenceId;
            
            return retVal;
        }

        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration InsertInternal(DataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            if(data.DoseUnit != null) data.DoseUnit = data.DoseUnit?.EnsureExists(context, principal) as Concept;
            if (data.Route != null) data.Route = data.Route?.EnsureExists(context, principal) as Concept;
            else
                data.RouteKey = NullReasonKeys.NoInformation;

            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.InsertInternal(context, data, principal);
        }


        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration UpdateInternal(DataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            if (data.DoseUnit != null) data.DoseUnit = data.DoseUnit?.EnsureExists(context, principal) as Concept;
            if (data.Route != null) data.Route = data.Route?.EnsureExists(context, principal) as Concept;
            else
                data.RouteKey = NullReasonKeys.NoInformation;

            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.UpdateInternal(context, data, principal);
        }
    }
}