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
        private Guid m_targetActKey;
        // The target entity
        
        private Act m_targetAct;
        // The association type key
        private Guid m_relationshipTypeKey;
        // The association type
        
        private Concept m_relationshipType;

        /// <summary>
        /// The target of the association
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid TargetActKey
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
        [DelayLoad(nameof(TargetActKey))]
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
                if (value == null)
                    this.m_targetActKey = Guid.Empty;
                else
                    this.m_targetActKey = value.Key;
            }
        }

        /// <summary>
        /// Association type key
        /// </summary>
        [XmlElement("relationshipType"), JsonProperty("relationshipType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid RelationshipTypeKey
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
        [DelayLoad(nameof(RelationshipTypeKey))]
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
