/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
    [XmlType(Namespace = "http://openiz.org/model"), JsonObject("Association")]
    public abstract class Association<TSourceType> : IdentifiedData, ISimpleAssociation where TSourceType : IdentifiedData, new()
    {

        // Target entity key
        private Guid? m_sourceEntityKey;
        // The target entity
        private TSourceType m_sourceEntity;

        /// <summary>
        /// Get the modification date
        /// </summary>
        public override DateTimeOffset ModifiedOn
        {
            get
            {
                if(this.m_sourceEntity != null)
                    return this.m_sourceEntity.ModifiedOn;
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Gets or sets the source entity's key (where the relationship is FROM)
        /// </summary>
        [XmlElement("source"), JsonProperty("source")]
        public virtual Guid? SourceEntityKey
        {
            get
            {
                return this.m_sourceEntityKey;
            }
            set
            {
                if (value != this.m_sourceEntity?.Key)
                {
                    this.m_sourceEntityKey = value;
                    this.m_sourceEntity = null;
                }
            }
        }

        /// <summary>
        /// The entity that this relationship targets
        /// </summary>
        [SerializationReference(nameof(SourceEntityKey))]
        [XmlIgnore, JsonIgnore, DataIgnore]
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
                this.m_sourceEntityKey = value?.Key;
            }
        }


        /// <summary>
        /// Should serialize obsolete
        /// </summary>
        public virtual bool ShouldSerializeSourceEntityKey()
        {
            return this.m_sourceEntityKey.HasValue;
        }


        /// <summary>
        /// Force delay load properties to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_sourceEntity = null;
        }

        /// <summary>
        /// Determines equality of this association
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as Association<TSourceType>;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.SourceEntityKey == other.SourceEntityKey;
        }
    }
}
