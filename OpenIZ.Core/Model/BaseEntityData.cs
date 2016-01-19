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
 * Date: 2016-1-19
 */
using MARC.Everest.Connectors;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Data;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model
{

    /// <summary>
    /// Represents the root of all model classes in the OpenIZ Core
    /// </summary>
    [Serializable]
    [DataContract(Name = "BaseEntityData", Namespace = "http://openiz.org/model")]
    public abstract class BaseEntityData : IdentifiedData
    {

        // Created by identifier
        private Guid m_createdById;
        // Created by
        [NonSerialized]
        private SecurityUser m_createdBy;
        // Obsoleted by
        private Guid? m_obsoletedById;
        // Obsoleted by user
        [NonSerialized]
        private SecurityUser m_obsoletedBy;
        
        /// <summary>
        /// Creation Time
        /// </summary>
        [DataMember(Name = "creationTime")]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Obsoletion time
        /// </summary>
        [DataMember(Name = "obsoletionTime")]
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the user that created this base data
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public virtual SecurityUser CreatedBy {
            get
            {
                return base.DelayLoad(this.m_createdById, this.m_createdBy);
            }
         }

        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public virtual SecurityUser ObsoletedBy {
            get
            {
                return base.DelayLoad(this.m_obsoletedById, this.m_obsoletedBy);
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name ="createdBy")]
        public virtual Guid CreatedById
        {
            get { return this.m_createdById; }
            set
            {
                if (this.m_createdById != value)
                    this.m_createdBy = null;
                this.m_createdById = value;
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted by identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name ="obsoletedBy")]
        public virtual Guid? ObsoletedById
        {
            get { return this.m_obsoletedById; }
            set
            {
                if (this.m_obsoletedById != value)
                    this.m_obsoletedBy = null;
                this.m_obsoletedById = value;
            }
        }

        /// <summary>
        /// Represent the data as a string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (K:{1})", this.GetType().Name, this.Key);
        }

        /// <summary>
        /// Clears delay load properties forcing a refresh
        /// </summary>
        public override void Refresh()
        {
            this.m_createdBy = this.m_obsoletedBy = null;
        }
    }
}
