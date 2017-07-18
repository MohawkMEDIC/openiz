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
 * Date: 2016-6-14
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
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
                    retVal.ReadWriteConnectionString = ApplicationContext.Current.GetService<IConfigurationManager>().ConnectionStrings[connectionNode.Attributes["readWriteConnection"].Value]?.ConnectionString;
                if (connectionNode.Attributes["readonlyConnection"] != null)
                    retVal.ReadonlyConnectionString = ApplicationContext.Current.GetService<IConfigurationManager>().ConnectionStrings[connectionNode.Attributes["readonlyConnection"].Value]?.ConnectionString;
                else
                    retVal.ReadonlyConnectionString = retVal.ReadWriteConnectionString;
                if (connectionNode.Attributes["maxCacheSize"] != null)
                    retVal.MaxCacheSize = Int32.Parse(connectionNode.Attributes["maxCacheSize"].Value);
                else
                    retVal.MaxCacheSize = ushort.MaxValue;
                if (connectionNode.Attributes["insertUpdate"] != null)
                    retVal.AutoUpdateExisting = bool.Parse(connectionNode.Attributes["insertUpdate"].Value);
                if (connectionNode.Attributes["traceSql"] != null)
                    retVal.TraceSql = Boolean.Parse(connectionNode.Attributes["traceSql"].Value);
            }

            if (retVal.ReadWriteConnectionString == null || retVal.ReadonlyConnectionString == null)
                throw new ConfigurationErrorsException("Connection string not found");



            return retVal;
                
        }
    }
}
