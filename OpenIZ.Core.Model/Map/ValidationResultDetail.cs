using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Map
{

    /// <summary>
    /// Result detail types
    /// </summary>
    public enum ResultDetailType
    {
        Error,
        Warning,
        Information
    }

    /// <summary>
    /// Represents a result detail which is a validation result
    /// </summary>
    public class ValidationResultDetail
    {

        public ValidationResultDetail(ResultDetailType level, string message, Exception causedBy, String location)
        {
            this.Level = level;
            this.Message = message;
            this.CausedBy = causedBy;
            this.Location = location;
        }

        /// <summary>
        /// Gets or sets the message which caused the error
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the exception that caused this error
        /// </summary>
        public Exception CausedBy { get; set; }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public String Location { get; set; }

        /// <summary>
        /// The level of the warning
        /// </summary>
        public ResultDetailType Level { get; set; }

    }
}
