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
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Configuration
{
    /// <summary>
    /// GS1 configuration 
    /// </summary>
    public class Gs1ConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Auto create materials
        /// </summary>
        [ConfigurationProperty("autoCreateMaterials")]
        public bool AutoCreateMaterials {
            get { return (bool)this["autoCreateMaterials"]; }
            set { this["autoCreateMaterials"] = value; }
        }

        /// <summary>
        /// Default content owner assigning authority
        /// </summary>
        [ConfigurationProperty("defaultAuthority")]
        public String DefaultContentOwnerAssigningAuthority {
            get { return (string)this["defaultAuthority"]; }
            set { this["defaultAuthority"] = value; }
        }

        /// <summary>
        /// Gets the queue on which to place messages
        /// </summary>
        [ConfigurationProperty("queueName")]
        public String Gs1QueueName {
            get { return (string)this["queueName"]; }
            set { this["queueName"] = value; }
        }

        /// <summary>
        /// Gets or sets the gs1 broker address
        /// </summary>
        [ConfigurationProperty("broker")]
        public As2ServiceElement Gs1BrokerAddress {
            get { return (As2ServiceElement)this["broker"]; }
            set { this["broker"] = value; }
        }

    }
}
