using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    /// <summary>
    /// Represents a class which can create care plans
    /// </summary>
    public interface ICarePlanService
    {

        /// <summary>
        /// Create a care plam
        /// </summary>
        IEnumerable<Act> CreateCarePlan(Patient p);
        
        /// <summary>
        /// Gets the list of protocols which can be or should be used to create the care plans
        /// </summary>
        List<IClinicalProtocol> Protocols { get; }

        /// <summary>
        /// Initializes the protocols
        /// </summary>
        void Initialize();
    }
}
