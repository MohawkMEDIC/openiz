using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a patch service which can calculate and apply patches
    /// </summary>
    public interface IPatchService
    {

        /// <summary>
        /// Performs a DIFF and creates the related patch which can be used to update <paramref name="existing"/>
        /// to <paramref name="updated"/>
        /// </summary>
        Patch Diff(IdentifiedData existing, IdentifiedData updated);

        /// <summary>
        /// Apples the specified <paramref name="patch"/> onto <paramref name="data"/> returning the updated object
        /// </summary>
        IdentifiedData Patch(Patch patch, IdentifiedData data, bool force = false);

        /// <summary>
        /// Tests that the patch can be applied on the specified object
        /// </summary>
        bool Test(Patch patch, IdentifiedData target);
    }
}
