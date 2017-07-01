using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Extensions
{
    /// <summary>
    /// Date extension handler
    /// </summary>
    public class DateExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the handler
        /// </summary>
        public string Name
        {
            get
            {
                return "Date";
            }
        }

        /// <summary>
        /// Serialize to bytes
        /// </summary>
        public object DeSerialize(byte[] extensionData)
        {
            Int64 tickData = BitConverter.ToInt64(extensionData, 0);
            return new DateTime(tickData, DateTimeKind.Utc);
        }

        /// <summary>
        /// Get the display value
        /// </summary>
        public string GetDisplay(object data)
        {
            return ((DateTime)data).ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public byte[] Serialize(object data)
        {
            DateTime dt = (DateTime)data;
            if (dt.Kind == DateTimeKind.Local)
                dt = dt.ToUniversalTime(); // adjust to UTC ticks
            return BitConverter.GetBytes(dt.Ticks);
        }
    }
}
