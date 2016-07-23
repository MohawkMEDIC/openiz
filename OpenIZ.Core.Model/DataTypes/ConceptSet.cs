/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{

    /// <summary>
    /// Represents set of concepts
    /// </summary>
    [XmlType("ConceptSet", Namespace = "http://openiz.org/model")]
    [XmlRoot("ConceptSet", Namespace = "http://openiz.org/model")]
    [JsonObject("ConceptSet")]
    [Classifier(nameof(Mnemonic)), KeyLookup(nameof(Mnemonic))]
    public class ConceptSet : BaseEntityData
    {

        /// <summary>
        /// Concept set
        /// </summary>
        public ConceptSet()
        {
            this.Concepts = new List<Concept>();
        }

        /// <summary>
        /// Gets or sets the name of the concept set
        /// </summary>
        [XmlElement("name"), JsonProperty("name")]
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the mnemonic for the concept set (used for convenient lookup)
        /// </summary>
        [XmlElement("mnemonic"), JsonProperty("mnemonic")]
        public String Mnemonic { get; set; }
        /// <summary>
        /// Gets or sets the oid of the concept set
        /// </summary>
        [XmlElement("oid"), JsonProperty("oid")]
        public String Oid { get; set; }
        /// <summary>
        /// Gets or sets the url of the concept set
        /// </summary>
        [XmlElement("url"), JsonProperty("url")]
        public String Url { get; set; }
        
        /// <summary>
        /// Concepts as identifiers for XML purposes only
        /// </summary>
        [XmlElement("concept"), JsonProperty("concept")]
        
        //[Bundle(nameof(Concepts))]
        public List<Guid> ConceptsXml
        {
            get
            {
                if (this.Concepts != null)
                    foreach (var itm in this.Concepts.Where(o => o.Key == null))
                        if (itm.Mnemonic != null)
                            itm.Key = EntitySource.Current.Provider.Query<Concept>(o => o.Mnemonic == itm.Mnemonic).FirstOrDefault()?.Key;
                        else
                            itm.Key = Guid.NewGuid();

                return this.Concepts?.Select(o => o.Key.Value).ToList();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets the concepts in the set
        /// </summary>
        
        [AutoLoad]
        [XmlIgnore, JsonIgnore]
        public List<Concept> Concepts { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion reason
        /// </summary>
        [XmlElement("obsoletionReason")]
        public string ObsoletionReason { get; set; }
    }
}
