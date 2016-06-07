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
    [Classifier(nameof(Mnemonic))]
    public class ConceptSet : BaseEntityData
    {

        // Set members
        private List<Concept> m_setMembers;

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
        [DelayLoad(null)]
        //[Bundle(nameof(Concepts))]
        public List<Guid> ConceptSetsXml
        {
            get
            {
                return this.Concepts?.Select(o => o.Key).ToList();
            }
            set
            {
                ; // nothing
            }
        }

        /// <summary>
        /// Gets the concepts in the set
        /// </summary>
        [DelayLoad(null)]
        [XmlIgnore, JsonIgnore]
        public List<Concept> Concepts
        {
            get {
                if(this.IsDelayLoadEnabled && this.m_setMembers == null)
                    this.m_setMembers = EntitySource.Current.Provider.Query<Concept>(o=>o.ConceptSets.Any(s=>s.Key == this.Key)).ToList();
                return this.m_setMembers;
            }
        }

        /// <summary>
        /// Gets or sets the obsoletion reason
        /// </summary>
        [XmlElement("obsoletionReason")]
        public string ObsoletionReason { get; set; }
    }
}
