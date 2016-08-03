/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-22
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Core.Wcf.Serialization
{
    /// <summary>
    /// Text body writer
    /// </summary>
    public class RawBodyWriter : BodyWriter
    {
        private byte[] m_bytes;
        private Stream m_stream;

        public RawBodyWriter(byte[] message)
            : base(true)
        {
            this.m_bytes = message;
        }

        /// <summary>
        /// Creates a new raw body writer with specified stream
        /// </summary>
        public RawBodyWriter(Stream s) : base(true)
        {
            this.m_stream = s;
        }

        /// <summary>
        /// Write body contents
        /// </summary>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary");
            if (this.m_stream == null)
                writer.WriteBase64(this.m_bytes, 0, this.m_bytes.Length);
            else
            {
                byte[] buffer = new byte[this.m_stream.Length];
                this.m_stream.Read(buffer, 0, (int)this.m_stream.Length);
                writer.WriteBase64(buffer, 0, (int)this.m_stream.Length);
            }
            writer.WriteEndElement();
        }
    }
}
