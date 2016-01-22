using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// An entity which is a place where healthcare services are delivered
    /// </summary>
    [Serializable]
    [DataContract(Name = "Place", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Clinical)]
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
        [Browsable(false)]
        [DataMember(Name = "classConcept")]
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
        [DataMember(Name = "isMobile")]
        public Boolean IsMobile { get; set; }

        /// <summary>
        /// Gets or sets the latitude
        /// </summary>
        [DataMember(Name = "lat")]
        public float Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        [DataMember(Name = "lng")]
        public float Lng { get; set; }

        /// <summary>
        /// Gets the services
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "service")]
        public List<PlaceService> Services
        {
            get
            {
                if(this.m_services == null)
                {
                    var dataPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<PlaceService>>();
                    this.m_services = dataPersistence.Query(s=>s.SourceEntityKey == this.Key, null).ToList();
                }
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
