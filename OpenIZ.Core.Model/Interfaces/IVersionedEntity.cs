using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Versioned entity
    /// </summary>
    public interface IVersionedEntity : IIdentifiedEntity
    {

        /// <summary>
        /// Gets the version sequence
        /// </summary>
        decimal VersionSequence { get; set; }

        /// <summary>
        /// Gets the version key
        /// </summary>
        Guid VersionKey { get; set; }
    }
}
