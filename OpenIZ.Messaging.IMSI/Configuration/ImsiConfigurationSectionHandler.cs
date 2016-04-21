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
 * User: fyfej
 * Date: 2016-1-22
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Messaging.IMSI.Configuration
{
    /// <summary>
    /// Configuration handler for the IMSI section
    /// </summary>
    public class ImsiConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            
            // Section
            XmlElement serviceElement = section.SelectSingleNode("./*[local-name() = 'service']") as XmlElement;

            string wcfServiceName = serviceElement?.Attributes["wcfServiceName"]?.Value;

            if(wcfServiceName == null)
                throw new ConfigurationErrorsException("Missing serviceElement", section);

            return new ImsiConfiguration(wcfServiceName);
        }
    }
}
