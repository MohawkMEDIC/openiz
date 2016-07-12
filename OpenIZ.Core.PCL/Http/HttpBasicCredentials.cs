using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a credential provider which does basic http
    /// </summary>
    public class HttpBasicCredentials : Credentials
    {
        // Password
        private String m_password;
        private string m_userName;

        /// <summary>
        /// Creates the basic credential 
        /// </summary>
        public HttpBasicCredentials(IPrincipal principal, string password) : base(principal)
        {
            if (!principal.Identity.IsAuthenticated)
                throw new InvalidOperationException("Principal must be authenticated");
            this.m_userName = principal.Identity.Name;
            this.m_password = password;

        }

        /// <summary>
        /// Gets the HTTP headers
        /// </summary>
        public override Dictionary<string, string> GetHttpHeaders()
        {
            var authString = String.Format("{0}:{1}", this.m_userName, this.m_password);
            return new Dictionary<string, string>()
            {
                {  "Authorization", String.Format("BASIC {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(authString))) }
            };
        }
    }
}
