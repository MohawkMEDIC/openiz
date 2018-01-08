using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// The query parameter attribute allows for the querying of non-serialized values such as simple values or "handy" shortcut values
    /// </summary>
    public class QueryParameterAttribute : Attribute
    {

        /// <summary>
        /// Creates a new query parameter attribute
        /// </summary>
        public QueryParameterAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        public String ParameterName { get; set; }
    }
}
