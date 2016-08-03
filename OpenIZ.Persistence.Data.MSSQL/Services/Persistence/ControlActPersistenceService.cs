using OpenIZ.Core.Model.Acts;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Linq;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Control act persistence service
    /// </summary>
    public class ControlActPersistenceService : ActDerivedPersistenceService<Core.Model.Acts.ControlAct, Data.ControlAct>
    {
        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override Core.Model.Acts.ControlAct ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var iddat = dataInstance as IDbIdentified;
            var controlAct = dataInstance as Data.ControlAct ?? context.GetTable<Data.ControlAct>().Where(o => o.ActVersionId == iddat.Id).First();
            var dba = dataInstance as Data.ActVersion ?? context.GetTable<Data.ActVersion>().Where(a => a.ActVersionId == controlAct.ActVersionId).First();
            // TODO: Any other cact fields
            return m_actPersister.ToModelInstance<Core.Model.Acts.ControlAct>(dba, context, principal);
        }
    }
}