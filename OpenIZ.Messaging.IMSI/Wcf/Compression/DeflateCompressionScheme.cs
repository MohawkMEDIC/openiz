using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Wcf.Compression
{
    /// <summary>
    /// Compression scheme for deflate
    /// </summary>
    public class DeflateCompressionScheme : ICompressionScheme
    {
        /// <summary>
        /// Encoding this compression scheme handles
        /// </summary>
        public string Encoding
        {
            get
            {
                return "deflate";
            }
        }

        /// <summary>
        /// Create the compression stream
        /// </summary>
        public Stream CreateCompressionStream(Stream underlyingStream)
        {
            return new DeflateStream(underlyingStream, CompressionMode.Compress);
        }

        /// <summary>
        /// Create a decompression stream
        /// </summary>
        public Stream CreateDecompressionStream(Stream underlyingStream)
        {
            return new DeflateStream(underlyingStream, CompressionMode.Decompress);
        }
    }
}
