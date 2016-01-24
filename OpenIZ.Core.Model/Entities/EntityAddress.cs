

using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Linq;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Entity address
    /// </summary>
    
    [XmlType("EntityAddress",  Namespace = "http://openiz.org/model"), JsonObject("EntityAddress")]
    public class EntityAddress : VersionedAssociation<Entity>
    {

        // Address use key
        private Guid? m_addressUseKey;
        // Address use concept
        
        private Concept m_addressUseConcept;
        // Address components
        
        private List<EntityAddressComponent> m_addressComponents;

        /// <summary>
        /// Gets or sets the address use key
        /// </summary>
        [XmlElement("addressUse"), JsonProperty("addressUse")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? AddressUseKey
        {
            get { return this.m_addressUseKey; }
            set
            {
                this.m_addressUseKey = value;
                this.m_addressUseConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the address use
        /// </summary>
        [DelayLoad(nameof(AddressUseKey))]
        [XmlIgnore, JsonIgnore]
        public Concept AddressUse
        {
            get {
                this.m_addressUseConcept = base.DelayLoad(this.m_addressUseKey, this.m_addressUseConcept);
                return this.m_addressUseConcept;
            }
            set
            {
                this.m_addressUseConcept = value;
                this.m_addressUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the component types
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("component"), JsonProperty("component")]
        public List<EntityAddressComponent> Component
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_addressComponents = EntitySource.Current.GetRelations(this.Key, this.m_addressComponents);
                return this.m_addressComponents;
            }
        }

        /// <summary>
        /// Force linked properties to delay load
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_addressComponents = null;
            this.m_addressUseKey = null;
        }
    }
}