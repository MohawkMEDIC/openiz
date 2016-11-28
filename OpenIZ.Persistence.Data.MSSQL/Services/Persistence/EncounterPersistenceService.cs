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
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Persistence class which persists encounters
    /// </summary>
    public class EncounterPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.PatientEncounter, Data.PatientEncounter>
    {

        /// <summary>
        /// Convert database instance to patient encounter
        /// </summary>
        public override Core.Model.Acts.PatientEncounter ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbVersionedData;
            var dbEnc = dataInstance as Data.PatientEncounter ?? context.GetTable<Data.PatientEncounter>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(a => a.ActVersionId == dbEnc.ActVersionId).First();
            var retVal = m_actPersister.ToModelInstance<Core.Model.Acts.PatientEncounter>(dba, context, principal);

            if (dbEnc.DischargeDispositionConceptId != null)
                retVal.DischargeDispositionKey = dbEnc.DischargeDispositionConceptId;
            return retVal;
        }

        /// <summary>
        /// Insert the patient encounter
        /// </summary>
        public override Core.Model.Acts.PatientEncounter Insert(ModelDataContext context, Core.Model.Acts.PatientEncounter data, IPrincipal principal)
        {
            data.DischargeDisposition?.EnsureExists(context, principal);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Updates the specified data
        /// </summary>
        public override Core.Model.Acts.PatientEncounter Update(ModelDataContext context, Core.Model.Acts.PatientEncounter data, IPrincipal principal)
        {
            data.DischargeDisposition?.EnsureExists(context, principal);
            data.DischargeDispositionKey = data.DischargeDisposition?.Key ?? data.DischargeDispositionKey;
            return base.Update(context, data, principal);
        }
    }
}