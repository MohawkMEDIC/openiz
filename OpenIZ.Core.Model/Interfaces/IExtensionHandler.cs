using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Interfaces
{
    /// <summary>
    /// Extension handler contract
    /// </summary>
    public interface IExtensionHandler
    {

        /// <summary>
        /// Gets the name of the handler
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Represents the data as a .net value
        /// </summary>
        object DeSerialize(byte[] extensionData);

        /// <summary>
        /// Serializes the data 
        /// </summary>
        byte[] Serialize(object data);

        /// <summary>
        /// Gets the display value
        /// </summary>
        String GetDisplay(object data);
    }
}
