using System;
using System.Collections.Generic;
using OpenIZ.Core.Model.Acts;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Protocol comparer
    /// </summary>
    internal class ProtocolComparer : IEqualityComparer<ActProtocol>
    {
        /// <summary>
        /// Two protocols equal each other
        /// </summary>
        public bool Equals(ActProtocol x, ActProtocol y)
        {
            return x.ProtocolKey == y.ProtocolKey;
        }

        public int GetHashCode(ActProtocol obj)
        {
            return obj.ProtocolKey.GetHashCode();
        }
    }
}