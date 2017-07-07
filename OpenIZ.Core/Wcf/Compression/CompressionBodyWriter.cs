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
 * Date: 2016-11-30
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Wcf.Compression
{

    /// <summary>
    /// Compression writer
    /// </summary>
    public class CompressionBodyWriter : BodyWriter
    {
        private byte[] m_data;
        private ICompressionScheme m_compressor;

        /// <summary>
        /// GZip writer
        /// </summary>
        public CompressionBodyWriter(byte[] data, ICompressionScheme compressor) : base(false)
        {
            this.m_data = data;
            this.m_compressor = compressor;
        }

        /// <summary>
        /// Write body contents
        /// </summary>
        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {

            writer.WriteStartDocument();
            
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream gzs = this.m_compressor.CreateCompressionStream(ms))
                {
                    gzs.Write(this.m_data, 0, this.m_data.Length);
                    gzs.Flush();
                }
                writer.WriteStartElement("Binary");
                byte[] arr = ms.ToArray();
                writer.WriteBase64(arr, 0, arr.Length);
                writer.WriteEndElement();

            }

            writer.WriteEndDocument();
        }
    }
}
