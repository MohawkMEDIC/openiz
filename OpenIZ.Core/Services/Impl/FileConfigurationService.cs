using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Core.Services.Impl
{
    /// <summary>
    /// Provides a redirected configuration service which reads configuration from a different file
    /// </summary>
    public class FileConfigurationService : MARC.HI.EHRS.SVC.Core.Services.IConfigurationManager
    {

        // Configuration
        private System.Configuration.Configuration m_configuration = null;

        /// <summary>
        /// 
        /// </summary>
        public FileConfigurationService()
        {
            var configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "openiz.config");
            if (!File.Exists(configFile))
                configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "openiz.exe.config");

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = configFile }; //Path to your config file
            this.m_configuration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

        }

        /// <summary>
        /// Application settings
        /// </summary>
        public NameValueCollection AppSettings
        {
            get
            {
                var retVal = new NameValueCollection();
                foreach (var itm in this.m_configuration.AppSettings.Settings.AllKeys)
                    retVal.Add(itm, this.m_configuration.AppSettings.Settings[itm].Value);
                return retVal;
            }
        }


        /// <summary>
        /// Connection Strings
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return this.m_configuration.ConnectionStrings.ConnectionStrings;
            }
        }

        /// <summary>
        /// Get the specified section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public object GetSection(string sectionName)
        {
            var configSection = this.m_configuration.GetSection(sectionName);

            if (configSection == null) throw new ConfigurationErrorsException($"Section {sectionName} not found");

            if (configSection is DefaultSection)
            {
                // Get the constructor
                Type handlerType = Type.GetType(configSection.SectionInformation.Type);
                if (handlerType == null)
                    throw new ConfigurationErrorsException($"Configuration handler {configSection.SectionInformation.Type} not found");

                var handler = Activator.CreateInstance(handlerType) as IConfigurationSectionHandler;
                if (handler == null)
                    throw new ConfigurationErrorsException($"Configuration handler {configSection.SectionInformation.Type} does not implement IConfigurationSectionHandler");

                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(configSection.SectionInformation.GetRawXml());
                return handler.Create(configSection.ElementInformation, configSection.CurrentConfiguration, xdoc.DocumentElement);

                //return this.m_configuration.GetSection(sectionName);
            }
            else
                return configSection;
        }
    }
}
