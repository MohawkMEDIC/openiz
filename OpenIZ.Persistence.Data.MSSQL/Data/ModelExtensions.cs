using OpenIZ.Persistence.Data.MSSQL.Services.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Data
{
    /// <summary>
    /// Model extension methods
    /// </summary>
    public static class ModelExtensions
    {

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.Security.SecurityRole EnsureExists(this Core.Model.Security.SecurityRole me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
                return new SecurityRolePersistenceService().Insert(me, principal, context);
            return me;
        }

        /// <summary>
        /// Ensure that the role exists
        /// </summary>
        public static Core.Model.Security.SecurityUser EnsureExists(this Core.Model.Security.SecurityUser me, IPrincipal principal, Data.ModelDataContext context)
        {
            if (me.Key == Guid.Empty)
                return new SecurityUserPersistenceService().Insert(me, principal, context);
            return me;
        }
    }
}
