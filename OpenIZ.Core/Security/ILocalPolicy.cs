using MARC.HI.EHRS.SVC.Core.Services.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Security
{
    /// <summary>
    /// Represents an OpenImmunize policy
    /// </summary>
    public interface ILocalPolicy : IPolicy
    {

        /// <summary>
        /// The handler
        /// </summary>
        IPolicyHandler Handler { get; }

    }
}
