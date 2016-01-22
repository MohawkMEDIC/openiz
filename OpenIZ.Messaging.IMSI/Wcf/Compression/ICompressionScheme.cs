using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.Wcf.Compression
{
    /// <summary>
    /// Represents a compression scheme
    /// </summary>
    public interface ICompressionScheme
    {

        /// <summary>
        /// Get the encoding 
        /// </summary>
        string Encoding { get; }
        /// <summary>
        /// Create a stream that compresses
        /// </summary>
        Stream CreateCompressionStream(Stream underlyingStream);
        /// <summary>
        /// Create a stream that de-compresses
        /// </summary>
        Stream CreateDecompressionStream(Stream underlyingStream);
    }


}
