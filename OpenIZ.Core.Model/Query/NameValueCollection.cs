using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Query
{
    /// <summary>
    /// Name value collection
    /// </summary>
    public class NameValueCollection : Dictionary<String, List<String>>
    {


        /// <summary>
        /// Default constructor
        /// </summary>
        public NameValueCollection() : base()
        {
        }

        /// <summary>
        /// Name value collection iwth capacity
        /// </summary>
        public NameValueCollection(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public NameValueCollection(IDictionary<String, List<String>> dictionary) : base(dictionary)
        {

        }

        /// <summary>
        /// Parse a query string
        /// </summary>
        public static NameValueCollection ParseQueryString(String qstring)
        {
            NameValueCollection retVal = new NameValueCollection();
            foreach (var itm in qstring.Split('&'))
            {
                var expr = itm.Split('=');
                retVal.Add(expr[0], expr[1]);
            }
            return retVal;
        }

        // Sync root
        private Object m_syncRoot = new object();

        /// <summary>
        /// Add the specified key and value to the collection
        /// </summary>
        public void Add(String name, String value)
        {
            List<String> cValue = null;
            if (this.TryGetValue(name, out cValue))
                cValue.Add(value);
            else
                lock (this.m_syncRoot)
                    this.Add(name, new List<String>() { value });
        }
    }
}
