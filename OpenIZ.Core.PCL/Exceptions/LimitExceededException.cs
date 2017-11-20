using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// An exception which indicates that some limit has been exceeeded
    /// </summary>
    public class LimitExceededException : Exception
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public LimitExceededException()
        {

        }

        /// <summary>
        /// Create new limit exceeded exception with specified message
        /// </summary>
        public LimitExceededException(String message) : base(message)
        {

        }

        /// <summary>
        /// Create new limit exceeded exception with specified message and cause
        /// </summary>
        public LimitExceededException(String message, Exception inner) : base(message, inner)
        {

        }
    }
}
