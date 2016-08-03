using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Protocol
{
    /// <summary>
    /// Represents a clinical protocol
    /// </summary>
    public interface IClinicalProtocol
    {

        /// <summary>
        /// Load the specified protocol data
        /// </summary>
        void Load(Core.Model.Acts.Protocol protocolData);

        /// <summary>
        /// Get the protocol data
        /// </summary>
        Core.Model.Acts.Protocol GetProtcolData();

        /// <summary>
        /// Gets the identifier for the protocol
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the protocol
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Calculate the clinical protocol for the given patient
        /// </summary>
        List<Act> Calculate(Patient p);

        /// <summary>
        /// Update the care plan based on new data
        /// </summary>
        List<Act> Update(Patient p, List<Act> existingPlan);

    }
}
