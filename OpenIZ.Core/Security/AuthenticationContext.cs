using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Authentication context
    /// </summary>
    public class AuthenticationContext
    {
        /// <summary>
        /// Current context in the request pipeline
        /// </summary>
        [ThreadStatic]
        private static AuthenticationContext s_current;

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
            get { return s_current; }
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
