using OpenIZ.Core.Model.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Patch exception
    /// </summary>
    public class PatchException : Exception
    {

        /// <summary>
        /// The patch operation which failed
        /// </summary>
        public PatchOperation Operation { get; set; }

        /// <summary>
        /// Creates a new patch exception
        /// </summary>
        public PatchException()
        {

        }

        /// <summary>
        /// Creates a new patch exception with the specified <paramref name="message"/>
        /// </summary>
        public PatchException(string message) : base(message)
        {
        }

        /// <summary>
        /// Represents a patch exception
        /// </summary>
        public PatchException(string message, PatchOperation operation) : base(message)
        {
            this.Operation = operation;
        }


    }

    /// <summary>
    /// Represents an exceptional condition for the application of a patch with assertion
    /// </summary>
    public class PatchAssertionException : PatchException
    {

        /// <summary>
        /// Creates a new patch assertion exception
        /// </summary>
        public PatchAssertionException()
        {

        }

        /// <summary>
        /// Creates a new patch assertion
        /// </summary>
        public PatchAssertionException(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates a new patch operation
        /// </summary>
        public PatchAssertionException(Object expected, Object actual, PatchOperation op) : base($"Assertion failed: {expected} expected but {actual} found at {op}", op)
        {

        }
    }
}
