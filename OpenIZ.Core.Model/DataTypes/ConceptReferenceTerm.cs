/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a reference term relationship between a concept and reference term
    /// </summary>
    [Classifier(nameof(ReferenceTerm))]
    [XmlType("ConceptReferenceTerm",  Namespace = "http://openiz.org/model"), JsonObject("ConceptReferenceTerm")]
    public class ConceptReferenceTerm : VersionedAssociation<Concept>
    {
        // Reference term id
        private Guid? m_referenceTermId;
        // Reference term
        
        private ReferenceTerm m_referenceTerm;
        // ConceptRelationship type
        private Guid? m_relationshipTypeId;
        // Relationship type
        
        private ConceptRelationshipType m_relationshipType;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptReferenceTerm"/> class.
		/// </summary>
		public ConceptReferenceTerm()
	    {
		    
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConceptReferenceTerm"/> class.
		/// </summary>
		/// <param name="referenceTermKey">The reference term identifier.</param>
		/// <param name="relationshipTypeKey">The relationship type identifier.</param>
		public ConceptReferenceTerm(Guid? referenceTermKey, Guid? relationshipTypeKey)
		{
			this.RelationshipTypeKey = relationshipTypeKey;
			this.ReferenceTermKey = referenceTermKey;
		}

        /// <summary>
        /// Gets or sets the reference term identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        [XmlElement("term"), JsonProperty("term")]
        public Guid?  ReferenceTermKey {
            get { return this.m_referenceTermId; }
            set
            {
                this.m_referenceTerm = null;
                this.m_referenceTermId = value;
            }
        }

        /// <summary>
        /// Gets or set the reference term
        /// </summary>
        [SerializationReference(nameof(ReferenceTermKey))]
        [XmlIgnore, JsonIgnore]
        public ReferenceTerm ReferenceTerm
        {
            get
            {
                this.m_referenceTerm = base.DelayLoad(this.m_referenceTermId, this.m_referenceTerm);
                return this.m_referenceTerm;
            }
            set
            {
                this.m_referenceTerm = value;
                this.m_referenceTermId = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the relationship type identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("relationshipType"), JsonProperty("relationshipType")]
        public Guid?  RelationshipTypeKey {
            get { return this.m_relationshipTypeId; }
            set
            {
                this.m_relationshipTypeId = value;
                this.m_relationshipType = null;
            }
        }

        /// <summary>
        /// Gets or sets the relationship type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(RelationshipTypeKey))]
        public ConceptRelationshipType RelationshipType {
            get
            {
                this.m_relationshipType = base.DelayLoad(this.m_relationshipTypeId, this.m_relationshipType);
                return this.m_relationshipType;
            }
            set
            {
                this.m_relationshipType = value;
                this.m_relationshipTypeId = value?.Key;
            }
        }

        /// <summary>
        /// Refresh the specified object
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_referenceTerm = null;
            this.m_relationshipType = null;
        }
    }
}