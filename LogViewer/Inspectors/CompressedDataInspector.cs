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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    public class CompressedDataInspector : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "GZIP Decoder";
            }
        }

        public override string Inspect(string source)
        {
            try
            {
                var bytes = Convert.FromBase64String(source);
                using (var ms = new MemoryStream(bytes))
                {
                    try
                    {
                        using (GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
                        using (StreamReader sr = new StreamReader(gzs))
                            return sr.ReadToEnd();
                    }
                    catch 
                    {
                        using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Decompress))
                        using (StreamReader sr = new StreamReader(gzs))
                            return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }
}
