using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Represents a object that can be extended with IModelExtensions
    /// </summary>
    public interface IExtendable
    {

        /// <summary>
        /// Gets the list of extensions
        /// </summary>
        IEnumerable<IModelExtension> Extensions { get; }

    }
}
