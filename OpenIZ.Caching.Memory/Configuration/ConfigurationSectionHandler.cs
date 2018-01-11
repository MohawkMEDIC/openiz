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
 * User: justi
 * Date: 2016-8-2
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Caching.Memory.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var typeConfigs = section.SelectNodes("./*[local-name() = 'cacheTarget']/*[local-name() = 'add']");
            var autoCreateNode = section.SelectSingleNode("./@autoRegister");


            // Iterate over registration types
            MemoryCacheConfiguration retVal = new MemoryCacheConfiguration();
            retVal.AutoSubscribeTypes = XmlConvert.ToBoolean(autoCreateNode?.Value);
            retVal.MaxCacheAge = TimeSpan.Parse(section.Attributes["maxAge"]?.Value ?? "1:0:0:0", CultureInfo.InvariantCulture).Ticks;
            retVal.MaxCacheSize = Int32.Parse(section.Attributes["maxSize"]?.Value ?? "5000");

            foreach (XmlElement itm in typeConfigs)
            {
                retVal.Types.Add(new TypeCacheConfigurationInfo()
                {
                    TypeXml = itm.Attributes["type"]?.Value,
                    SeedQueries = itm.SelectNodes("./*[local-name() = 'seed']/*[local-name() = 'add']").OfType<XmlElement>().Select(o => o.InnerText).ToList()
                });
            }

            return retVal;

        }
    }
}
