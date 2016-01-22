using OpenIZ.Messaging.IMSI.Wcf.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Wcf.Compression
{

    /// <summary>
    /// Compression writer
    /// </summary>
    public class CompressionWriter : BodyWriter
    {
        private byte[] m_data;
        private ICompressionScheme m_compressor;

        /// <summary>
        /// GZip writer
        /// </summary>
        public CompressionWriter(byte[] data, ICompressionScheme compressor) : base(false)
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
