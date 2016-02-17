using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security.Wcf
{
    /// <summary>
    /// Unauthorized access
    /// </summary>
    public class UnauthorizedRequestException : AuthenticationException
    {

        /// <summary>
        /// Authenticate challenge
        /// </summary>
        public String AuthenticateChallenge { get; set; }

        /// <summary>
        /// Unauthorized access exception
        /// </summary>
        public UnauthorizedRequestException(String message, String scheme, String realm, String scope) : base(message)
        {
            StringBuilder authenticateString = new StringBuilder();
            authenticateString.AppendFormat("{0} realm=\"{1}\"", scheme, realm ?? "anonymous");
            if(!String.IsNullOrEmpty(scope))
                authenticateString.AppendFormat(", scope=\"{0}\"", scope);
            this.AuthenticateChallenge = authenticateString.ToString();
        }

    }
}
