using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{

    /// <summary>
    /// Application identity
    /// </summary>
    public class ApplicationIdentity : IIdentity
    {

        /// <summary>
        /// Application identity ctor
        /// </summary>
        public ApplicationIdentity(Guid name, Boolean isAuthenticated)
        {
            this.Name = name.ToString();
            this.IsAuthenticated = isAuthenticated;
        }

        /// <summary>
        /// Identity for an application
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "SYSTEM";
            }
        }

        /// <summary>
        /// True if is authenticated
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; private set; }
    }
    /// <summary>
    /// Represents an IPrincipal related to an application
    /// </summary>
    public class ApplicationPrincipal : IPrincipal
    {

        /// <summary>
        /// Application principal
        /// </summary>
        public ApplicationPrincipal(IIdentity identity)
        {
            this.Identity = identity;
        }

        /// <summary>
        /// Gets the identity
        /// </summary>
        public IIdentity Identity { get; private set; }

        /// <summary>
        /// Is in role?
        /// </summary>
        public bool IsInRole(string role)
        {
            return false;
        }
    }
}
