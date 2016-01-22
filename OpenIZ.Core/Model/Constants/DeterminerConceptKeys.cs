using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Determiner codes
    /// </summary>
    public static class DeterminerKeys
    {
        /// <summary>
        /// QUALIFIEDKIND
        /// </summary>
        public static readonly Guid DescribedQualified = Guid.Parse("604CF1B7-8891-49FB-B95F-3E4E875691BC");
        /// <summary>
        /// instance
        /// </summary>
        public static readonly Guid Specific = Guid.Parse("F29F08DE-78A7-4A5E-AEAF-7B545BA19A09");
        /// <summary>
        /// Described
        /// </summary>
        public static readonly Guid Described = Guid.Parse("AD28A7AC-A66B-42C4-91B4-DE40A2B11980");
    }
}
