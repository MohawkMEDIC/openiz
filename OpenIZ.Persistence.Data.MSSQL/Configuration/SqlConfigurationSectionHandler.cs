using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Persistence.Data.MSSQL.Configuration
{
    /// <summary>
    /// Configuration section handler for the SQL Server handler
    /// </summary>
    public class SqlConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, XmlNode section)
        {
            SqlConfiguration retVal = new SqlConfiguration();

            XmlElement connectionNode = section.SelectSingleNode("./connectionManager") as XmlElement;
            if (connectionNode == null)
                throw new ConfigurationErrorsException("No connection manager specified", section);
            else
            {
                if (connectionNode.Attributes["readWriteConnection"] == null)
                    throw new ConfigurationErrorsException("Connection manager must have a read/write connection", connectionNode);
                else
                    retVal.ReadWriteConnectionString = ConfigurationManager.ConnectionStrings[connectionNode.Attributes["readWriteConnection"].Value].ConnectionString;
                if (connectionNode.Attributes["readonlyConnection"] != null)
                    retVal.ReadonlyConnectionString = ConfigurationManager.ConnectionStrings[connectionNode.Attributes["readonlyConnection"].Value].ConnectionString;
                else
                    retVal.ReadonlyConnectionString = retVal.ReadWriteConnectionString;
            }
            return retVal;
                
        }
    }
}
