using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Extension
{
    /// <summary>
    /// Extension handler that can handle decimal values
    /// </summary>
    public class DecimalExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the handler
        /// </summary>
        public string Name
        {
            get
            {
                return "Decimal";
            }
        }

        /// <summary>
        /// De-serializes data
        /// </summary>
        public object DeSerialize(byte[] extensionData)
        {
            Int32[] ints = new int[]
            {
                BitConverter.ToInt32(extensionData, 0),
                BitConverter.ToInt32(extensionData, 4),
                BitConverter.ToInt32(extensionData, 8),
                BitConverter.ToInt32(extensionData, 12)
            };
            return new Decimal(ints);
        }

        /// <summary>
        /// Get display
        /// </summary>
        public string GetDisplay(object data)
        {
            return data.ToString();
        }

        /// <summary>
        /// Serialize the data
        /// </summary>
        public byte[] Serialize(object data)
        {
            var ints = Decimal.GetBits((Decimal)data);
            byte[] retVal = new byte[16];
            for(int i = 0; i < ints.Length; i++)
                Array.Copy(BitConverter.GetBytes(ints[i]), 0, retVal, i * 4, 4);
            return retVal;
        }
    }
}
