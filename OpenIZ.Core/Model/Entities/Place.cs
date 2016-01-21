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
    public class Place : Entity
    {
        // Servics
        [NonSerialized]
        private List<PlaceService> m_services;

        /// <summary>
        /// Place ctor
        /// </summary>
        public Place()
        {
            base.ClassConceptKey = EntityClassConceptKeys.Place;
            base.DeterminerConceptKey = DeterminerConceptKeys.Specific;
        }

        /// <summary>
        /// Gets or sets the class concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "classConceptRef")]
        public override Guid ClassConceptKey
        {
            get
            {
                return base.ClassConceptKey;
            }

            set
            {
                if (value == EntityClassConceptKeys.Place ||
                    value == EntityClassConceptKeys.ServiceDeliveryLocation ||
                    value == EntityClassConceptKeys.State ||
                    value == EntityClassConceptKeys.CityOrTown)
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


    }
}
