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
            foreach (XmlElement itm in typeConfigs)
            {
                retVal.Types.Add(new TypeCacheConfigurationInfo()
                {
                    TypeXml = itm.Attributes["type"]?.Value,
                    MaxCacheAge = TimeSpan.Parse(itm.Attributes["maxAge"]?.Value ?? "1:0:0:0", CultureInfo.InvariantCulture).Ticks,
                    MaxCacheSize = Int32.Parse(itm.Attributes["maxSize"]?.Value ?? "50"),
                    SeedQueries = itm.SelectNodes("./*[local-name() = 'seed']/*[local-name() = 'add']").OfType<XmlElement>().Select(o => o.Value).ToList()
                });
            }

            return retVal;

        }
    }
}
