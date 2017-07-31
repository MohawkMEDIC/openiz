using MARC.HI.EHRS.SVC.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OpenIZ.Core.Configuration.UI
{
    /// <summary>
    /// Core Configuration Panel for OpenIZ
    /// </summary>
    public class CoreConfigurationPanel : IConfigurableFeature
    {

        private ucCoreSettings m_panel = new ucCoreSettings();

        /// <summary>
        /// Always configure this feature
        /// </summary>
        public bool AlwaysConfigure
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Enable the configuration
        /// </summary>
        public bool EnableConfiguration
        {
            get;set;
        }

        /// <summary>
        /// Get or set the name
        /// </summary>
        public string Name
        {
            get
            {
                return "OpenIZ/Options";
            }
        }

        /// <summary>
        /// Gets the panel
        /// </summary>
        public Control Panel
        {
            get
            {
                return this.m_panel;
            }
        }

        /// <summary>
        /// Configure the features
        /// </summary>
        public void Configure(XmlDocument configurationDom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Easy configure the features
        /// </summary>
        /// <param name="configFile"></param>
        public void EasyConfigure(XmlDocument configFile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// True if the service is configured
        /// </summary>
        public bool IsConfigured(XmlDocument configurationDom)
        {
            return true;
        }

        /// <summary>
        /// Un-configure the option
        /// </summary>
        public void UnConfigure(XmlDocument configurationDom)
        {
        }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        public bool Validate(XmlDocument configurationDom)
        {
            return true;
        }
    }
}
