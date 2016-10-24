using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Represents a JIRA issue response
    /// </summary>
    [JsonObject]
    public class JiraIssueResponse
    {

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [JsonProperty("key")]
        public String Key { get; set; }

        /// <summary>
        /// Self link
        /// </summary>
        [JsonProperty("self")]
        public String Self { get; set; }

    }
}
