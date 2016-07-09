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
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Interfaces;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a base entity extension
    /// </summary>
    [Classifier(nameof(ExtensionType)), SimpleValue(nameof(ExtensionValue))]
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class Extension<TBoundModel> : VersionedAssociation<TBoundModel> where TBoundModel : VersionedEntityData<TBoundModel>
    {

        // Extension type key
        private Guid? m_extensionTypeKey;
        // Extension type
        private ExtensionType m_extensionType;
        // Extension handler
        private IExtensionHandler m_extensionHandler;

        /// <summary>
        /// Gets or sets the value of the extension
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public byte[] ExtensionValue { get; set; }

        /// <summary>
        /// Gets or sets an extension displayable value
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String ExtensionDisplay { get; set; }

        /// <summary>
        /// Gets or sets the extension type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore, JsonIgnore]
        public Guid? ExtensionTypeKey
        {
            get { return this.m_extensionTypeKey; }
            set
            {
                this.m_extensionTypeKey = value;
                this.m_extensionType = null;
            }
        }

        /// <summary>
        /// Gets or sets the extension type
        /// </summary>
        [DelayLoad(nameof(ExtensionTypeKey))]
        [XmlElement("extensionType"), JsonProperty("extensionType")]
        [AutoLoad]
        public ExtensionType ExtensionType
        {
            get {
                this.m_extensionType = base.DelayLoad(this.m_extensionTypeKey, this.m_extensionType);
                return this.m_extensionType;
            }
            set
            {
                this.m_extensionType = value;
                this.m_extensionTypeKey = value?.Key;
            }
        }

        /// <summary>
        /// Forces refresh 
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_extensionType = null;
        }
    }

    /// <summary>
    /// Extension bound to entity
    /// </summary>
    
    [XmlType("EntityExtension",  Namespace = "http://openiz.org/model"), JsonObject("EntityExtension")]
    public class EntityExtension : Extension<Entity>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityExtension()
        {

        }

        /// <summary>
        /// Creates an entity extension
        /// </summary>
        public EntityExtension(Guid extensionType, byte[] value)
        {
            this.ExtensionTypeKey = extensionType;
            this.ExtensionValue = value;
        }
        
    }

    /// <summary>
    /// Act extension
    /// </summary>
    
    [XmlType("ActExtension",  Namespace = "http://openiz.org/model"), JsonObject("ActExtension")]
    public class ActExtension : Extension<Act>
    {

    }
}
