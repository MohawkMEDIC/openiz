using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Mood concept keys
    /// </summary>
    public static class MoodConceptKeys
    {
        /// <summary>
        /// The act represents a goal to be acheived
        /// </summary>
        public static readonly Guid Goal = Guid.Parse("13925967-E748-4DD6-B562-1E1DA3DDFB06");
        /// <summary>
        /// The act represents a promise to do something
        /// </summary>
        public static readonly Guid Promise = Guid.Parse("B389DEDF-BE61-456B-AA70-786E1A5A69E0");
        /// <summary>
        /// The act represents a request by a person to perfrom some action
        /// </summary>
        public static readonly Guid Request = Guid.Parse("E658CA72-3B6A-4099-AB6E-7CF6861A5B61");
        /// <summary>
        /// The act represents an event that actually occurred
        /// </summary>
        public static readonly Guid Eventoccurrence = Guid.Parse("EC74541F-87C4-4327-A4B9-97F325501747");
        /// <summary>
        /// The act represents an intent that a human WILL do something
        /// </summary>
        public static readonly Guid Intent = Guid.Parse("099BCC5E-8E2F-4D50-B509-9F9D5BBEB58E");
        /// <summary>
        /// The act represents a proposal to do something
        /// </summary>
        public static readonly Guid Proposal = Guid.Parse("ACF7BAF2-221F-4BC2-8116-CEB5165BE079");
    }
}
