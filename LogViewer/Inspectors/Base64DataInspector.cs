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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    public class Base64TextDecoder : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "Base64 Text Decoder";
            }
        }

        public override string Inspect(string source)
        {
            try
            {
                var bytes = Convert.FromBase64String(source);
                return Encoding.UTF8.GetString(bytes);   
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }


    public class Base6DecodeInspector : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "Base64 Binary Decoder";
            }
        }
        
        public override string Inspect(string source)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                var bytes = Convert.FromBase64String(source);
                sb.AppendFormat("{0} bytes\r\n", bytes.Length);
                for (int i = 0; i < bytes.Length; i += 8)
                {
                    foreach (var e in Enumerable.Range(i, 8))
                    {
                        if (e < bytes.Length)
                            sb.AppendFormat("0x{0:X2} ", bytes[e]);
                        else
                            sb.AppendFormat("    ");
                        if (e % 4 == 3) sb.Append("\t");
                    }
                }
                return sb.ToString();
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }
}
