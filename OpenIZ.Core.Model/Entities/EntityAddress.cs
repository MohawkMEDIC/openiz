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
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Entity address
	/// </summary>
	[Classifier(nameof(AddressUse))]
	[XmlType("EntityAddress", Namespace = "http://openiz.org/model"), JsonObject("EntityAddress")]
	public class EntityAddress : VersionedAssociation<Entity>
	{
		// Address use concept
		private Concept m_addressUseConcept;

		// Address use key
		private Guid? m_addressUseKey;

		/// <summary>
		/// Create the address from components
		/// </summary>
		public EntityAddress(Guid useKey, String streetAddressLine, String city, String province, String country, String zipCode)
		{
			this.m_addressUseKey = useKey;
			this.Component = new List<EntityAddressComponent>();
			if (!String.IsNullOrEmpty(streetAddressLine))
				this.Component.Add(new EntityAddressComponent(AddressComponentKeys.StreetAddressLine, streetAddressLine));
			if (!String.IsNullOrEmpty(city))
				this.Component.Add(new EntityAddressComponent(AddressComponentKeys.City, city));
			if (!String.IsNullOrEmpty(province))
				this.Component.Add(new EntityAddressComponent(AddressComponentKeys.State, province));
			if (!String.IsNullOrEmpty(country))
				this.Component.Add(new EntityAddressComponent(AddressComponentKeys.Country, country));
			if (!String.IsNullOrEmpty(zipCode))
				this.Component.Add(new EntityAddressComponent(AddressComponentKeys.PostalCode, zipCode));
		}

        /// <summary>
        /// Create the address from components
        /// </summary>
        public EntityAddress(Guid useKey, String streetAddressLine, String precinct, String city, String county, String province, String country, String zipCode) : this(useKey, streetAddressLine, city, province, country, zipCode)
        {
            if (!String.IsNullOrEmpty(precinct))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.Precinct, precinct));
            if (!String.IsNullOrEmpty(county))
                this.Component.Add(new EntityAddressComponent(AddressComponentKeys.County, county));
        }

        /// <summary>
        /// Default CTOR
        /// </summary>
        public EntityAddress()
		{
			this.Component = new List<EntityAddressComponent>();
		}

		/// <summary>
		/// Gets or sets the address use
		/// </summary>
		[SerializationReference(nameof(AddressUseKey))]
		[XmlIgnore, JsonIgnore]
		[AutoLoad]
		public Concept AddressUse
		{
			get
			{
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
		/// Gets or sets the address use key
		/// </summary>
		[XmlElement("use"), JsonProperty("use")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Binding(typeof(AddressUseKeys))]
		public Guid? AddressUseKey
		{
			get { return this.m_addressUseKey; }
			set
			{
				if (this.m_addressUseKey != value)
				{
					this.m_addressUseKey = value;
					this.m_addressUseConcept = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the component types
		/// </summary>
		[XmlElement("component"), JsonProperty("component")]
		[AutoLoad]
		public List<EntityAddressComponent> Component { get; set; }


        /// <summary>
        /// Get components
        /// </summary>
        public string GetComponent(Guid key)
        {
            var comps = this.LoadCollection<EntityAddressComponent>("Component");
            return comps.FirstOrDefault(o => o.ComponentTypeKey == key)?.Value;
        }

        /// <summary>
        /// Remove empty components
        /// </summary>
        public override IdentifiedData Clean()
		{
			this.Component.RemoveAll(o => String.IsNullOrEmpty(o.Value));
			return this;
		}

		/// <summary>
		/// True if empty
		/// </summary>
		/// <returns></returns>
		public override bool IsEmpty()
		{
			return this.Component.Count == 0;
		}

		/// <summary>
		/// Semantic equality function
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as EntityAddress;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.AddressUseKey == other.AddressUseKey &&
				this.Component?.SemanticEquals(other.Component) == true;
		}

        /// <summary>
        /// Never need to serialize the entity source key
        /// </summary>
        /// <returns></returns>
        public override bool ShouldSerializeSourceEntityKey()
        {
            return false;
        }

        /// <summary>
        /// Represent as display
        /// </summary>
        public override string ToDisplay()
        {

            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.StreetAddressLine)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.StreetAddressLine));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.Precinct)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.Precinct));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.City)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.City));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.County)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.County));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.State)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.State));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.Country)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.Country));
            if (!string.IsNullOrEmpty(this.GetComponent(AddressComponentKeys.PostalCode)))
                sb.AppendFormat("{0}, ", this.GetComponent(AddressComponentKeys.PostalCode));

            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}