using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Configuration
{
    /// <summary>
    /// Configuration section handler
    /// </summary>
    public class SqlConfiguration
    {

        /// <summary>
        /// Read/write connection string
        /// </summary>
        public String ReadWriteConnectionString { get; set; }

        /// <summary>
        /// Readonly connection string
        /// </summary>
        public String ReadonlyConnectionString { get; set; }
    }
}
