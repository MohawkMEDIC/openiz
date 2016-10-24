using Newtonsoft.Json;
using System;

namespace OpenIZ.Persistence.Diagnostics.Jira.Model
{
    /// <summary>
    /// Issue fields
    /// </summary>
    [JsonObject]
    public class JiraIssueFields
    {
        /// <summary>
        /// Assignee
        /// </summary>
        [JsonProperty("assignee")]
        public String Assignee { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public String Description { get; set; }

        /// <summary>
        /// Issue Type
        /// </summary>
        [JsonProperty("issuetype")]
        public JiraIdentifier IssueType { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        [JsonProperty("priority")]
        public JiraIdentifier Priority { get; set; }

        /// <summary>
        /// Project ID
        /// </summary>
        [JsonProperty("project")]
        public JiraKey Project { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        [JsonProperty("summary")]
        public String Summary { get; set; }
    }
}