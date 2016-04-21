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
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an association between two entities
    /// </summary>
    [Classifier(nameof(RelationshipType))]
    [XmlType("EntityRelationship",  Namespace = "http://openiz.org/model"), JsonObject("EntityRelationship")]
    public class EntityRelationship : VersionedAssociation<Entity>
    {

        // The entity key
        private Guid m_targetEntityKey;
        // The target entity
        
        private Entity m_targetEntity;
        // The association type key
        private Guid m_associationTypeKey;
        // The association type
        
        private Concept m_relationshipType;

        /// <summary>
        /// The target of the association
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid TargetEntityKey
        {
            get { return this.m_targetEntityKey; }
            set { this.m_targetEntityKey = value;
                this.m_targetEntity = null;
            }
        }

        /// <summary>
        /// Target entity reference
        /// </summary>
        [DelayLoad(nameof(TargetEntityKey))]
        [XmlIgnore, JsonIgnore]
        public Entity TargetEntity
        {
            get {
                this.m_targetEntity = base.DelayLoad(this.m_targetEntityKey, this.m_targetEntity);
                return this.m_targetEntity;
            }
            set
            {
                this.m_targetEntity = value;
                if (value == null)
                    this.m_targetEntityKey = Guid.Empty;
                else
                    this.m_targetEntityKey = value.Key;
            }
        }

        /// <summary>
        /// Association type key
        /// </summary>
        [XmlElement("relationshipType"), JsonProperty("relationshipType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid RelationshipTypeKey
        {
            get { return this.m_associationTypeKey; }
            set
            {
                this.m_associationTypeKey = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [DelayLoad(nameof(RelationshipTypeKey))]
        public Concept RelationshipType
        {
            get {
                this.m_relationshipType = base.DelayLoad(this.m_associationTypeKey, this.m_relationshipType);
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                if (value == null)
                    this.m_associationTypeKey = Guid.Empty;
                else
                    this.m_associationTypeKey = value.Key;
            }
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
    }
}