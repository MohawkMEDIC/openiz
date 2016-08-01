using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Assigning authority persistence service
    /// </summary>
    public class AssigningAuthorityPersistenceService : BaseDataPersistenceService<Core.Model.DataTypes.AssigningAuthority, Data.AssigningAuthority>
    {
        /// <summary>
        /// Convert assigning authority to model
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var dataAA = dataInstance as Data.AssigningAuthority;
            var retVal = base.ToModelInstance(dataInstance, context, principal);
            retVal.AuthorityScopeXml = dataAA.AssigningAuthorityScopes.Select(o => o.ScopeConceptId).ToList();
            return retVal;
        }

        /// <summary>
        /// Insert the specified data
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority Insert(ModelDataContext context, Core.Model.DataTypes.AssigningAuthority data, IPrincipal principal)
        {
            var retVal = base.Insert(context, data, principal);

            // Scopes?
            if(retVal.AuthorityScopeXml != null)
                context.AssigningAuthorityScopes.InsertAllOnSubmit(retVal.AuthorityScopeXml.Select(o => new AssigningAuthorityScope() { ScopeConceptId = o, AssigningAuthorityId = retVal.Key.Value }));
            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        public override Core.Model.DataTypes.AssigningAuthority Update(ModelDataContext context, Core.Model.DataTypes.AssigningAuthority data, IPrincipal principal)
        {
            var retVal = base.Update(context, data, principal);

            // Scopes?
            if (retVal.AuthorityScopeXml != null)
            {
                context.AssigningAuthorityScopes.DeleteAllOnSubmit(context.AssigningAuthorityScopes.Where(o => o.AssigningAuthorityId == retVal.Key.Value));
                context.AssigningAuthorityScopes.InsertAllOnSubmit(retVal.AuthorityScopeXml.Select(o => new AssigningAuthorityScope() { ScopeConceptId = o, AssigningAuthorityId = retVal.Key.Value }));
            }
            return retVal;
        }

    }
}
