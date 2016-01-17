using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Concept identifiers
    /// </summary>
    public static class ConceptIds
    {

        /// <summary>
        /// Status - New
        /// </summary>
        public static readonly Guid StatusNew = Guid.Parse("C34FCBF1-E0FE-4989-90FD-0DC49E1B9685");
        /// <summary>
        /// Status - Obsolete
        /// </summary>
        public static readonly Guid StatusObsolete = Guid.Parse("BDEF5F90-5497-4F26-956C-8F818CCE2BD2");
        /// <summary>
        /// Status - Nullified
        /// </summary>
        public static readonly Guid StatusNullfied = Guid.Parse("CD4AA3C4-02D5-4CC9-9088-EF8F31E321C5");
        /// <summary>
        /// Status - Active
        /// </summary>
        public static readonly Guid StatusActive = Guid.Parse("C8064CBD-FA06-4530-B430-1A52F1530C27");
    }
}
