using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Configuration
{
    /// <summary>
    /// GS1 configuration 
    /// </summary>
    public class Gs1Configuration
    {
        /// <summary>
        /// Auto create materials
        /// </summary>
        public bool AutoCreateMaterials { get; internal set; }

        /// <summary>
        /// Default content owner assigning authority
        /// </summary>
        public String DefaultContentOwnerAssigningAuthority { get; internal set; }

        /// <summary>
        /// Gets the queue on which 
        /// </summary>
        public String Gs1QueueName { get; internal set; }
    }
}
