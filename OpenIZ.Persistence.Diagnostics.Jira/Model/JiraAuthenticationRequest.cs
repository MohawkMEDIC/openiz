using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// JIRA Authentication request
    /// </summary>
    [JsonObject]
    public class JiraAuthenticationRequest 
    {
        /// <summary>
        /// Authentication request
        /// </summary>
        public JiraAuthenticationRequest()
        {

        }

        /// <summary>
        /// Authentication request
        /// </summary>
        public JiraAuthenticationRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        [JsonProperty("username")]
        public String UserName { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        [JsonProperty("password")]
        public String Password { get; set; }
    }
}
