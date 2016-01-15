using MARC.Everest.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Exceptions
{
    /// <summary>
    /// Represents a validation exception of a model map
    /// </summary>
    public class ModelMapValidationException : ModelValidationException
    {

        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelMapValidationException(IEnumerable<IResultDetail> errors) : this(null, errors)
        {
        }

        /// <summary>
        /// Creates a new model validation exception
        /// </summary>
        public ModelMapValidationException(String message, IEnumerable<IResultDetail> errors) : base(message, errors)
        {
        }


    }
}
