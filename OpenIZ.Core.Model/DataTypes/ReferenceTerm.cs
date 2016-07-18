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
using System.Linq;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic reference term
    /// </summary>
    [Classifier(nameof(CodeSystem))]
    [XmlType("ReferenceTerm",  Namespace = "http://openiz.org/model"), JsonObject("ReferenceTerm")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "ReferenceTerm")]
    public class ReferenceTerm : NonVersionedEntityData
    {

        /// <summary>
        /// Gets or sets the mnemonic for the reference term
        /// </summary>
        [XmlElement("mnemonic"), JsonProperty("mnemonic")]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the code system 
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore, SerializationReference(nameof(CodeSystemKey))]
		public CodeSystem CodeSystem { get; set; }
        
        /// <summary>
        /// Gets or sets the code system identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("codeSystem"), JsonProperty("codeSystem")]
        public Guid?  CodeSystemKey {
            get { return this.CodeSystem?.Key; }
            set
            {
                if (this.CodeSystem?.Key != value)
                    this.CodeSystem = this.EntityProvider.Get<CodeSystem>(value);
            }
        }

        /// <summary>
        /// Gets display names associated with the reference term
        /// </summary>
        [AutoLoad]
        [XmlElement("name"), JsonProperty("name")]
        public List<ReferenceTermName> DisplayNames { get; set; }


    }
}