using Newtonsoft.Json;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Represents a jira identifier
    /// </summary>
    [JsonObject]
    public class JiraIdentifier
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public JiraIdentifier()
        {

        }

        /// <summary>
        /// Creates a new identifier with specified id
        /// </summary>
        public JiraIdentifier(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Creates a new identifier with specified name
        /// </summary>
        public JiraIdentifier(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Represents numeric identifier
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Represents name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}