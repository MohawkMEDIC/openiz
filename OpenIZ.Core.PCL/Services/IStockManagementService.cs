using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services
{
    
    /// <summary>
    /// Represents an abstract way for PCL classes to interact with persistence events that occur on the back end
    /// </summary>
    public interface IStockManagementService
    {

        /// <summary>
        /// Performs a stock adjustment for the specified facility and material
        /// </summary>
        Act Adjust(ManufacturedMaterial material, Place place, int quantity, Concept reason);

        /// <summary>
        /// Gets the balance for the material
        /// </summary>
        int GetBalance(Place place, ManufacturedMaterial material);

    }
}
