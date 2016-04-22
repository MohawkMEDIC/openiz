using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Security repository service is responsible for the maintenance of security entities
    /// </summary>
    public interface ISecurityRepositoryService
    {

        /// <summary>
        /// Create the specified user with specified password
        /// </summary>
        SecurityUser CreateUser(SecurityUser userInfo, String password);

        /// <summary>
        /// Obsolete a user
        /// </summary>
        SecurityUser ObsoleteUser(Guid userId);

        /// <summary>
        /// Change user's password
        /// </summary>
        SecurityUser ChangePassword(Guid userId, String password);

        /// <summary>
        /// Lock a user
        /// </summary>
        void LockUser(Guid userId);

        /// <summary>
        /// Unlock a user
        /// </summary>
        void UnlockUser(Guid userId);

        /// <summary>
        /// Gets the specified security user based on the principal
        /// </summary>
        SecurityUser GetUser(IIdentity identity);

        /// <summary>
        /// Get the specified user
        /// </summary>
        SecurityUser GetUser(Guid userId);

        /// <summary>
        /// Finds the specified users
        /// </summary>
        IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query);

        /// <summary>
        /// Finds the specified users matching the query 
        /// </summary>
        IEnumerable<SecurityUser> FindUsers(Expression<Func<SecurityUser, bool>> query, int offset, int? count, out int total);

        /// <summary>
        /// Save the specified security user
        /// </summary>
        SecurityUser SaveUser(SecurityUser user);

    }
}
