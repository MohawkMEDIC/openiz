using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Represents base entity data
    /// </summary>
    public interface IBaseEntityData : IIdentifiedEntity
    {
        /// <summary>
        /// Gets or sets the creator of the data
        /// </summary>
        Guid? CreatedByKey { get; set; }

        /// <summary>
        /// Gets or sets teh obsoletor of the data
        /// </summary>
        Guid? ObsoletedByKey { get; set; }

        /// <summary>
        /// Gets or sets the time when the data was created
        /// </summary>
        DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the time with the data was obsoleted.
        /// </summary>
        DateTimeOffset? ObsoletionTime { get; set; }
    }
}
