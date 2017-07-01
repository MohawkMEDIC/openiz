using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1
{
    /// <summary>
    /// GS1 extensions to core model attributes
    /// </summary>
    public static class Gs1ModelExtensions
    {
        /// <summary>
        /// Expeted delivery date
        /// </summary>
        public static readonly Guid ExpectedDeliveryDate = Guid.Parse("6b2ae7b5-158f-46d0-9ff9-0e3fa999dcaa");
        /// <summary>
        /// Actual shipment date
        /// </summary>
        public static readonly Guid ActualShipmentDate = Guid.Parse("34073745-018b-48d1-82da-c545391d294a");
    }
}
