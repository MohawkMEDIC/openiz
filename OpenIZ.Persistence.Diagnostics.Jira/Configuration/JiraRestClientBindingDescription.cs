using System;
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
    /// <summary>
    /// Represents a JIRA REST client binding description
    /// </summary>
    internal class JiraRestClientBindingDescription : IRestClientBindingDescription
    {
        /// <summary>
        /// Gets the content type mapper
        /// </summary>
        public IContentTypeMapper ContentTypeMapper
        {
            get
            {
                return new DefaultContentTypeMapper();
            }
        }

        /// <summary>
        /// Whether optimizations should be performed
        /// </summary>
        public bool Optimize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the security description
        /// </summary>
        public IRestClientSecurityDescription Security
        {
            get
            {
                return new JiraSecurityDescription();
            }
        }
    }
}