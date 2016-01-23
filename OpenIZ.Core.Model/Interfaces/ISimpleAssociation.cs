using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Represents bound relational data
    /// </summary>
    public interface ISimpleAssociation
    {

        /// <summary>
        /// Gets or sets the source entity key
        /// </summary>
        Guid SourceEntityKey { get; set; }
    }
}
