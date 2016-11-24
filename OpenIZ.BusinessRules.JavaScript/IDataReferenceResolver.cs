using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript
{
    /// <summary>
    /// Reference resolver is responsible for turning a file path into a stream
    /// </summary>
    public interface IDataReferenceResolver
    {

        /// <summary>
        /// Resolves the specified reference text 
        /// </summary>
        Stream Resolve(String reference);

    }
}
