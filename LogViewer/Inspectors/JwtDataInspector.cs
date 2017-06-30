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
 * Date: 2017-3-31
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    public class JwtDataInspector : Base6DecodeInspector
    {
        public override string Name
        {
            get
            {
                return "JSON Web Token";
            }
        }

        /// <summary>
        /// Decode a JWT
        /// </summary>
        public override string Inspect(string token)
        {
            String[] tokenObjects = token.Split('.');
            // Correct each token to be proper B64 encoding
            for (int i = 0; i < tokenObjects.Length; i++)
                tokenObjects[i] = tokenObjects[i].PadRight(tokenObjects[i].Length + (tokenObjects[i].Length % 4), '=').Replace("===", "=");

            StringBuilder sb = new StringBuilder();

            // Parse
            String header = Encoding.UTF8.GetString(Convert.FromBase64String(tokenObjects[0])),
                body = Encoding.UTF8.GetString(Convert.FromBase64String(tokenObjects[1])),
                signature = tokenObjects[2];

            // First, header
            JObject obj = JsonConvert.DeserializeObject<JObject>(header);
            sb.AppendFormat("JSON WEB TOKEN HEADER:\r\n" +
                            "======================\r\n" +
                            "Type: {0}\r\nSignature Algorithm: {1}\r\nKey Id: {2}\r\n\r\n",
                            obj["typ"], obj["alg"], obj["keyid"]);
            // Then, body
            obj = JsonConvert.DeserializeObject<JObject>(body);
            sb.Append("CLAIMS:\r\n" +
                      "=======\r\n");
            foreach(var kv in obj)
            {
                sb.AppendFormat("{0} = ", this.ConvertClaim(kv.Key));
                if (kv.Value is JArray)
                {
                    sb.Append("[\r\n");
                    foreach (var ai in kv.Value as JArray)
                        sb.AppendFormat("\t{0}\r\n", ai);
                    sb.Append("]\r\n");
                }
                else
                {
                    if (kv.Key == "nbf" || kv.Key == "exp")
                    {
                        var dt = new DateTime(1970, 1, 1);
                        dt = dt.AddSeconds(kv.Value.Value<Int32>());
                        sb.AppendFormat("{0:o}\r\n", dt);
                    }
                    else
                        sb.AppendFormat("{0}\r\n", kv.Value);
                }
            }
            
            sb.AppendFormat("\r\nSIGNATURE:\r\n" +
                      "==========\r\n{0}\r\n\r\n", base.Inspect(tokenObjects[2]));

            sb.AppendFormat("JSON HEADER:\r\n" +
                            "============\r\n" +
                            "{0}\r\n\r\n" +
                            "JSON BODY:\r\n" +
                            "==========\r\n{1}\r\n\r\n" +
                            "JSON SIGNATURE:\r\n" + 
                            "===============\r\n{2}",
                            JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(header), Formatting.Indented),
                            JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(body), Formatting.Indented),
                            signature);

            return sb.ToString();
        }

        private object ConvertClaim(string key)
        {
            switch (key)
            {
                case "role": return "Security Role";
                case "iss": return "Issued By";
                case "authmethod": return "Authentication Method";
                case "unique_name": return "Principal Name";
                case "http://openiz.org/claims/grant": return "Granted Policies";
                case "sub": return "Subject Identifier";
                case "aud": return "Token Audience";
                case "email": return "E-Mail Address";
                case "name": return "Name";
                case "nbf": return "Not Before";
                case "exp": return "Expiry";
                default: return key;
            }
        }
    }
}
