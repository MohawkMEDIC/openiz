using SharpCompress.Compressors.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Wcf.Compression
{
    /// <summary>
    /// BZip2 Compression stream
    /// </summary>
    public class BZip2CompressionScheme : ICompressionScheme
    {
        /// <summary>
        /// Get the encoding
        /// </summary>
        public string Encoding
        {
            get
            {
                return "bzip2";
            }
        }

        /// <summary>
        /// Create compression stream
        /// </summary>
        public Stream CreateCompressionStream(Stream underlyingStream)
        {
            return new BZip2Stream(underlyingStream, SharpCompress.Compressors.CompressionMode.Compress);
        }

        /// <summary>
        /// Create decompression stream
        /// </summary>
        public Stream CreateDecompressionStream(Stream underlyingStream)
        {
            return new BZip2Stream(underlyingStream, SharpCompress.Compressors.CompressionMode.Decompress);

        }
    }
}
