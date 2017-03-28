/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-8-2
 */
using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Extensions
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
	        if (extensionData == null)
	        {
		        return default(decimal);
	        }

            Int32[] ints = new int[]
            {
                BitConverter.ToInt32(extensionData, 0),
                BitConverter.ToInt32(extensionData, 4),
                BitConverter.ToInt32(extensionData, 8),
            };

			// only attempt to convert the value with a start index of 12, if the value acutally has a length of 12 or greater
	        if (extensionData.Length >= 12)
	        {
		        ints[ints.Length + 1] = BitConverter.ToInt32(extensionData, 12);
	        }

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
