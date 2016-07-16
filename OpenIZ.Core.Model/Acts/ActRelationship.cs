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
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Act relationships
    /// </summary>
    
    [XmlType("ActRelationship",  Namespace = "http://openiz.org/model"), JsonObject("ActRelationship")]
    public class ActRelationship : VersionedAssociation<Act>
    {


        /// <summary>
        /// The target of the association
        /// </summary>
        [DataIgnore, XmlElement("target"), JsonProperty("target")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? TargetActKey
        {
            get { return this.TargetAct?.Key; }
            set
            {
                if (this.TargetAct?.Key != value)
                    this.TargetAct = this.EntityProvider.Get<Act>(value);
            }
        }

        /// <summary>
        /// Target act reference
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(TargetActKey))]
		public Act TargetAct { get; set; }

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
                    this.RelationshipType = this.EntityProvider.Get<Concept>(value);

            }
        }

        /// <summary>
        /// Gets or sets the association type
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(RelationshipTypeKey))]
		public Concept RelationshipType { get; set; }


    }
}
