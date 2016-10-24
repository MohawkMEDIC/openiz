using System;
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;

namespace OpenIZ.Persistence.Diagnostics.Jira.Configuration
{
    /// <summary>
    /// Represents a security description for JIRA
    /// </summary>
    internal class JiraSecurityDescription : IRestClientSecurityDescription
    {

        /// <summary>
        /// Gets or sets the authentication realm
        /// </summary>
        public string AuthRealm
        {
            get
            {
                return "*";
            }
        }

        /// <summary>
        /// Gets the certificate validator
        /// </summary>
        public ICertificateValidator CertificateValidator
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the certificate description
        /// </summary>
        public IRestClientCertificateDescription ClientCertificate
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Credential provider
        /// </summary>
        public ICredentialProvider CredentialProvider
        {
            get
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gets the security scheme
        /// </summary>
        public SecurityScheme Mode
        {
            get
            {
                return SecurityScheme.Bearer;
            }
        }

        /// <summary>
        /// Preemtive authentication
        /// </summary>
        public bool PreemptiveAuthentication {  get { return true; } set { } }
    }
}