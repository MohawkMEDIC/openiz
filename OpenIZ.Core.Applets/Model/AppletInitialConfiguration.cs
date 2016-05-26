using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.Model
{
    /// <summary>
    /// Represents a configuration of an applet
    /// </summary>
    [XmlType(nameof(AppletInitialConfiguration), Namespace = "http://openiz.org/applet")]
    public class AppletInitialConfiguration
    {

        /// <summary>
        /// Gets or sets the applet id
        /// </summary>
        [XmlAttribute("applet")]
        public String AppletId
        {
            get;
            set;
        }

        /// <summary>
        /// Applet configuration entry
        /// </summary>
        [XmlElement("appSetting")]
        public List<AppletConfigurationEntry> AppSettings
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Applet configuration entry
    /// </summary>
    [XmlType(nameof(AppletConfigurationEntry), Namespace = "http://openiz.org/applet")]
    public class AppletConfigurationEntry
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlAttribute("value")]
        public String Value
        {
            get;
            set;
        }
    }

}
