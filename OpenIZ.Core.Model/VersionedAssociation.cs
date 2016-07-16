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
    public abstract class VersionedAssociation<TSourceType> : Association<TSourceType>, IVersionedAssociation where TSourceType : VersionedEntityData<TSourceType>, new()
    {

       
        /// <summary>
        /// Gets or sets the effective version of this type
        /// </summary>
        [XmlElement("effectiveVersionSequence"), JsonProperty("effectiveVersionSequence")]
        public Decimal? EffectiveVersionSequenceId { get; set; }

        /// <summary>
        /// Gets or sets the obsoleted version identifier
        /// </summary>
        [XmlElement("obsoleteVersionSequence"), JsonProperty("obsoleteVersionSequence")]
        public Decimal? ObsoleteVersionSequenceId { get; set; }
        
        /// <summary>
        /// Shoudl seralize
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeObsoleteVersionSequenceId()
        {
            return this.ObsoleteVersionSequenceId.HasValue;
        }
        /// <summary>
        /// Should serialize
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeEffectiveVersionSequenceId()
        {
            return this.EffectiveVersionSequenceId.HasValue;
        }

    }
}
