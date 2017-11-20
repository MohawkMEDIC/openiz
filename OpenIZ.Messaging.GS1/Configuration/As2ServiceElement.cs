/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2017-7-4
 */
using OpenIZ.Core.Http;
using OpenIZ.Core.Http.Description;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Configuration
{

    /// <summary>
    /// JIRA Service configuration
    /// </summary>
    public class As2ServiceElement : ServiceClientDescription
    {

        /// <summary>
        /// AS2 service configuration
        /// </summary>
        public As2ServiceElement()
        {

        }

        /// <summary>
        /// Use AS2 standard mime based encoding
        /// </summary>
        [ConfigurationProperty("useAs2MimeEncoding")]
        public bool UseAS2MimeEncoding {
            get { return (bool)this["useAs2MimeEncoding"]; }
            set { this["useAs2MimeEncoding"] = value; }
        }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        [ConfigurationProperty("userName")]
        public String UserName {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        [ConfigurationProperty("password")]
        public String Password {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        /// <summary>
        /// Configuration property for trusted cert
        /// </summary>
        [ConfigurationProperty("trustedCert")]
        public String TrustedCertificate {
            get { return (string)this["trustedCert"]; }
            set { this["trustedCert"] = value; }
        }


    }
}
