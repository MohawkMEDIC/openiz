using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Model extension
    /// </summary>
    public interface IModelExtension
    {

        /// <summary>
        /// Gets the extension type key
        /// </summary>
        Guid ExtensionTypeKey { get; }

        /// <summary>
        /// Gets the data for the extension
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Gets the display value
        /// </summary>
        string Display { get; }

        /// <summary>
        /// Gets the value of the extension
        /// </summary>
        object Value { get; }
    }
}
