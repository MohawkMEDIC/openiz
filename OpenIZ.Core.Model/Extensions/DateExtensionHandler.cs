/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-7-4
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
