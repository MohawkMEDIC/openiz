using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenIZ.Messaging.IMSI.Wcf.Compression
{
    /// <summary>
    /// Text body writer
    /// </summary>
    public class TextBodyWriter : BodyWriter
    {
        byte[] m_bytes;

        public TextBodyWriter(byte[] message)
            : base(true)
        {
            this.m_bytes = message;
        }

        /// <summary>
        /// Write body contents
        /// </summary>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary");
            writer.WriteBase64(this.m_bytes, 0, this.m_bytes.Length);
            writer.WriteEndElement();
        }
    }
}
