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
using OpenIZ.Core.Model.Interfaces;

namespace OpenIZ.Core.Model.DataTypes
{

    /// <summary>
    /// Represents a base entity extension
    /// </summary>
    [Classifier(nameof(ExtensionType)), SimpleValue(nameof(ExtensionValueString))]
    [XmlType(Namespace = "http://openiz.org/model"), JsonObject("Extension")]
    public abstract class Extension<TBoundModel> : 
        VersionedAssociation<TBoundModel>, IModelExtension where TBoundModel : VersionedEntityData<TBoundModel>, new()
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
        public byte[] ExtensionValueXml { get; set; }

        /// <summary>
        /// Value as string of bytes
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ExtensionValueXml))]
        public string ExtensionValueString
        {
            get
            {
                if (this.ExtensionValueXml == null) return null;
                try
                {
                    return BitConverter.ToString(this.ExtensionValueXml).Replace("-", "");
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value == null) this.ExtensionValueXml = null;
                try
                {
                    if (value.Length % 2 == 1) value = "0" + value;
                    this.ExtensionValueXml = Enumerable.Range(0, value.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(value.Substring(x, 2), 16)).ToArray();
                }
                catch
                {
                    this.ExtensionValueXml = Encoding.UTF8.GetBytes(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the ignore value
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Object ExtensionValue
        {
            get
            {
                return this.ExtensionType?.ExtensionHandlerInstance?.DeSerialize(this.ExtensionValueXml);
            }
            set
            {
                if (this.ExtensionType?.ExtensionHandlerInstance != null)
                    this.ExtensionValueXml = this.ExtensionType?.ExtensionHandlerInstance?.Serialize(value);
            }
        }

        /// <summary>
        /// Get the value of the extension
        /// </summary>
        /// <returns></returns>
        public Object GetValue()
        {
            return this.LoadProperty<ExtensionType>("ExtensionType")?.ExtensionHandlerInstance?.DeSerialize(this.ExtensionValueXml);
        }

        /// <summary>
        /// Gets or sets an extension displayable value
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String ExtensionDisplay
        {
            get
            {
                this.ExtensionType = this.LoadProperty<ExtensionType>(nameof(ExtensionType));
                return this.ExtensionType?.ExtensionHandlerInstance?.GetDisplay(this.ExtensionValue);
            }
            set { }
        }
         
        /// <summary>
        /// Gets or sets the extension type
        /// </summary>
        [SerializationReference(nameof(ExtensionTypeKey))]
        [XmlIgnore, JsonIgnore]
        [AutoLoad]
        public ExtensionType ExtensionType
        {
            get
            {
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
        /// Gets or sets the extension type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("extensionType"), JsonProperty("extensionType")]
        public Guid? ExtensionTypeKey
        {
            get { return this.m_extensionTypeKey; }
            set
            {
                if (this.m_extensionTypeKey != value)
                {
                    this.m_extensionTypeKey = value;
                    this.m_extensionType = null;
                }
            }
        }

        /// <summary>
        /// Get the type key
        /// </summary>
        Guid IModelExtension.ExtensionTypeKey
        {
            get
            {
                return this.ExtensionTypeKey.Value;
            }
        }

        /// <summary>
        /// Gets the data
        /// </summary>
        byte[] IModelExtension.Data
        {
            get
            {
                return this.ExtensionValueXml;
            }
        }

        /// <summary>
        /// Get the display
        /// </summary>
        string IModelExtension.Display
        {
            get
            {
                return this.ExtensionDisplay;
            }
        }
        
        /// <summary>
        /// Get the value of the extension
        /// </summary>
        object IModelExtension.Value
        {
            get
            {
                return this.ExtensionValue;
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

        /// <summary>
        /// Determine equality
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            Extension<TBoundModel> other = obj as Extension<TBoundModel>;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.ExtensionTypeKey == this.ExtensionTypeKey &&
                this.ExtensionValueString == other.ExtensionValueString;
        }
    }

    /// <summary>
    /// Extension bound to entity
    /// </summary>

    [XmlType("EntityExtension", Namespace = "http://openiz.org/model"), JsonObject("EntityExtension")]
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
            this.ExtensionValueXml = value;
        }

        /// <summary>
        /// Creates an entity extension
        /// </summary>
        public EntityExtension(Guid extensionType, Type extensionHandlerType, object value)
        {
            this.ExtensionTypeKey = extensionType;
            this.ExtensionValueXml = (Activator.CreateInstance(extensionHandlerType) as IExtensionHandler)?.Serialize(value);
        }

    }

    /// <summary>
    /// Act extension
    /// </summary>

    [XmlType("ActExtension", Namespace = "http://openiz.org/model"), JsonObject("ActExtension")]
    public class ActExtension : Extension<Act>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public ActExtension()
        {

        }

        /// <summary>
        /// Creates an entity extension
        /// </summary>
        public ActExtension(Guid extensionType, byte[] value)
        {
            this.ExtensionTypeKey = extensionType;
            this.ExtensionValueXml = value;
        }

        /// <summary>
        /// Creates an entity extension
        /// </summary>
        public ActExtension(Guid extensionType, Type extensionHandlerType, object value)
        {
            this.ExtensionTypeKey = extensionType;
            this.ExtensionValueXml = (Activator.CreateInstance(extensionHandlerType) as IExtensionHandler)?.Serialize(value);
        }
    }
}
