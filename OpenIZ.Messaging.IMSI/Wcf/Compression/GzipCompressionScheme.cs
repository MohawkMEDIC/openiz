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
    /// Represents a compresson scheme which can deflate objects
    /// </summary>
    public class GzipCompressionScheme : ICompressionScheme
    {
        /// <summary>
        /// Encoding this scheme handles
        /// </summary>
        public string Encoding
        {
            get
            {
                return "gzip";
            }
        }

        /// <summary>
        /// Create a compression stream
        /// </summary>
        public Stream CreateCompressionStream(Stream underlyingStream)
        {
            return new GZipStream(underlyingStream, CompressionMode.Compress);
        }

        /// <summary>
        /// Create a decompression stream
        /// </summary>
        public Stream CreateDecompressionStream(Stream underlyingStream)
        {
            return new GZipStream(underlyingStream, CompressionMode.Decompress);
        }
    }
}
