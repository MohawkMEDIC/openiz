using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Represents a JIRA authentication response
    /// </summary>
    [JsonObject]
    public class JiraAuthenticationResponse
    {

        /// <summary>
        /// Gets or sets the session information
        /// </summary>
        [JsonProperty("session")]
        public JiraSessionInfo Session { get; set; }
    }

}
