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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Configuration.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.ADO.Data.SQL
{
    /// <summary>
    /// Represents a data definition feature
    /// </summary>
    public class AdoCoreDataFeature : IDataFeature
    {
        /// <summary>
        /// Gets or sets the name of the feature
        /// </summary>
        public string Name
        {
            get
            {
                return "OpenIZ Core Deployment";
            }
        }

        /// <summary>
        /// Get the check sql
        /// </summary>
        public string GetCheckSql(string invariantName)
        {
            return "SELECT false;";
        }

        /// <summary>
        /// Get deployment sql
        /// </summary>
        public string GetDeploySql(string invariantName)
        {
            String[] resource = new String[]
            {
                "openiz-ddl.sql",
                "openiz-fn.sql"
            };

            // Build sql
            switch (invariantName.ToLower())
            {
                case "npgsql":
                    StringBuilder sql = new StringBuilder();

                    foreach (var itm in resource)
                        using (var streamReader = new StreamReader(typeof(AdoCoreDataFeature).Assembly.GetManifestResourceStream($"OpenIZ.Persistence.Data.ADO.Data.SQL.PSQL.{itm}")))
                            sql.Append(streamReader.ReadToEnd());
                    return sql.ToString();
                default:
                    throw new InvalidOperationException($"Deployment for {invariantName} not supported");
            }
        }

        /// <summary>
        /// Get un-deploy sql
        /// </summary>
        public string GetUnDeploySql(string invariantName)
        {
            // Build sql
            switch (invariantName.ToLower())
            {
                case "npgsql":
                    using (var streamReader = new StreamReader(typeof(AdoCoreDataFeature).Assembly.GetManifestResourceStream($"OpenIZ.Persistence.Data.ADO.Data.SQL.PSQL.openiz-drop.sql")))
                        return streamReader.ReadToEnd();
                default:
                    throw new InvalidOperationException($"Deployment for {invariantName} not supported");
            }
        }
    }
}
