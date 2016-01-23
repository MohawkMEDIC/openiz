using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Identified entity
    /// </summary>
    public interface IIdentifiedEntity
    {

        /// <summary>
        /// Gets the identifier for the entity
        /// </summary>
        Guid Key { get; set; }

    }
}
