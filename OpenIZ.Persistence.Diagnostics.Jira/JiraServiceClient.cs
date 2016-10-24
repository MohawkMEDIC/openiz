using OpenIZ.Core.Interop.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Http;
using OpenIZ.Persistence.Diagnostics.Jira.Model;
using OpenIZ.Persistence.Diagnostics.Jira.Configuration;

namespace OpenIZ.Persistence.Diagnostics.Jira
{
    /// <summary>
    /// Represents a service client base
    /// </summary>
    public class JiraServiceClient : ServiceClientBase
    {

        /// <summary>
        /// Creates the specified service client
        /// </summary>
        public JiraServiceClient(IRestClient restClient) : base(restClient)
        {
            this.Client.Accept = this.Client.Accept ?? "application/json";
        }

        /// <summary>
        /// Represents a JIRA authentication request
        /// </summary>
        public JiraAuthenticationResponse Authenticate(JiraAuthenticationRequest jiraAuthenticationRequest)
        {
            var result = this.Client.Post<JiraAuthenticationRequest, JiraAuthenticationResponse>("auth/1/session", "application/json", jiraAuthenticationRequest);
            this.Client.Credentials = new JiraCredentials(result);
            return result;
        }

        /// <summary>
        /// Create an issue
        /// </summary>
        public JiraIssueResponse CreateIssue(JiraIssueRequest issue)
        {
            return this.Client.Post<JiraIssueRequest, JiraIssueResponse>("api/2/issue", "application/json", issue);
        }

        /// <summary>
        /// Create an attachment
        /// </summary>
        public void CreateAttachment(JiraIssueResponse issue, MultipartAttachment attachment)
        {
            String boundary = String.Format("------{0:N}", Guid.NewGuid());
            this.Client.Post<MultipartAttachment, Object>(String.Format("api/2/issue/{0}/attachments", issue.Key), String.Format("multipart/form-data; boundary={0}", boundary), attachment); 
        }

        /// <summary>
        /// Create an attachment
        /// </summary>
        public void CreateAttachment(JiraIssueResponse issue, List<MultipartAttachment> attachment)
        {
            String boundary = String.Format("------{0:N}", Guid.NewGuid());
            this.Client.Post<List<MultipartAttachment>, Object>(String.Format("api/2/issue/{0}/attachments", issue.Key), String.Format("multipart/form-data; boundary={0}", boundary), attachment);
        }
    }
}
