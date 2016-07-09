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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using System.ComponentModel;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents versioned based data, that is base data which has versions
    /// </summary>
    
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class VersionedEntityData<THistoryModelType> : BaseEntityData, IVersionedEntity where THistoryModelType : VersionedEntityData<THistoryModelType>
    {

        // Previous version id
        private Guid? m_previousVersionId;
        // Previous version
        
        private THistoryModelType m_previousVersion;

        /// <summary>
        /// Creates a new versioned base data class
        /// </summary>
        public VersionedEntityData()
        {
        }

        /// <summary>
        /// Previous version
        /// </summary>
        IVersionedEntity IVersionedEntity.PreviousVersion
        {
            get
            {
                return this.PreviousVersion;
            }
        }

        /// <summary>
        /// Gets or sets the previous version key
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("previousVersion"), JsonProperty("previousVersion")]
        public virtual Guid? PreviousVersionKey
        {
            get
            {
                return this.m_previousVersionId;
            }
            set
            {
                this.m_previousVersionId = value;
                this.m_previousVersion = default(THistoryModelType);
            }
        }

        /// <summary>
        /// Gets or sets the previous version
        /// </summary>
        [DelayLoad(nameof(PreviousVersionKey))]
        [XmlIgnore, JsonIgnore]
        public virtual THistoryModelType PreviousVersion
        {
            get
            {
                if(this.m_previousVersion == null && this.IsDelayLoadEnabled && 
                    this.m_previousVersionId.HasValue)
                    this.m_previousVersion = EntitySource.Current.Get(this.Key, this.m_previousVersionId.Value, this.m_previousVersion);
                return this.m_previousVersion;
            }
            set
            {
                this.m_previousVersion = value;
                if (value == default(THistoryModelType))
                    this.m_previousVersionId = null;
                else
                    this.m_previousVersionId = value.VersionKey;
            }
        }

        /// <summary>
        /// Gets or sets the key which represents the version of the entity
        /// </summary>
        [XmlElement("version"), JsonProperty("version")]
        public Guid? VersionKey { get; set; }

        /// <summary>
        /// The sequence number of the version (for ordering)
        /// </summary>
        [XmlElement("sequence"), JsonProperty("sequence")]
        public Decimal? VersionSequence { get; set; }

        /// <summary>
        /// Represent the versioned data as a string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (K:{1}, V:{2})", this.GetType().Name, this.Key, this.VersionKey);
        }

        /// <summary>
        /// Force bound attributes to reload
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_previousVersion = default(THistoryModelType);
        }
    }

}
