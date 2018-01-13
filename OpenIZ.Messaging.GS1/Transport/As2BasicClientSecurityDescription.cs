/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
using System;
using System.Security.Principal;
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;
using OpenIZ.Messaging.GS1.Configuration;

namespace OpenIZ.Messaging.GS1.Transport.AS2
{
    /// <summary>
    /// Client security description
    /// </summary>
    internal class As2BasicClientSecurityDescription : IRestClientSecurityDescription
    {
        private As2ServiceElement m_configurationData;

        /// <summary>
        /// Creates a new instance of the client security description
        /// </summary>
        /// <param name="configurationData"></param>
        public As2BasicClientSecurityDescription(As2ServiceElement configurationData)
        {
            this.m_configurationData = configurationData;
        }

        /// <summary>
        /// Gets or sets the authentication realm
        /// </summary>
        public string AuthRealm
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the certificate validator
        /// </summary>
        public ICertificateValidator CertificateValidator
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get client certificates
        /// </summary>
        public IRestClientCertificateDescription ClientCertificate
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get the credential provider
        /// </summary>
        public ICredentialProvider CredentialProvider
        {
            get
            {
                return new As2BasicCredentialProvider(this.m_configurationData);
            }
        }

        /// <summary>
        /// Gets the mode of security
        /// </summary>
        public SecurityScheme Mode
        {
            get
            {
                return SecurityScheme.Basic;
            }
        }

        /// <summary>
        /// Preemptive
        /// </summary>
        public bool PreemptiveAuthentication
        {
            get
            {
                return true;
            }

            set
            {
                
            }
        }

        /// <summary>
        /// Credential provider
        /// </summary>
        private class As2BasicCredentialProvider : ICredentialProvider
        {
            private As2ServiceElement m_configurationData;

            /// <summary>
            /// Creates a basic credential provider
            /// </summary>
            public As2BasicCredentialProvider(As2ServiceElement m_configurationData)
            {
                this.m_configurationData = m_configurationData;
            }

            /// <summary>
            /// Authenticate on the specified context
            /// </summary>
            public Credentials Authenticate(IRestClient context)
            {
                return new HttpBasicCredentials(this.m_configurationData.UserName, this.m_configurationData.Password);
            }

            /// <summary>
            /// Get credentials from principal
            /// </summary>
            /// <param name="principal"></param>
            /// <returns></returns>
            public Credentials GetCredentials(IPrincipal principal)
            {
                return new HttpBasicCredentials(this.m_configurationData.UserName, this.m_configurationData.Password);
            }

            /// <summary>
            /// Get the credentials
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public Credentials GetCredentials(IRestClient context)
            {
                return new HttpBasicCredentials(this.m_configurationData.UserName, this.m_configurationData.Password);
            }
        }
    }
}