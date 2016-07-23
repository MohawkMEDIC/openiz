/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Act relationships
    /// </summary>
    
    [XmlType("ActRelationship",  Namespace = "http://openiz.org/model"), JsonObject("ActRelationship")]
    public class ActRelationship : VersionedAssociation<Act>
    {
        // The entity key
        private Guid? m_targetActKey;
        // The target entity
        
        private Act m_targetAct;
        // The association type key
        private Guid? m_relationshipTypeKey;
        // The association type
        
        private Concept m_relationshipType;

        /// <summary>
        /// Default constructor for entity relationship
        /// </summary>
        public ActRelationship()
        {
        }

        /// <summary>
        /// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        public ActRelationship(Guid? relationshipType, Act target)
        {
            this.RelationshipTypeKey = relationshipType;
            this.TargetAct = target;
        }

        /// <summary>
        /// Entity relationship between <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        public ActRelationship(Guid? relationshipType, Guid? targetKey)
        {
            this.RelationshipTypeKey = relationshipType;
            this.TargetActKey = targetKey;
        }

        /// <summary>
        /// The target of the association
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? TargetActKey
        {
            get { return this.m_targetActKey; }
            set
            {
                this.m_targetActKey = value;
                this.m_targetAct = null;
            }
        }

        /// <summary>
        /// Target act reference
        /// </summary>
        [SerializationReference(nameof(TargetActKey))]
        [XmlIgnore, JsonIgnore]
        public Act TargetAct
        {
            get
            {
                this.m_targetAct = base.DelayLoad(this.m_targetActKey, this.m_targetAct);
                return this.m_targetAct;
            }
            set
            {
                this.m_targetAct = value;
                this.m_targetActKey = value?.Key;
            }
        }

        /// <summary>
        /// Association type key
        /// </summary>
        [XmlElement("relationshipType"), JsonProperty("relationshipType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? RelationshipTypeKey
        {
            get { return this.m_relationshipTypeKey; }
            set
            {
                this.m_relationshipTypeKey = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(RelationshipTypeKey))]
        public Concept RelationshipType
        {
            get
            {
                this.m_relationshipType = base.DelayLoad(this.m_relationshipTypeKey, this.m_relationshipType);
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                if (value == null)
                    this.m_relationshipTypeKey = Guid.Empty;
                else
                    this.m_relationshipTypeKey = value.Key;
            }
        }

        /// <summary>
        /// Refreshes the model to force reload from underlying model
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_relationshipType = null;
            this.m_targetAct = null;
        }
    }
}
