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
 * Date: 2016-8-2
 */
using System;
using System.Collections;
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
        /// Creates a new name value collection from the kvp array
        /// </summary>
        public NameValueCollection(KeyValuePair<string, object>[] kvpa)
        {
            foreach (var kv in kvpa)
                if(kv.Value is IList)
                    this.Add(kv.Key, (kv.Value as IList).OfType<Object>().Select(o=>o.ToString()).ToList());
                else
                    this.Add(kv.Key, kv.Value?.ToString());
        }

        /// <summary>
        /// Parse a query string
        /// </summary>
        public static NameValueCollection ParseQueryString(String qstring)
        {
            NameValueCollection retVal = new NameValueCollection();
            if (qstring.StartsWith("?")) qstring = qstring.Substring(1);
            foreach (var itm in qstring.Split('&'))
            {
                var expr = itm.Split('=');
                expr[0] = Uri.UnescapeDataString(expr[0]);
                expr[1] = Uri.UnescapeDataString(expr[1]);
                var value = expr[1].Replace('+', ' ').
                    Replace("%3A", ":").
                    Replace("%2F", "/").
                    Replace("%2A", "*").
                    Replace("%3C", "<").
                    Replace("%3E", ">").
                    Replace("%21", "!").
                    Replace("%3D", "=").
                    Replace("%5B", "[").
                    Replace("%5D", "]").
                    Replace("%7C", "|").Trim();
                // HACK: Replace this later
                if(!String.IsNullOrEmpty(value))
                    retVal.Add(expr[0].Trim(), value);
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

        /// <summary>
        /// Represent as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String queryString = String.Empty;
            foreach (var kv in this)
            {
                foreach (var val in kv.Value)
                {
                    queryString += String.Format("{0}={1}", kv.Key, Uri.EscapeDataString(val));
                    if (!kv.Equals(this.Last()))
                        queryString += "&";
                }
            }
            return queryString;
        }
    }

}
