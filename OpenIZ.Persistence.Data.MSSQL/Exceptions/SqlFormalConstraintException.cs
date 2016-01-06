using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Exceptions
{
    /// <summary>
    /// Represents a violation of a formal constraint
    /// </summary>
    public class SqlFormalConstraintException : ConstraintException
    {
        /// <summary>
        /// Formal constraint message
        /// </summary>
        public SqlFormalConstraintException(string message) : base(message)
        {

        }
    }
}
