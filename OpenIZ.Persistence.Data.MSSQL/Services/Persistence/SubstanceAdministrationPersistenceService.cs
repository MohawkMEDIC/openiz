using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents a persistence service for substance administrations
    /// </summary>
    public class SubstanceAdministrationPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.SubstanceAdministration, Data.SubstanceAdministration>
    {
        /// <summary>
        /// Convert databased model to model
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbVersionedData;
            var dbSbadm = dataInstance as Data.SubstanceAdministration ?? context.GetTable<Data.SubstanceAdministration>().Where(o => o.ActVersionId == iddat.VersionId).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(a => a.ActVersionId == dbSbadm.ActVersionId).First();
            var retVal = m_actPersister.ToModelInstance<Core.Model.Acts.SubstanceAdministration>(dba, context, principal);

            if (dbSbadm.DoseUnitConceptId != null)
                retVal.DoseUnitKey = dbSbadm.DoseUnitConceptId;
            if (dbSbadm.RouteConceptId != null)
                retVal.RouteKey = dbSbadm.RouteConceptId;
            retVal.DoseQuantity = dbSbadm.DoseQuantity;
            retVal.SequenceId = (int)dbSbadm.SequenceId;
            
            return retVal;
        }

        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration Insert(ModelDataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            data.DoseUnit?.EnsureExists(context, principal);
            data.Route?.EnsureExists(context, principal);
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Insert(context, data, principal);
        }


        /// <summary>
        /// Insert the specified sbadm
        /// </summary>
        public override Core.Model.Acts.SubstanceAdministration Update(ModelDataContext context, Core.Model.Acts.SubstanceAdministration data, IPrincipal principal)
        {
            data.DoseUnit?.EnsureExists(context, principal);
            data.Route?.EnsureExists(context, principal);
            data.DoseUnitKey = data.DoseUnit?.Key ?? data.DoseUnitKey;
            data.RouteKey = data.Route?.Key ?? data.RouteKey;
            return base.Update(context, data, principal);
        }
    }
}