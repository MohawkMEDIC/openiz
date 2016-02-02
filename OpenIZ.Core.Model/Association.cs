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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Interfaces;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents a bse class for bound relational data
    /// </summary>
    /// <typeparam name="TSourceType"></typeparam>
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class Association<TSourceType> : IdentifiedData, ISimpleAssociation where TSourceType : IdentifiedData
    {

        // Target entity key
        private Guid m_sourceEntityKey;
        // The target entity
        
        private TSourceType m_sourceEntity;

        /// <summary>
        /// Gets or sets the source entity's key (where the relationship is FROM)
        /// </summary>
        [XmlElement("source"), JsonProperty("source")]
        public virtual Guid SourceEntityKey
        {
            get
            {
                return this.m_sourceEntityKey;
            }
            set
            {
                this.m_sourceEntityKey = value;
                this.m_sourceEntity = null;
            }
        }

        /// <summary>
        /// The entity that this relationship targets
        /// </summary>
        [DelayLoad(nameof(SourceEntityKey))]
        [XmlIgnore, JsonIgnore]
        public TSourceType SourceEntity
        {
            get
            {
                this.m_sourceEntity = this.DelayLoad(this.m_sourceEntityKey, this.m_sourceEntity);
                return this.m_sourceEntity;
            }
            set
            {
                this.m_sourceEntity = value;
                if (value == null)
                    this.m_sourceEntityKey = default(Guid);
                else
                    this.m_sourceEntityKey = value.Key;
            }
        }

        /// <summary>
        /// Force delay load properties to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_sourceEntity = null;
        }
    }
}
