using MARC.Everest.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Model validation exception
    /// </summary>
    public class ModelValidationException : Exception
    {
        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelValidationException(IEnumerable<IResultDetail> errors) : this(null, errors)
        {
        }

        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelValidationException(String message, IEnumerable<IResultDetail> errors) : base(message)
        {
            this.ValidationDetails = errors;
        }


        /// <summary>
        /// The errors from validation
        /// </summary>
        public IEnumerable<IResultDetail> ValidationDetails { get; private set; }

    }
}
