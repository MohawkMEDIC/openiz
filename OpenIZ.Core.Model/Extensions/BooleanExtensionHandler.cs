using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Extensions
{
    /// <summary>
    /// Boolean extension handler
    /// </summary>
    public class BooleanExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the handler
        /// </summary>
        public string Name
        {
            get
            {
                return "Boolean";
            }
        }

        /// <summary>
        /// Gets the boolean obect from a byte array
        /// </summary>
        public object DeSerialize(byte[] extensionData)
        {
            return BitConverter.ToBoolean(extensionData, 0);
        }

        /// <summary>
        /// Get display name
        /// </summary>
        public string GetDisplay(object data)
        {
            return data.ToString();
        }

        /// <summary>
        /// Serialize the data into byte array
        /// </summary>
        public byte[] Serialize(object data)
        {
            return BitConverter.GetBytes((bool)data);
        }
    }
}
