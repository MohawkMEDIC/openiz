using Newtonsoft.Json;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Represents a jira key
    /// </summary>
    [JsonObject]
    public class JiraKey
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public JiraKey()
        {

        }

        /// <summary>
        /// Creates a new key with specified key
        /// </summary>
        public JiraKey(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}