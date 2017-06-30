using Newtonsoft.Json;
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
			        return jsz.Deserialize(jr);
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
