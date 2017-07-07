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
 * Date: 2017-4-29
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Caching.Redis.Configuration
{
    /// <summary>
    /// REDIS Configuration Section
    /// </summary>
    public class RedisConfigurationHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            // REDIS configuration
            return new RedisConfiguration()
            {
                Servers = section.SelectNodes("./server/add").OfType<XmlElement>().Select(o=>String.Format("{0}:{1}", o.Attributes["host"]?.Value ?? "localhost", o.Attributes["port"]?.Value ?? "6379")).ToList(),
                UserName = section.SelectSingleNode("./authentication/@userName")?.Value,
                Password = section.SelectSingleNode("./authentication/@password")?.Value
            };
        }
    }
}
