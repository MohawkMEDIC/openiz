/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
