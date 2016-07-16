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
    /// A generic class representing components of a larger item (i.e. address, name, etc);
    /// </summary>
    /// <typeparam name="TBoundModel"></typeparam>
    [Classifier(nameof(ComponentType)), SimpleValue(nameof(Value))]
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class GenericComponentValues<TBoundModel> : Association<TBoundModel> where TBoundModel : IdentifiedData, new()
    {
       
        /// <summary>
        /// Default ctor
        /// </summary>
        public GenericComponentValues()
        {

        }

        /// <summary>
        /// Creates a generic component value with the specified classifier
        /// </summary>
        public GenericComponentValues(Guid partType, String value)
        {
            this.ComponentTypeKey = partType;
            this.Value = value;
        }

        /// <summary>
        /// Constructor with the specified identifier
        /// </summary>
        public GenericComponentValues(String value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Component type key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("type"), JsonProperty("type")]
        public Guid? ComponentTypeKey
        {
            get { return this.ComponentType?.Key; }
            set
            {
                if (this.ComponentType?.Key != value)
                    this.ComponentType = this.EntityProvider.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the type of address component
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ComponentTypeKey))]
		public Concept ComponentType { get; set; }


        /// <summary>
        /// Gets or sets the value of the name component
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }


    }
}