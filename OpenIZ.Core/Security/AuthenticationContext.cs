using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Authentication context
    /// </summary>
    public sealed class AuthenticationContext
    {

        /// <summary>
        /// System identity
        /// </summary>
        private static readonly IPrincipal s_system = new GenericPrincipal(new GenericIdentity("SYSTEM"), new string[] { });
        
        /// <summary>
        /// Anonymous identity
        /// </summary>
        private static readonly IPrincipal s_anonymous = new GenericPrincipal(new GenericIdentity("ANONYMOUS"), new string[] { });

        /// <summary>
        /// Gets the anonymous principal
        /// </summary>
        public static IPrincipal AnonymousPrincipal
        {
            get
            {
                return s_anonymous;
            }
        }

        /// <summary>
        /// Get the system principal
        /// </summary>
        public static IPrincipal SystemPrincipal
        {
            get
            {
                return s_system;
            }
        }

        /// <summary>
        /// Current context in the request pipeline
        /// </summary>
        [ThreadStatic]
        private static AuthenticationContext s_current;

        /// <summary>
        /// Locking
        /// </summary>
        private static Object s_lockObject = new object();

        /// <summary>
        /// The principal
        /// </summary>
        private IPrincipal m_principal;

        /// <summary>
        /// Creates a new instance of the authentication context
        /// </summary>
        public AuthenticationContext(IPrincipal principal)
        {
            this.m_principal = principal;
        }

        /// <summary>
        /// Gets or sets the current context
        /// </summary>
        public static AuthenticationContext Current
        {
            get
            {
                if(s_current == null)
                    lock(s_lockObject)
                        s_current = new AuthenticationContext(s_anonymous);
                return s_current;
            }
            set { s_current = value; }
        }

        /// <summary>
        /// Gets the principal 
        /// </summary>
        public IPrincipal Principal
        {
            get
            {
                return this.m_principal;
            }
        }
    }
}
