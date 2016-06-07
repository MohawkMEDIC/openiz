using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Constants
{
    /// <summary>
    /// Represents user classification keys
    /// </summary>
    public static class UserClassKeys
    {

        /// <summary>
        /// Represents a user which is an application
        /// </summary>
        public static readonly Guid ApplictionUser = Guid.Parse("E9CD4DAD-2759-4022-AB07-92FCFB236A98");
        /// <summary>
        /// Represents a user which is a human
        /// </summary>
        public static readonly Guid HumanUser = Guid.Parse("33932B42-6F4B-4659-8849-6ACA54139D8E");
        /// <summary>
        /// Represents a user which is a system user
        /// </summary>
        public static readonly Guid SystemUser = Guid.Parse("9F71BB34-9691-440F-8249-9C831EA16D58");
    }


}
