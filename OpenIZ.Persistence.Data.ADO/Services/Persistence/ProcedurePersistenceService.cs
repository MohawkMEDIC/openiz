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
    public class ProcedurePersistenceService : ActDerivedPersistenceService<Core.Model.Acts.Procedure,DbProcedure>
    {
        /// <summary>
        /// Convert databased model to model
        /// </summary>
        public Core.Model.Acts.Procedure ToModelInstance(DbProcedure procedureInstance, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = m_actPersister.ToModelInstance<Core.Model.Acts.Procedure>(actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;
            else if(procedureInstance == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Warning, -0493043, "PROC is missing PROC data: {0}", actInstance.Key);
                return null;
            }

            if (procedureInstance.MethodConceptKey != null)
                retVal.MethodKey = procedureInstance.MethodConceptKey;
            if (procedureInstance.ApproachSiteConceptKey != null)
                retVal.ApproachSiteKey = procedureInstance.ApproachSiteConceptKey;
            if (procedureInstance.TargetSiteConceptKey != null)
                retVal.TargetSiteKey = procedureInstance.TargetSiteConceptKey;

            return retVal;
        }

        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.Procedure InsertInternal(DataContext context, Core.Model.Acts.Procedure data, IPrincipal principal)
        {
            if (data.Method != null) data.Method = data.Method?.EnsureExists(context, principal) as Concept;
            else if(!data.MethodKey.HasValue)
                data.MethodKey = NullReasonKeys.NoInformation;

            if (data.ApproachSite != null) data.ApproachSite = data.ApproachSite?.EnsureExists(context, principal) as Concept;
            if (data.TargetSite != null) data.TargetSite = data.TargetSite?.EnsureExists(context, principal) as Concept;

            // JF: Correct dose unit key
             data.MethodKey = data.Method?.Key ?? data.MethodKey;
            data.ApproachSiteKey = data.ApproachSite?.Key ?? data.ApproachSiteKey;
            data.TargetSiteKey = data.TargetSite?.Key ?? data.TargetSiteKey;

            return base.InsertInternal(context, data, principal);
        }


        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.Procedure UpdateInternal(DataContext context, Core.Model.Acts.Procedure data, IPrincipal principal)
        {
            if (data.Method != null) data.Method = data.Method?.EnsureExists(context, principal) as Concept;
            else if (!data.MethodKey.HasValue)
                data.MethodKey = NullReasonKeys.NoInformation;

            if (data.ApproachSite != null) data.ApproachSite = data.ApproachSite?.EnsureExists(context, principal) as Concept;
            if (data.TargetSite != null) data.TargetSite = data.TargetSite?.EnsureExists(context, principal) as Concept;

            // JF: Correct dose unit key
            data.MethodKey = data.Method?.Key ?? data.MethodKey;
            data.ApproachSiteKey = data.ApproachSite?.Key ?? data.ApproachSiteKey;
            data.TargetSiteKey = data.TargetSite?.Key ?? data.TargetSiteKey;

            return base.UpdateInternal(context, data, principal);
        }
    }
}