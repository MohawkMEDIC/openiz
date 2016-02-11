using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Authentication.AccessControlService.Model
{
    /// <summary>
    /// A token request
    /// </summary>
    public class OAuthTokenRequest
    {
        ///<summary>Grant type</summary> 
        public String Grant_Type { get; set; }
        ///<summary>Scope of the grant</summary>
        public String Scope { get; set; }
        ///<summary>User name</summary>
        public String UserName { get; set; }
        ///<summary>Password</summary>
        public String Password { get; set; }
        ///<summary>Auth code from authorization service</summary>
        public String Code { get; set; }
        ///<summary>Red</summary>
        public String Redirect_Uri { get; set; }
        /// <summary>
        /// Refreshing token
        /// </summary>
        public String Refresh_Token { get; set; }
        /// <summary>
        /// Assertion
        /// </summary>
        public String Assertion { get; set; }
    }
}
