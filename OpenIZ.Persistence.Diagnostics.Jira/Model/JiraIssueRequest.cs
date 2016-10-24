using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Represents a JIRA issue request
    /// </summary>
    [JsonObject]
    public class JiraIssueRequest
    {

        /// <summary>
        /// Fields to be set
        /// </summary>
        [JsonProperty("fields")]
        public JiraIssueFields Fields { get; set; }

        

    }
}
