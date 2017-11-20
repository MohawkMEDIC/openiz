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
    public class AdoCodeDataFeature : IDataFeature
    {
        /// <summary>
        /// Gets or sets the name of the feature
        /// </summary>
        public string Name
        {
            get
            {
                return "OpenIZ Codes Deployment";
            }
        }

        /// <summary>
        /// Get the check sql
        /// </summary>
        public string GetCheckSql(string invariantName)
        {
            return "SELECT COUNT(*) > 0 FROM cd_tbl WHERE cd_id = 'a87a6d21-2ca6-4aea-88f3-6135cceb58d1';";
        }

        /// <summary>
        /// Get deployment sql
        /// </summary>
        public string GetDeploySql(string invariantName)
        {
            String[] resource = new String[]
            {
                "openiz-codes.sql",
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
            return null;
        }
    }
}
