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
            var iddat = dataInstance as IDbIdentified;
            var dbEnc = dataInstance as Data.PatientEncounter ?? context.GetTable<Data.PatientEncounter>().Where(o => o.ActVersionId == iddat.Id).First();
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