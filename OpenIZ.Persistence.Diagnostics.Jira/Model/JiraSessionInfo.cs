using Newtonsoft.Json;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{

    /// <summary>
    /// Represents JIRA session information
    /// </summary>
    [JsonObject]
    public class JiraSessionInfo
    {

        /// <summary>
        /// Gets or sets the name of the session
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the session
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

    }
}