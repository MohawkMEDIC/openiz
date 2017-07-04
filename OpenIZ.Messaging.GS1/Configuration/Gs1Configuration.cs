using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.GS1.Configuration
{
    /// <summary>
    /// GS1 configuration 
    /// </summary>
    public class Gs1ConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Auto create materials
        /// </summary>
        [ConfigurationProperty("autoCreateMaterials")]
        public bool AutoCreateMaterials {
            get { return (bool)this["autoCreateMaterials"]; }
            set { this["autoCreateMaterials"] = value; }
        }

        /// <summary>
        /// Default content owner assigning authority
        /// </summary>
        [ConfigurationProperty("defaultAuthority")]
        public String DefaultContentOwnerAssigningAuthority {
            get { return (string)this["defaultAuthority"]; }
            set { this["defaultAuthority"] = value; }
        }

        /// <summary>
        /// Gets the queue on which to place messages
        /// </summary>
        [ConfigurationProperty("queueName")]
        public String Gs1QueueName {
            get { return (string)this["queueName"]; }
            set { this["queueName"] = value; }
        }

        /// <summary>
        /// Gets or sets the gs1 broker address
        /// </summary>
        [ConfigurationProperty("broker")]
        public As2ServiceElement Gs1BrokerAddress {
            get { return (As2ServiceElement)this["broker"]; }
            set { this["broker"] = value; }
        }

    }
}
