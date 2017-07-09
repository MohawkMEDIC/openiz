using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Extensions
{
    /// <summary>
    /// Binary extension handler
    /// </summary>
    public class BinaryExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the handler
        /// </summary>
        public string Name
        {
            get
            {
                return "Binary";
            }
        }

        /// <summary>
        /// Deserialize data
        /// </summary> 
        public object DeSerialize(byte[] extensionData)
        {
            return extensionData;
        }

        /// <summary>
        /// Get display 
        /// </summary>
        public string GetDisplay(object data)
        {
            return $"Binary Data : {(data as byte[]).Length} Bytes";
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public byte[] Serialize(object data)
        {
            return data as byte[];
        }
    }
}
