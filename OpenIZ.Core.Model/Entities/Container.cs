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
 * Date: 2017-4-4
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents a container.
	/// </summary>
	/// <seealso cref="OpenIZ.Core.Model.Entities.ManufacturedMaterial" />
    [XmlType(nameof(Container), Namespace = "http://openiz.org/model")]
    [XmlRoot(nameof(Container), Namespace = "http://openiz.org/model")]
    [JsonObject(nameof(Container))]
	public class Container : ManufacturedMaterial
	{
		/// <summary>
		/// The cap type concept.
		/// </summary>
		private Concept capTypeConcept;

		/// <summary>
		/// The cap type concept key.
		/// </summary>
		private Guid? capTypeConceptKey;

		/// <summary>
		/// The separator type concept.
		/// </summary>
		private Concept separatorTypeConcept;

		/// <summary>
		/// The separator type concept key.
		/// </summary>
		private Guid? separatorTypeConceptKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="Container"/> class.
		/// </summary>
		public Container()
		{
			this.ClassConceptKey = EntityClassKeys.Container;
			this.DeterminerConceptKey = DeterminerKeys.Specific;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Container"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public Container(Guid key)
		{
			this.Key = key;
		}

		/// <summary>
		/// Gets or sets the barrier delta quantity. The distance from the Point of Reference to the separator material (barrier) within a container.
		/// </summary>
		/// <value>The barrier delta quantity.</value>
		[XmlElement("barrierDeltaQuantity"), JsonProperty("barrierDeltaQuantity")]
		public decimal? BarrierDeltaQuantity { get; set; }

		/// <summary>
		/// Gets or sets the bottom delta quantity. The distance from the Point of Reference to the outside bottom of the container.
		/// </summary>
		/// <value>The bottom delta quantity.</value>
		[XmlElement("bottomDeltaQuantity"), JsonProperty("bottomDeltaQuantity")]
		public decimal? BottomDeltaQuantity { get; set; }

		/// <summary>
		/// Gets or sets the capacity quantity. The functional capacity of the container.
		/// </summary>
		/// <value>The capacity quantity.</value>
		[XmlElement("capacityQuantity"), JsonProperty("capacityQuantity")]
		public decimal? CapacityQuantity { get; set; }

		/// <summary>
		/// Gets or sets the cap type concept. The type of container cap consistent with de-capping, piercing or other automated manipulation.
		/// </summary>
		/// <value>The cap type concept.</value>
		[XmlIgnore, JsonIgnore]
		[SerializationReference(nameof(CapTypeConceptKey))]
		public Concept CapTypeConcept
		{
			get
			{
				capTypeConcept = base.DelayLoad(this.capTypeConceptKey, this.capTypeConcept);
				return this.capTypeConcept;
			}
			set
			{
				this.capTypeConcept = value;
				this.capTypeConceptKey = value?.Key;
			}
		}

		/// <summary>
		/// Gets or sets the cap type concept key.
		/// </summary>
		/// <value>The cap type concept key.</value>
		[XmlElement("capTypeConcept"), JsonProperty("capTypeConcept")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Guid? CapTypeConceptKey
		{
			get { return this.capTypeConceptKey; }
			set
			{
				if (this.capTypeConceptKey != value)
				{
					this.capTypeConceptKey = value;
					this.capTypeConcept = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the diameter quantity. The outside diameter of the container.
		/// </summary>
		/// <value>The diameter quantity.</value>
		[XmlElement("diameterQuantity"), JsonProperty("diameterQuantity")]
		public decimal? DiameterQuantity { get; set; }

		/// <summary>
		/// Gets or sets the height quantity. The height of the container.
		/// </summary>
		/// <value>The height quantity.</value>
		[XmlElement("heightQuantity"), JsonProperty("heightQuantity")]
		public decimal? HeightQuantity { get; set; }

		/// <summary>
		/// Gets or sets the separator type concept. A material added to a container to facilitate and create a physical separation of specimen components of differing density.
		/// Examples: A gel material added to blood collection tubes that following centrifugation creates a physical barrier between the blood cells and the serum or plasma.
		/// </summary>
		/// <value>The separator type concept.</value>
		[XmlIgnore, JsonIgnore]
		[SerializationReference(nameof(SeparatorTypeConceptKey))]
		public Concept SeparatorTypeConcept
		{
			get
			{
				this.separatorTypeConcept = base.DelayLoad(this.separatorTypeConceptKey, this.separatorTypeConcept);
				return this.separatorTypeConcept;
			}
			set
			{
				this.separatorTypeConcept = value;
				this.separatorTypeConceptKey = value?.Key;
			}
		}

		/// <summary>
		/// Gets or sets the separator type concept key.
		/// </summary>
		/// <value>The separator type concept key.</value>
		[XmlElement("separatorTypeConcept"), JsonProperty("separatorTypeConcept")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Guid? SeparatorTypeConceptKey
		{
			get { return this.separatorTypeConceptKey; }
			set
			{
				if (this.separatorTypeConceptKey != value)
				{
					this.separatorTypeConceptKey = value;
					this.separatorTypeConcept = null;
				}
			}
		}

		/// <summary>
		/// Determines if two containers are semantically equal.
		/// </summary>
		/// <param name="obj">The container to compare against.</param>
		/// <returns>Returns true if the two containers are equal, otherwise false.</returns>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as Container;

			if (other == null)
			{
				return false;
			}

			return base.SemanticEquals(other) &&
					this.BarrierDeltaQuantity == other.BarrierDeltaQuantity &&
					this.BottomDeltaQuantity == other.BottomDeltaQuantity &&
					this.CapTypeConceptKey == other.CapTypeConceptKey &&
					this.CapacityQuantity == other.CapacityQuantity &&
					this.DiameterQuantity == other.DiameterQuantity &&
					this.HeightQuantity == other.HeightQuantity &&
					this.SeparatorTypeConceptKey == other.SeparatorTypeConceptKey;
		}
	}
}