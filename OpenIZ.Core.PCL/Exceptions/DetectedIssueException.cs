using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Represents an exception which contains a series of detected issue events
    /// </summary>
    public class DetectedIssueException : Exception
    {

        /// <summary>
        /// Gets the list of issues set by the BRE 
        /// </summary>
        public List<DetectedIssue> Issues { get; private set; }

        /// <summary>
        /// Creates a new detected issue exception
        /// </summary>
        public DetectedIssueException()
        {

        }

        /// <summary>
        /// Creates a new detected issue exception with the specified <paramref name="issues"/> and <paramref name="message"/>
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues, String message) : this(issues, message, null)
        {

        }

        /// <summary>
        /// Creates a new detected issue exception with the specified <paramref name="issues"/> <paramref name="message"/> and causal exception (<paramref name="innerException"/>)
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues, String message, Exception innerException) : base(message, innerException)
        {

        }

        /// <summary>
        /// Creates a new detected issue exception with the specified issue list
        /// </summary>
        public DetectedIssueException(List<DetectedIssue> issues) : this(issues, null, null)
        {
        }


    }
}
