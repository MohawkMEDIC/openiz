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

using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model
{

    /// <summary>
    /// Represents the root of all model classes in the OpenIZ Core
    /// </summary>
    
    [XmlType("BaseEntityData", Namespace = "http://openiz.org/model")]
    public abstract class BaseEntityData : IdentifiedData
    {

        // Created by identifier
        private Guid m_createdById;
        // Created by
        
        private SecurityUser m_createdBy;
        // Obsoleted by
        private Guid? m_obsoletedById;
        // Obsoleted by user
        
        private SecurityUser m_obsoletedBy;
        
        /// <summary>
        /// Creation Time
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("creationTime")]
        public String CreationTimeXml
        {
            get { return this.CreationTime.ToString("o", CultureInfo.InvariantCulture); }
            set {
                if (value != null)
                    this.CreationTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else this.CreationTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Obsoletion time
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("obsoletionTime")]
        public String ObsoletionTimeXml
        {
            get { return this.ObsoletionTime?.ToString("o", CultureInfo.InvariantCulture); }
            set {
                if (value != null)
                    this.ObsoletionTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else this.ObsoletionTime = null;
            }
        }


        /// <summary>
        /// Gets or sets the user that created this base data
        /// </summary>
        [DelayLoad(nameof(CreatedByKey))]
        [XmlIgnore]
        public virtual SecurityUser CreatedBy {
            get
            {
                this.m_createdBy = base.DelayLoad(this.m_createdById, this.m_createdBy);
                return this.m_createdBy;
            }
         }

        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        [DelayLoad(nameof(ObsoletedByKey))]
        [XmlIgnore]
        public virtual SecurityUser ObsoletedBy {
            get
            {
                this.m_obsoletedBy= base.DelayLoad(this.m_obsoletedById, this.m_obsoletedBy);
                return this.m_obsoletedBy;
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("createdBy")]
        public virtual Guid CreatedByKey
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
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("obsoletedBy")]
        public virtual Guid? ObsoletedByKey
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
