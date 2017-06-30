using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.RISI.Wcf
{
    /// <summary>
    /// Http helper extensions
    /// </summary>
    public static class HttpHelperExtensions
    {

        /// <summary>
        /// Convert query types
        /// </summary>
        public static OpenIZ.Core.Model.Query.NameValueCollection ToQuery(this System.Collections.Specialized.NameValueCollection nvc)
        {
            var retVal = new OpenIZ.Core.Model.Query.NameValueCollection();
            foreach (var k in nvc.AllKeys)
                retVal.Add(k, new List<String>(nvc.GetValues(k)));
            return retVal;
        }
    }

}
