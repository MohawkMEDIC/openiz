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
 * Date: 2017-4-17
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Extensions
{
    /// <summary>
    /// Extension that emits data as kvp
    /// </summary>
    public class DictionaryExtensionHandler : IExtensionHandler
    {
        /// <summary>
        /// Gets the name of the extension
        /// </summary>
        public string Name
        {
            get
            {
                return "Dictionary";
            }
        }

        /// <summary>
        /// Deserialize data from the extension
        /// </summary>
        public object DeSerialize(byte[] extensionData)
        {
			// HACK: will fix ASAP, deadlines are deadlines
	        try
	        {
		        JsonSerializer jsz = new JsonSerializer();
		        using (var ms = new MemoryStream(extensionData))
		        using (var tr = new StreamReader(ms))
		        using (var jr = new JsonTextReader(tr))
		        {
			        var obj = jsz.Deserialize<dynamic>(jr);
                    if (obj is JArray)
                        return (obj as JArray).Values<dynamic>().ToArray();
                    else if (obj is JObject)
                        return (obj as JObject).Value<dynamic>();
		        }
			}
	        catch
	        {
		        // ignored
	        }

	        return null;
        }

        /// <summary>
        /// Get display value
        /// </summary>
        public string GetDisplay(object data)
        {
            // Anything that is smart enough to read JSON data is smart enough to use the raw stream data
            // (We also want to prevent the raw literatl from going in the db)
            var strData = JsonConvert.SerializeObject(data);
            if(strData.Length > 64)
                strData = strData.Substring(0, 64);
            return strData;
	        //return this.DeSerialize(data);
	        //return null;
        }

        /// <summary>
        /// Serialize actual data
        /// </summary>
        public byte[] Serialize(object data)
        {
            JsonSerializer jsz = new JsonSerializer();
            using (var ms = new MemoryStream())
            {
                using (var tr = new StreamWriter(ms))
                using (var jr = new JsonTextWriter(tr))
                    jsz.Serialize(jr, data);
                return ms.ToArray();
            }
        }
    }
}
