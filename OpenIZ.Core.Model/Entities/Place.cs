

using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// An entity which is a place where healthcare services are delivered
    /// </summary>
    
    [XmlType("Place",  Namespace = "http://openiz.org/model"), JsonObject("Place")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Place")]
    public class Place : Entity
    {
        // Servics
        
        private List<PlaceService> m_services;

        /// <summary>
        /// Place ctor
        /// </summary>
        public Place()
        {
            base.ClassConceptKey = EntityClassKeys.Place;
            base.DeterminerConceptKey = DeterminerKeys.Specific;
        }

        /// <summary>
        /// Gets or sets the class concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("classConcept"), JsonProperty("classConcept")]
        public override Guid ClassConceptKey
        {
            get
            {
                return base.ClassConceptKey;
            }

            set
            {
                if (value == EntityClassKeys.Place ||
                    value == EntityClassKeys.ServiceDeliveryLocation ||
                    value == EntityClassKeys.State ||
                    value == EntityClassKeys.CityOrTown)
                    base.ClassConceptKey = value;
                else throw new ArgumentOutOfRangeException("Invalid ClassConceptKey value");
            }

        }

        /// <summary>
        /// True if location is mobile
        /// </summary>
        [XmlElement("isMobile"), JsonProperty("isMobile")]
        public Boolean IsMobile { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        [XmlElement("lat"), JsonProperty("lat")]
        public float Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        [XmlElement("lng"), JsonProperty("lng")]
        public float Lng { get; set; }

        /// <summary>
        /// Gets the services
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("service"), JsonProperty("service")]
        public List<PlaceService> Services
        {
            get
            {
                if (this.m_services == null)
                    this.m_services = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, m_services);
                return this.m_services;
            }
        }

        /// <summary>
        /// Refresh the place entity
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_services = null;
        }

    }
}
