using MARC.HI.EHRS.SVC.Configuration;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Persistence.Data.ADO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Persistence.Data.ADO.Configuration.UserInterface
{
    /// <summary>
    /// ADO.NET Persistence configuration feature
    /// </summary>
    public class AdoPersistenceConfigurableFeature : IScriptableConfigurableFeature
    {

        #region Console Parameters

        /// <summary>
        /// Gets the deployment options
        /// </summary>
        public class DeploymentOptions
        {

            /// <summary>
            /// Gets or sets the host name
            /// </summary>
            [Description("The host name of the database server")]
            public String HostName { get; set; }

            /// <summary>
            /// Gets or sets the user name
            /// </summary>
            [Description("User account to use to create the schema")]
            public String UserName { get; set; }

            /// <summary>
            /// Gets or sets the password
            /// </summary>
            [Description("Password to use to create the schema")]
            public String Password { get; set; }

            /// <summary>
            /// Gets or sets the catalog
            /// </summary>
            [Description("Catalog to use to create the schema")]
            public String Catalog { get; set; }

            /// <summary>
            /// Gets or sets whether to include updates
            /// </summary>
            [Description("True if updates should be included")]
            public bool IncludeUpdates { get; set; }

        }

        /// <summary>
        /// Gets the un-deployment options
        /// </summary>
        public class UnDeploymentOptions
        {

            /// <summary>
            /// Gets or sets whether to include updates
            /// </summary>
            [Description("True if the schema should be removed")]
            public bool DropData { get; set; }

        }
        #endregion

        // Panel 
        private ucAdoPersistence m_panel = new ucAdoPersistence();

        /// <summary>
        /// True if the feature should always be configured
        /// </summary>
        public bool AlwaysConfigure
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the deployment argument type
        /// </summary>
        public Type DeployArgumentType
        {
            get
            {
                return typeof(DeploymentOptions);
            }
        }

        /// <summary>
        /// True if this configuration is enabled
        /// </summary>
        public bool EnableConfiguration { get; set; }

        /// <summary>
        /// Gets the name of the configuration panel
        /// </summary>
        public string Name
        {
            get
            {
                return Locale.ConfigurationPanelName;
            }
        }

        /// <summary>
        /// Gets the panel which is the configuration for this object
        /// </summary>
        public System.Windows.Forms.Control Panel
        {
            get
            {
                return this.m_panel;
            }
        }

        /// <summary>
        /// Gets the un-deploy argument type
        /// </summary>
        public Type UnDeployArgumentType
        {
            get
            {
                return typeof(UnDeploymentOptions);
            }
        }

        /// <summary>
        /// Creates the configuration in the configuration section
        /// </summary>
        public void Configure(XmlDocument configurationDom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deploys the specified object on script basis
        /// </summary>
        public void Deploy(string[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform the "easy configuration" option
        /// </summary>
        public void EasyConfigure(XmlDocument configFile)
        {
            
        }

        /// <summary>
        /// Return whether this option is configured
        /// </summary>
        public bool IsConfigured(XmlDocument configurationDom)
        {
            // Read configuration here
            return false;
        }

        /// <summary>
        /// Removes configuration from the application
        /// </summary>
        public void UnConfigure(XmlDocument configurationDom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Undeploy the options from command prompt
        /// </summary>
        /// <param name="args"></param>
        public void Undeploy(string[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        public bool Validate(XmlDocument configurationDom)
        {
            return false;
        }
    }
}
