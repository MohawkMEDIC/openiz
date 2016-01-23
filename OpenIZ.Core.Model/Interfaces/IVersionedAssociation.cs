using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Versioned relationship
    /// </summary>
    public interface IVersionedAssociation : ISimpleAssociation
    {

        /// <summary>
        /// Effective version sequence
        /// </summary>
        Decimal EffectiveVersionSequenceId { get; set; }

        /// <summary>
        /// Obsolete version sequence
        /// </summary>
        Decimal? ObsoleteVersionSequenceId { get; set; }
    }
}
