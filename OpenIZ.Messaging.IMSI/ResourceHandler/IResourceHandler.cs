using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Resource handler 
    /// </summary>
    public interface IResourceHandler
    {

        /// <summary>
        /// Get the resource name handled
        /// </summary>
        String ResourceName { get; }
        /// <summary>
        /// Get the specified resource instance
        /// </summary>
        IdentifiedData Get(Guid id, Guid versionId);

    }
}
