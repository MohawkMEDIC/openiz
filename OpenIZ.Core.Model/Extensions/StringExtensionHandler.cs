using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Extensions
{
    /// <summary>
    /// An extension handler that handles strings
    /// </summary>
    public class StringExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the extension handler
        /// </summary>
        public string Name
        {
            get
            {
                return "String";
            }
        }

        /// <summary>
        /// Parses the string from bytes
        /// </summary>
        public object DeSerialize(byte[] extensionData)
        {
            if (extensionData == null)
                return null;
            return Encoding.UTF8.GetString(extensionData, 0, extensionData.Length);
        }

        /// <summary>
        /// Get display representation
        /// </summary>
        public string GetDisplay(object data)
        {
            return data.ToString();
        }

        /// <summary>
        /// Serialize the value
        /// </summary>
        public byte[] Serialize(object data)
        {
            if (data == null)
                return null;
            return Encoding.UTF8.GetBytes(data.ToString());
        }
    }
}
