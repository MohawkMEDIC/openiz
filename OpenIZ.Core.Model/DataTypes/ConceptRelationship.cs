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
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a relationship between two concepts
    /// </summary>
    [Classifier(nameof(RelationshipType))]
    [XmlType("ConceptRelationship",  Namespace = "http://openiz.org/model"), JsonObject("ConceptRelationship")]
    public class ConceptRelationship : VersionedAssociation<Concept>
    {


        /// <summary>
        /// Gets or sets the target concept identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("targetConcept"), JsonProperty("targetConcept")]
        public Guid? TargetConceptKey
        {
            get { return this.TargetConcept?.Key; }
            set
            {
                if (this.TargetConcept?.Key != value)
                    this.TargetConcept = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the target concept
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(TargetConceptKey))]
		public Concept TargetConcept { get; set; }

        /// <summary>
        /// Relationship type
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("relationshipType"), JsonProperty("relationshipType")]
        public Guid?  RelationshipTypeKey {
            get { return this.RelationshipType?.Key; }
            set
            {
                if (this.RelationshipType?.Key != value)
                    this.RelationshipType = this.EntityProvider?.Get<ConceptRelationshipType>(value);
            }
        }

        /// <summary>
        /// Gets or sets the relationship type
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(RelationshipTypeKey))]
		public ConceptRelationshipType RelationshipType { get; set; }


    }
}