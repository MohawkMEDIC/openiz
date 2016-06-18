/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
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
 * User: Nityan
 * Date: 2016-6-17
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Messaging.AMI.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class AmiConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration object
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {

            XmlElement caConfigurationElement = section.SelectSingleNode("./*[local-name() = 'ca']") as XmlElement;
            CertificationAuthorityConfiguration caConfiguration = new CertificationAuthorityConfiguration();

            if(caConfigurationElement != null)
            {
                caConfiguration.AutoApprove = caConfigurationElement?.Attributes["autoApprove"]?.Value == "true";
                caConfiguration.Name = caConfigurationElement?.Attributes["cn"]?.Value;
                caConfiguration.ServerName = caConfigurationElement?.Attributes["serverName"]?.Value;
            }

            // Configuration
            return new AmiConfiguration()
            {
                CaConfiguration = caConfiguration
            };
        }
    }
}
