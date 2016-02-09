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
 * Date: 2016-1-24
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Linq;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a name for an entity
    /// </summary>
    
    [XmlType("EntityName",  Namespace = "http://openiz.org/model"), JsonObject("EntityName")]
    public class EntityName : VersionedAssociation<Entity>
    {
        // Name use key
        private Guid? m_nameUseKey;
        // Name use concept
        
        private Concept m_nameUseConcept;
        // Name components
        
        private List<EntityNameComponent> m_nameComponents;

        /// <summary>
        /// Gets or sets the name use key
        /// </summary>
        [XmlElement("nameUse"), JsonProperty("nameUse")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? NameUseKey
        {
            get { return this.m_nameUseKey; }
            set
            {
                this.m_nameUseKey = value;
                this.m_nameUseConcept = null;
            }
        }

        /// <summary>
        /// Gets or sets the name use
        /// </summary>
        [DelayLoad(nameof(NameUseKey))]
        [XmlIgnore, JsonIgnore]
        public Concept NameUse
        {
            get {
                this.m_nameUseConcept = base.DelayLoad(this.m_nameUseKey, this.m_nameUseConcept);
                return this.m_nameUseConcept;
            }
            set
            {
                this.m_nameUseConcept = value;
                this.m_nameUseKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the component types
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("component"), JsonProperty("component")]
        public List<EntityNameComponent> Component
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_nameComponents = EntitySource.Current.GetRelations(this.Key, this.m_nameComponents);
                return this.m_nameComponents;
            }
        }

        /// <summary>
        /// Refreshes the underlying content
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_nameComponents = null;
            this.m_nameUseKey = null;
        }
    }
}