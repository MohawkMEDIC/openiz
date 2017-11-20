/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
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
 * User: justi
 * Date: 2016-7-16
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// An entity which is a place where healthcare services are delivered
	/// </summary>

	[XmlType("Place", Namespace = "http://openiz.org/model"), JsonObject("Place")]
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
		[XmlElement("classConcept"), JsonProperty("classConcept")]
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
        /// Should serialize mobile
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeIsMobile() => this.IsMobile;

        /// <summary>
        /// Should serialize latitude
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeLat() => this.Lat.GetValueOrDefault() != 0;

        /// <summary>
        /// Should serialize longitude
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeLng() => this.Lng.GetValueOrDefault() != 0;

		/// <summary>
		/// Gets the services
		/// </summary>
		[AutoLoad, XmlElement("service"), JsonProperty("service")]
		public List<PlaceService> Services { get; set; }

		/// <summary>
		/// Determine semantic equality
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as Place;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.IsMobile == other.IsMobile &&
				this.Lat == other.Lat &&
				this.Lng == other.Lng &&
				this.Services?.SemanticEquals(other.Services) == true;
		}
	}
}