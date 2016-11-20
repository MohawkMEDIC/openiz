using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Represents an exception related to data persistence
    /// </summary>
    public class DataPersistenceException : Exception
    {

        /// <summary>
        /// Creates a new data persistence exception
        /// </summary>
        public DataPersistenceException()
        {

        }

        /// <summary>
        /// Creates a new data persistence exception
        /// </summary>
        public DataPersistenceException(String message) : base(message)
        {

        }

        /// <summary>
        /// Creates a new data persistence exception
        /// </summary>
        public DataPersistenceException(String message, Exception e) : base(message, e)
        {

        }
    }
}
