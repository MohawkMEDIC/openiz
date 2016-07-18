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
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an association between two entities
    /// </summary>
    [Classifier(nameof(RelationshipType)), SimpleValue(nameof(TargetEntityKey))]
    [XmlType("EntityRelationship",  Namespace = "http://openiz.org/model"), JsonObject("EntityRelationship")]
    public class EntityRelationship : VersionedAssociation<Entity>
    {


        /// <summary>
        /// Default constructor for entity relationship
        /// </summary>
        public EntityRelationship()
        {
        }

        /// <summary>
        /// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        public EntityRelationship(Guid relationshipType, Entity target)
        {
            this.RelationshipTypeKey = relationshipType;
            this.TargetEntity = target;
        }

        /// <summary>
        /// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        public EntityRelationship(Guid? relationshipType, Guid? targetId)
        {
            this.RelationshipTypeKey = relationshipType;
            this.m_targetEntityKey = targetId;
        }

        /// <summary>
        /// Raw target entity key
        /// </summary>
        private Guid? m_targetEntityKey;

        /// <summary>
        /// The target of the association
        /// </summary>
        [ XmlElement("target"), JsonProperty("target")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? TargetEntityKey
        {
            get { return this.m_targetEntityKey ?? this.TargetEntity?.Key; }
            set {
                if (this.TargetEntity?.Key != value)
                    this.TargetEntity = this.EntityProvider?.Get<Entity>(value);
                if (this.m_targetEntityKey != value)
                    this.m_targetEntityKey = value;
            }
        }

        /// <summary>
        /// Target entity reference
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore, SerializationReference(nameof(TargetEntityKey))]
		public Entity TargetEntity { get; set; }

        /// <summary>
        /// Association type key
        /// </summary>
        [DataIgnore, XmlElement("relationshipType"), JsonProperty("relationshipType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? RelationshipTypeKey
        {
            get { return this.RelationshipType?.Key; }
            set
            {
                if (this.RelationshipType?.Key != value)
                    this.RelationshipType = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// The inversion indicator
        /// </summary>
        [XmlElement("inversionInd"), JsonProperty("inversionInd")]
        public bool InversionIndicator { get; set; }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [AutoLoad]
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(RelationshipTypeKey))]
		public Concept RelationshipType { get; set; }


    }
}