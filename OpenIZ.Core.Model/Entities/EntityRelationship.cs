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
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
	/// <summary>
	/// Represents an association between two entities
	/// </summary>
	[Classifier(nameof(RelationshipType))]
	[XmlRoot("EntityRelationship", Namespace = "http://openiz.org/model")]
	[XmlType("EntityRelationship", Namespace = "http://openiz.org/model"), JsonObject("EntityRelationship")]
	public class EntityRelationship : VersionedAssociation<Entity>
	{
		// The association type key
		private Guid? m_associationTypeKey;

		private Concept m_relationshipType;

		private Entity m_targetEntity;

		// The entity key
		private Guid? m_targetEntityKey;

		// The target entity
		// The association type
		/// <summary>
		/// Default constructor for entity relationship
		/// </summary>
		public EntityRelationship()
		{
		}

		/// <summary>
		/// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
		/// </summary>
		public EntityRelationship(Guid? relationshipType, Entity target)
		{
			this.RelationshipTypeKey = relationshipType;
			this.TargetEntity = target;
		}

		/// <summary>
		/// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
		/// </summary>
		public EntityRelationship(Guid? relationshipType, Guid? targetKey)
		{
			this.RelationshipTypeKey = relationshipType;
			this.TargetEntityKey = targetKey;
		}

		/// <summary>
		/// The entity that this relationship targets
		/// </summary>
		[XmlIgnore, JsonIgnore, SerializationReference(nameof(HolderKey)), DataIgnore]
		public Entity Holder
		{
			get
			{
				return this.SourceEntity;
			}
			set
			{
				this.SourceEntity = value;
			}
		}

		/// <summary>
		/// The entity that this relationship targets
		/// </summary>
		[JsonProperty("holder"), XmlElement("holder")]
		public Guid? HolderKey
		{
			get
			{
				return this.SourceEntityKey;
			}
			set
			{
				this.SourceEntityKey = value;
			}
		}

		/// <summary>
		/// The inversion indicator
		/// </summary>
		[XmlElement("inversionInd"), JsonProperty("inversionInd")]
		public bool InversionIndicator { get; set; }

		/// <summary>
		/// Represents the quantity of target in source
		/// </summary>
		[XmlElement("quantity"), JsonProperty("quantity")]
		public int? Quantity { get; set; }

		/// <summary>
		/// Gets or sets the association type
		/// </summary>
		[AutoLoad]
		[XmlIgnore, JsonIgnore]
		[SerializationReference(nameof(RelationshipTypeKey))]
		public Concept RelationshipType
		{
			get
			{
				this.m_relationshipType = base.DelayLoad(this.m_associationTypeKey, this.m_relationshipType);
				return this.m_relationshipType;
			}
			set
			{
				this.m_relationshipType = value;
				this.m_associationTypeKey = value?.Key;
			}
		}

		/// <summary>
		/// Association type key
		/// </summary>
		[XmlElement("relationshipType"), JsonProperty("relationshipType")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Binding(typeof(EntityRelationshipTypeKeys))]
		public Guid? RelationshipTypeKey
		{
			get { return this.m_associationTypeKey; }
			set
			{
				if (this.m_associationTypeKey != value)
				{
					this.m_associationTypeKey = value;
					this.m_relationshipType = null;
				}
			}
		}

		/// <summary>
		/// Target entity reference
		/// </summary>
		[SerializationReference(nameof(TargetEntityKey))]
		[XmlIgnore, JsonIgnore]
		public Entity TargetEntity
		{
			get
			{
				this.m_targetEntity = base.DelayLoad(this.m_targetEntityKey, this.m_targetEntity);
				return this.m_targetEntity;
			}
			set
			{
				this.m_targetEntity = value;
				this.m_targetEntityKey = value?.Key;
			}
		}

		/// <summary>
		/// The target of the association
		/// </summary>
		[XmlElement("target"), JsonProperty("target")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Guid? TargetEntityKey
		{
			get { return this.m_targetEntityKey; }
			set
			{
				if (this.m_targetEntityKey != value)
				{
					this.m_targetEntityKey = value;
					this.m_targetEntity = null;
				}
			}
		}

		/// <summary>
		/// Clean the entity
		/// </summary>
		/// <returns></returns>
		public override IdentifiedData Clean()
		{
			this.m_targetEntity = this.m_targetEntity?.Clean() as Entity;
			return this;
		}

		/// <summary>
		/// Is empty
		/// </summary>
		/// <returns></returns>
		public override bool IsEmpty()
		{
			return this.RelationshipType == null && this.RelationshipTypeKey == null ||
				this.TargetEntity == null && this.TargetEntityKey == null;
		}

		/// <summary>
		/// Refresh this entity
		/// </summary>
		public override void Refresh()
		{
			base.Refresh();
			this.m_relationshipType = null;
			this.m_targetEntity = null;
		}

		/// <summary>
		/// Determine semantic equality
		/// </summary>
		public override bool SemanticEquals(object obj)
		{
			var other = obj as EntityRelationship;
			if (other == null) return false;
			return base.SemanticEquals(obj) && this.TargetEntityKey == other.TargetEntityKey &&
				this.RelationshipTypeKey == other.RelationshipTypeKey &&
                this.Quantity == other.Quantity;
		}

		/// <summary>
		/// Should serialize inversion indicator?
		/// </summary>
		public bool ShouldSerializeInversionIndicator()
		{
			return this.InversionIndicator;
		}

		/// <summary>
		/// Should serialize quantity?
		/// </summary>
		public bool ShouldSerializeQuantity()
		{
			return this.Quantity.HasValue;
		}
        /// <summary>
        /// Shoudl serialize source entity?
        /// </summary>
        public override bool ShouldSerializeSourceEntityKey()
        {
            return false;
        }

        /// <summary>
        /// Represent as string
        /// </summary>
        public override string ToString()
        {
            return string.Format("({0}) {1} = {2}", this.RelationshipType?.ToString() ?? this.RelationshipTypeKey?.ToString(), this.TargetEntityKey, this.Quantity);
        }
    }
}