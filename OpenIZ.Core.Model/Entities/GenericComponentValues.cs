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
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// A generic class representing components of a larger item (i.e. address, name, etc);
	/// </summary>
	/// <typeparam name="TBoundModel"></typeparam>
	[Classifier(nameof(ComponentType)), SimpleValue(nameof(Value))]
	[XmlType(Namespace = "http://openiz.org/model"), JsonObject("GenericComponentValues")]
	public abstract class GenericComponentValues<TBoundModel> : Association<TBoundModel> where TBoundModel : IdentifiedData, new()
	{
		private Concept m_componentType;

		// Component type
		private Guid? m_componentTypeKey;

		// Component type
		/// <summary>
		/// Default ctor
		/// </summary>
		public GenericComponentValues()
		{
		}

		/// <summary>
		/// Creates a generic component value with the specified classifier
		/// </summary>
		public GenericComponentValues(Guid partType, String value)
		{
			this.m_componentTypeKey = partType;
			this.Value = value;
		}

		/// <summary>
		/// Constructor with the specified identifier
		/// </summary>
		public GenericComponentValues(String value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the type of address component
		/// </summary>
		[XmlIgnore, JsonIgnore]
		[SerializationReference(nameof(ComponentTypeKey)), AutoLoad]
		public Concept ComponentType
		{
			get
			{
				this.m_componentType = base.DelayLoad(this.m_componentTypeKey, this.m_componentType);
				return this.m_componentType;
			}
			set
			{
				this.m_componentType = value;
				this.m_componentTypeKey = value?.Key;
			}
		}

		/// <summary>
		/// Component type key
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("type"), JsonProperty("type")]
		public virtual Guid? ComponentTypeKey
		{
			get { return this.m_componentTypeKey; }
			set
			{
				if (this.m_componentTypeKey != value)
				{
					this.m_componentTypeKey = value;
					this.m_componentType = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the name component
		/// </summary>
		[XmlElement("value"), JsonProperty("value")]
		public String Value { get; set; }

		/// <summary>
		/// Gets if the item is empty
		/// </summary>
		/// <returns></returns>
		public override bool IsEmpty()
		{
			return String.IsNullOrEmpty(this.Value);
		}

		/// <summary>
		/// Forces refreshing of delay load properties
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();
			this.m_componentType = null;
		}

		/// <summary>
		/// Semantic equality function
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as GenericComponentValues<TBoundModel>;
			if (other == null) return false;
			return base.SemanticEquals(obj) &&
				this.Value == other.Value &&
				this.ComponentTypeKey == other.ComponentTypeKey;
		}

		/// <summary>
		/// Should serialize component type key
		/// </summary>
		public bool ShouldSerializeComponentTypeKey()
		{
			return this.ComponentTypeKey.HasValue;
		}
        /// <summary>
        /// Never need to serialize the entity source key
        /// </summary>
        /// <returns></returns>
        public override bool ShouldSerializeSourceEntityKey()
        {
            return false;
        }
	}
}