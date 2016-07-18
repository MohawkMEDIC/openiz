/*
 * Copyright 2016-2016 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2016-2-1
 */
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

        /// <summary>
        /// Place ctor
        /// </summary>
        public Place()
        {
            base.ClassConceptKey = EntityClassKeys.Place;
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            this.Services = new List<PlaceService>();
        }

        /// <summary>
        /// Gets or sets the class concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("classConcept"), JsonProperty("classConcept")]
        public override Guid? ClassConceptKey
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
                    value == EntityClassKeys.CityOrTown ||
                    value == EntityClassKeys.Country ||
                    value == EntityClassKeys.CountyOrParish)
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
        public double? Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude
        /// </summary>
        [XmlElement("lng"), JsonProperty("lng")]
        public double? Lng { get; set; }

        /// <summary>
        /// Should serialize Lat?
        /// </summary>
        public bool ShouldSerializeLat() { return this.Lat.HasValue;  }

        /// <summary>
        /// Should serialize longitude
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeLng() { return this.Lng.HasValue; }

        /// <summary>
        /// Gets the services
        /// </summary>
        [AutoLoad]
        [XmlElement("service"), JsonProperty("service")]
        public List<PlaceService> Services { get; set; }


    }
}
