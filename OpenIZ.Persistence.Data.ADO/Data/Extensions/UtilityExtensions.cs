using OpenIZ.Persistence.Data.ADO.Data.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.Extensions
{
    /// <summary>
    /// Represents utility extensions on common .net types
    /// </summary>
    public static class UtilityExtensions
    {

        /// <summary>
        /// Get the user identifier from the authorization context
        /// </summary>
        /// <param name="principal">The current authorization context</param>
        /// <param name="dataContext">The context under which the get operation should be completed</param>
        /// <returns>The UUID of the user which the authorization context subject represents</returns>
        public static Guid? GetUserKey(this IPrincipal principal, DataContext dataContext)
        {

            if (principal == null)
                return null;

            var user = dataContext.SingleOrDefault<DbSecurityUser>(o => o.UserName == principal.Identity.Name && o.ObsoletionTime == null);
            // TODO: Enable auto-creation of users via configuration
            if (user == null)
                throw new SecurityException("User in authorization context does not exist or is obsolete");

            return user.Key;

        }
    }
}
