using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript.JNI
{
    /// <summary>
    /// Hacks to operate with CLR Objects
    /// </summary>
    public class JsObjectProvider
    {

        /// <summary>
        /// Get list of keys in expando object
        /// </summary>
        /// <param name="obj">The object keys</param>
        public object[] properties(ExpandoObject obj)
        {
            return (obj as IDictionary<String, Object>).Keys.ToArray();

        }
    }
}
