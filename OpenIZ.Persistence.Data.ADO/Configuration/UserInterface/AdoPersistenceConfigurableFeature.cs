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
using MARC.HI.EHRS.SVC.Configuration.Data;
using OpenIZ.Persistence.Data.ADO.Data.SQL;
using MARC.HI.EHRS.SVC.Configuration.UI;
using OpenIZ.OrmLite.Providers;
using OpenIZ.Persistence.Data.ADO.Services;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Persistence;

namespace OpenIZ.Persistence.Data.ADO.Configuration.UserInterface
{
    /// <summary>
    /// ADO.NET Persistence configuration feature
    /// </summary>
    public class AdoPersistenceConfigurableFeature : IScriptableConfigurableFeature, IDataboundFeature, IReportProgressChanged
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

        // Configuration
        private AdoConfiguration m_configuration;

        // Progress changed
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

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
        /// Gets or sets the connection string for this object
        /// </summary>
        public DbConnectionString ConnectionString
        {
            get
            {
                return this.m_panel.ConnectionString;
            }
            set
            {
                this.m_panel.ConnectionString = value;
            }
        }
        
        public List<IDataFeature> DataFeatures
        {
            get
            {
                return new List<IDataFeature>()
                {
                    new AdoCoreDataFeature(),
                    new AdoCodeDataFeature(),
                    new AdoDataInitialization()
                };
            }
        }

        /// <summary>
        /// Updates
        /// </summary>
        public List<IDataUpdate> Updates
        {
            get
            {
                List<IDataUpdate> retVal = new List<IDataUpdate>();
                foreach(var nam in typeof(AdoPersistenceConfigurableFeature).Assembly.GetManifestResourceNames())
                    if(nam.StartsWith("OpenIZ.Persistence.Data.ADO.Data.SQL.Updates"))
                    {
                        using (var str = typeof(AdoPersistenceConfigurableFeature).Assembly.GetManifestResourceStream(nam))
                            retVal.Add(SqlSourceUpdate.Load(str));
                    }
                return retVal;
            }
        }

        /// <summary>
        /// Creates the configuration in the configuration section
        /// </summary>
        public void Configure(XmlDocument configurationDom)
        {
            // TODO: Check for updates and then install them

            // TODO: Check for dataset files and then install them

            // Configure XML
            var xmlSection = configurationDom.GetOrAddSectionDefinition(AdoDataConstants.ConfigurationSectionName, typeof(AdoConfigurationSectionHandler));
            var configurationElement = xmlSection.SelectSingleNode("./connectionManager");
            if (configurationElement == null)
            {
                configurationElement = configurationDom.CreateElement("connectionManager");
                xmlSection.AppendChild(configurationElement);
            }

            // ro connection
            if (configurationElement.Attributes["readonlyConnection"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("readonlyConnection", this.m_panel.ConfigurationObject.ReadonlyConnectionString ?? this.ConnectionString.Name));
            else
                configurationElement.Attributes["readonlyConnection"].Value = this.m_panel.ConfigurationObject.ReadonlyConnectionString ?? this.ConnectionString.Name;

            // rw connection
            if (configurationElement.Attributes["readWriteConnection"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("readWriteConnection", m_panel.ConfigurationObject.ReadWriteConnectionString ?? this.ConnectionString.Name));
            else
                configurationElement.Attributes["readWriteConnection"].Value = this.m_panel.ConfigurationObject.ReadWriteConnectionString ?? this.ConnectionString.Name;

            // trace
            if (configurationElement.Attributes["traceSql"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("traceSql", this.m_panel.ConfigurationObject.TraceSql.ToString()));
            else
                configurationElement.Attributes["traceSql"].Value = this.m_panel.ConfigurationObject.TraceSql.ToString();

            // autoinsert kids
            if (configurationElement.Attributes["autoInsertChildren"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("autoInsertChildren", this.m_panel.ConfigurationObject.AutoInsertChildren.ToString()));
            else
                configurationElement.Attributes["autoInsertChildren"].Value = this.m_panel.ConfigurationObject.AutoInsertChildren.ToString();

            // auto-update
            if (configurationElement.Attributes["insertUpdate"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("insertUpdate", this.m_panel.ConfigurationObject.AutoUpdateExisting.ToString()));
            else
                configurationElement.Attributes["insertUpdate"].Value = this.m_panel.ConfigurationObject.AutoUpdateExisting.ToString();

            // Find the provider 
            var providerType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.ExportedTypes).FirstOrDefault(o => typeof(IDbProvider).IsAssignableFrom(o) && !o.IsInterface && !o.IsAbstract && (Activator.CreateInstance(o) as IDbProvider).Name.ToLower() == this.ConnectionString.Provider.InvariantName.ToLower());
            if (configurationElement.Attributes["provider"] == null)
                configurationElement.Attributes.Append(configurationDom.CreateAttributeValue("provider", providerType.AssemblyQualifiedName));
            else
                configurationElement.Attributes["provider"].Value = providerType.AssemblyQualifiedName;

            configurationDom.RegisterService(typeof(AdoPersistenceService));

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
            this.m_panel.ConfigurationObject = new AdoConfiguration()
            {
                AutoInsertChildren = false,
                AutoUpdateExisting = false,
                TraceSql = false,
            };
            this.m_panel.ConnectionString = this.ConnectionString;
            this.Configure(configFile);
            this.EnableConfiguration = true;
        }

        /// <summary>
        /// Return whether this option is configured
        /// </summary>
        public bool IsConfigured(XmlDocument configurationDom)
        {
            // Read configuration here
            var adoConfiguration = configurationDom.GetSection("openiz.persistence.data.ado") as AdoConfiguration;
            this.m_panel.ConfigurationObject = adoConfiguration ?? new AdoConfiguration()
            {
                AutoInsertChildren = false,
                AutoUpdateExisting = false,
                TraceSql = false,
                ReadWriteConnectionString = this.ConnectionString.Name,
                ReadonlyConnectionString = this.ConnectionString.Name
            };

            if (adoConfiguration != null)
            {
                var connectionString = configurationDom.GetConnectionString(adoConfiguration.ReadWriteConnectionString);
                this.m_panel.ConnectionString = connectionString;
            }

            return adoConfiguration != null;
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

        /// <summary>
        /// Represent as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// After deployment
        /// </summary>
        public void AfterDeploy()
        {

        }

        /// <summary>
        /// Fired after applying updates
        /// </summary>
        public void AfterUpdate()
        {

            // Update the configuration
            ApplicationContext.Current.GetService<FileConfigurationService>().Open(ConfigurationApplicationContext.s_configFile);

            if (ApplicationContext.Current.GetService<IDataPersistenceService<Entity>>() == null)
            {
                ApplicationContext.Current.AddServiceProvider(typeof(AdoPersistenceService));
                ApplicationContext.Current.GetService<AdoPersistenceService>().Start();
            }

            // Check the directory for datasets to install
            var dsi = new DataInitializationService();
            dsi.ProgressChanged += (o, e) => this.ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)(e.Progress * 100), e.State));
            dsi.InstallDataDirectory((o,e) => this.ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)(e.Progress * 100), null)));
        }

        public void AfterUnDeploy()
        {
        }
    }
}
