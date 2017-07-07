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

namespace OpenIZ.Persistence.Data.ADO.Services.Persistence
{
    /// <summary>
    /// Persistence class which persists encounters
    /// </summary>
    public class EncounterPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.PatientEncounter, DbPatientEncounter>
    {

        /// <summary>
        /// Convert database instance to patient encounter
        /// </summary>
        public Core.Model.Acts.PatientEncounter ToModelInstance(DbPatientEncounter dbEnc, DbActVersion actVersionInstance, DbAct actInstance, DataContext context, IPrincipal principal)
        {
            var retVal = m_actPersister.ToModelInstance<Core.Model.Acts.PatientEncounter>(actVersionInstance, actInstance, context, principal);
            if (retVal == null) return null;
            else if (dbEnc == null)
            {
                this.m_tracer.TraceEvent(System.Diagnostics.TraceEventType.Warning, -0493043, "ENC is missing ENC data: {0}", actInstance.Key);
                return null;
            }

            if (dbEnc.DischargeDispositionKey != null)
                retVal.DischargeDispositionKey = dbEnc.DischargeDispositionKey;
            return retVal;
        }

        /// <summary>
        /// Insert the patient encounter
        /// </summary>
        public override Core.Model.Acts.PatientEncounter InsertInternal(DataContext context, Core.Model.Acts.PatientEncounter data, IPrincipal principal)
        {
            if(data.DischargeDisposition != null) data.DischargeDisposition = data.DischargeDisposition?.EnsureExists(context, principal) as Concept;
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.InsertInternal(context, data, principal);
        }

        /// <summary>
        /// Updates the specified data
        /// </summary>
        public override Core.Model.Acts.PatientEncounter UpdateInternal(DataContext context, Core.Model.Acts.PatientEncounter data, IPrincipal principal)
        {
            if (data.DischargeDisposition != null) data.DischargeDisposition = data.DischargeDisposition?.EnsureExists(context, principal) as Concept;
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.UpdateInternal(context, data, principal);
        }
    }
}