using System;
using System.IO;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Binary body serializer
    /// </summary>
    public class BinaryBodySerializer : IBodySerializer
    {
        /// <summary>
        /// De-serialize to the desired type
        /// </summary>
        public object DeSerialize(Stream s)
        {
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                ms.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public void Serialize(Stream s, object o)
        {
            if (o is byte[])
            {
                using (var ms = new MemoryStream((byte[])o))
                    ms.CopyTo(s);
            }
            else if (o is Stream)
                (o as Stream).CopyTo(s);
            else
                throw new NotSupportedException("Object must be byte array");
        }
    }
}