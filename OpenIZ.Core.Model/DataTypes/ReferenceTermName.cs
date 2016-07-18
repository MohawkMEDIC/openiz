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
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Display name of a code system or reference term
    /// </summary>
    [XmlType("ReferenceTermName",  Namespace = "http://openiz.org/model"), JsonObject("ReferenceTermName")]
    [Classifier(nameof(Language)), SimpleValue(nameof(Name))]
    public class ReferenceTermName : BaseEntityData
    {

        /// <summary>
        /// Back-reference to reference term
        /// </summary>
        [XmlIgnore, JsonIgnore]
		public Guid  ReferenceTermKey { get; set; }

        /// <summary>
        /// Gets or sets the language code of the object
        /// </summary>
        [XmlElement("language"), JsonProperty("language")]
        public String Language { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference term
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the phonetic code of the reference term
        /// </summary>
        [XmlElement("phoneticCode"), JsonProperty("phoneticCode")]
        public String PhoneticCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the phonetic code
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("phoneticAlgorithm"), JsonProperty("phoneticAlgorithm")]
        public Guid?  PhoneticAlgorithmKey
        {
            get { return this.PhoneticAlgorithm?.Key; }
            set
            {
                if (this.PhoneticAlgorithm?.Key != value)
                    this.PhoneticAlgorithm = this.EntityProvider?.Get<PhoneticAlgorithm>(value);
            }
        }

        /// <summary>
        /// Gets or sets the phonetic algorithm
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(PhoneticAlgorithmKey))]
		public PhoneticAlgorithm PhoneticAlgorithm { get; set; }

    }
}