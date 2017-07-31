using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Configuration;
using MARC.HI.EHRS.SVC.Configuration;
using System.Xml;

namespace ConfigTool
{
    public class OpenIZAboutPanel : IConfigurableFeature
    {
        #region IConfigurationPanel Members

        private ucAboutOpenIZ m_panel = new ucAboutOpenIZ();
        /// <summary>
        /// Configure the item
        /// </summary>
        public void Configure(System.Xml.XmlDocument configurationDom)
        {
        }

        /// <summary>
        /// Enable configuration
        /// </summary>
        public bool EnableConfiguration
        {
            get;set;
        }

        /// <summary>
        /// Is configured
        /// </summary>
        public bool IsConfigured(System.Xml.XmlDocument configurationDom)
        {
            return true;
        }

        /// <summary>
        /// Get the name
        /// </summary>
        public string Name
        {
            get { return "OpenIZ"; }
        }

        /// <summary>
        /// Get the panel
        /// </summary>
        public System.Windows.Forms.Control Panel
        {
            get { return m_panel; }
        }

        public bool AlwaysConfigure
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Unconfigure
        /// </summary>
        public void UnConfigure(System.Xml.XmlDocument configurationDom)
        {
        }

        /// <summary>
        /// Validate
        /// </summary>
        public bool Validate(System.Xml.XmlDocument configurationDom)
        {
            return true;
        }

        public void EasyConfigure(XmlDocument configFile)
        {
        }

        #endregion
    }
}
