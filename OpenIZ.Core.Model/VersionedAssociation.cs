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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Security;

using OpenIZ.Core.Model.Attributes;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Represents a relational class which is bound on a version boundary
    /// </summary>
    
    [XmlType(Namespace = "http://openiz.org/model")]
    public abstract class VersionedAssociation<TSourceType> : Association<TSourceType>, IVersionedAssociation where TSourceType : VersionedEntityData<TSourceType>
    {

        // The identifier of the version where this data is effective
        private Decimal m_effectiveVersionSequenceId;
        // The identifier of the version where this data is no longer effective
        private Decimal? m_obsoleteVersionSequenceId;
        // The version where this data is effective
        
        private TSourceType m_effectiveVersion;
        // The version where this data is obsolete
        
        private TSourceType m_obsoleteVersion;
       
        /// <summary>
        /// Gets or sets the effective version of this type
        /// </summary>
        [XmlElement("effectiveVersionSequence"), JsonProperty("effectiveVersionSequence")]
        public Decimal EffectiveVersionSequenceId
        {
            get { return this.m_effectiveVersionSequenceId; }
            set
            {
                this.m_effectiveVersionSequenceId = value;
                this.m_effectiveVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted version identifier
        /// </summary>
        [XmlElement("obsoleteVersionSequence"), JsonProperty("obsoleteVersionSequence")]
        public Decimal? ObsoleteVersionSequenceId
        {
            get { return this.m_obsoleteVersionSequenceId; }
            set
            {
                this.m_obsoleteVersionSequenceId = value;
                this.m_obsoleteVersion = null;
            }
        }

        /// <summary>
        /// Gets or sets the effective version
        /// </summary>
        [DelayLoad(nameof(EffectiveVersionSequenceId))]
        [XmlIgnore, JsonIgnore]
        public TSourceType EffectiveVersion
        {
            get
            {
                if(this.m_effectiveVersion == null &&
                    this.IsDelayLoadEnabled &&
                    this.m_effectiveVersionSequenceId != default(Decimal))
                    this.m_effectiveVersion = EntitySource.Current.Provider.Query<TSourceType>(t => t.VersionSequence == this.m_effectiveVersionSequenceId).FirstOrDefault();
                return this.m_effectiveVersion;
            }
            set
            {
                this.m_effectiveVersion = value;
                if (value == null)
                    this.m_effectiveVersionSequenceId = default(Decimal);
                else
                    this.m_effectiveVersionSequenceId = value.VersionSequence;
            }
        }

        /// <summary>
        /// Gets the obsoletion version
        /// </summary>
        [DelayLoad(nameof(ObsoleteVersionSequenceId))]
        [XmlIgnore, JsonIgnore]
        public TSourceType ObsoleteVersion
        {
            get
            {
                if(this.m_obsoleteVersion == null &&
                    this.IsDelayLoadEnabled &&
                    this.m_obsoleteVersionSequenceId.HasValue)
                    this.m_obsoleteVersion = EntitySource.Current.Provider.Query<TSourceType>(t => t.VersionSequence == this.m_obsoleteVersionSequenceId).FirstOrDefault();
                return this.m_obsoleteVersion;
            }
            set
            {
                this.m_obsoleteVersion = value;
                if (value == null)
                    this.m_obsoleteVersionSequenceId = null;
                else
                    this.m_obsoleteVersionSequenceId = value.VersionSequence;
            }
        }

        /// <summary>
        /// Gets or sets the user that created this relationship
        /// </summary>
        [DelayLoad(nameof(CreatedByKey))]
        [XmlIgnore, JsonIgnore]
        public override SecurityUser CreatedBy
        {
            get
            {
                return this.EffectiveVersion?.CreatedBy;
            }
        }

        /// <summary>
        /// Gets the identifier of the user that created this relationship
        /// </summary>
        [DelayLoad(null)]
        [XmlIgnore, JsonIgnore]
        public override Guid CreatedByKey
        {
            get
            {
                if (this.EffectiveVersion == null)
                    return Guid.Empty;
                else
                    return this.EffectiveVersion.CreatedByKey;
            }
            set
            {
                throw new NotSupportedException("CreatedById is based on EffectiveVersion property");
            }
        }

        /// <summary>
        /// Obsoleted by
        /// </summary>
        [DelayLoad(nameof(ObsoletedByKey))]
        [XmlIgnore, JsonIgnore]
        public override SecurityUser ObsoletedBy
        {
            get
            {
                return this.ObsoleteVersion?.CreatedBy;
            }
        }

        /// <summary>
        /// Gets the identifier of the user that obsoleted the relationship
        /// </summary>
        [DelayLoad(null)]
        [XmlIgnore, JsonIgnore]
        public override Guid? ObsoletedByKey
        {
            get
            {
                    return this.ObsoleteVersion?.CreatedByKey;
                
            }
            set
            {
                throw new NotSupportedException("ObsoletedById is based on EffectiveVersion property");
            }
        }

        /// <summary>
        /// Refresh
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_effectiveVersion = this.m_obsoleteVersion = null;
        }
    }
}
