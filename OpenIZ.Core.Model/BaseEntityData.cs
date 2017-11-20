/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
using OpenIZ.Core.Model.Interfaces;
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
    /// <remarks>
    /// This abstract class is used to encapsulate the key properties of base data elements in the OpenIZ
    /// model, namely it keeps track of which entities created and obsoleted a particular resource and when those
    /// events occurred.
    /// </remarks>
    
    [XmlType("BaseEntityData",  Namespace = "http://openiz.org/model"), JsonObject("BaseEntityData")]
    public abstract class BaseEntityData : IdentifiedData, IBaseEntityData
    {

        // Created by identifier
        private Guid? m_createdById;
        // Created by
        private SecurityUser m_createdBy;
        // Obsoleted by
        private Guid? m_obsoletedById;
        // Obsoleted by user
        private SecurityUser m_obsoletedBy;

        /// <summary>
        /// Constructs a new base entity data
        /// </summary>
        public BaseEntityData()
        {
        }

        /// <summary>
        /// Gets or sets the time at which the data was created
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(CreationTimeXml))]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("creationTime"), JsonProperty("creationTime"), DataIgnore()]
        public String CreationTimeXml
        {
            get {
				if (this.CreationTime == default(DateTimeOffset))
					return null;
				else
					return this.CreationTime.ToString("o", CultureInfo.InvariantCulture);
			}
            set {
                if (value != null)
                    this.CreationTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else this.CreationTime = default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Gets or sets the time when the data is or will become invalid
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ObsoletionTimeXml))]
        public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("obsoletionTime", IsNullable = false), JsonProperty("obsoletionTime"), DataIgnore()]
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
        [SerializationReference(nameof(CreatedByKey)), DataIgnore()]
        [XmlIgnore, JsonIgnore]
        public virtual SecurityUser CreatedBy {
            get
            {
                this.m_createdBy = base.DelayLoad(this.m_createdById, this.m_createdBy);
                return this.m_createdBy;
            }
            set
            {
                this.m_createdBy = value;
                this.m_createdById = value?.Key;
            }
         }

        /// <summary>
        /// Gets the time that the object was last modified (from base data, default to CreationTime)
        /// </summary>
        [JsonIgnore, XmlIgnore]

        public override DateTimeOffset ModifiedOn
        {
            get
            {
                return this.CreationTime;
            }
        }


        /// <summary>
        /// True if key should be serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeCreatedByKey()
        {
            return this.CreatedByKey.HasValue;
        }

        /// <summary>
        /// True if key should be serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeObsoletedByKey()
        {
            return this.ObsoletedByKey.HasValue;
        }
        

        /// <summary>
        /// Gets or sets the user that obsoleted this base data
        /// </summary>
        [SerializationReference(nameof(ObsoletedByKey)), DataIgnore()]
        [XmlIgnore, JsonIgnore]
        public virtual SecurityUser ObsoletedBy {
            get
            {
                this.m_obsoletedBy= base.DelayLoad(this.m_obsoletedById, this.m_obsoletedBy);
                return this.m_obsoletedBy;
            }
            set
            {
                this.m_obsoletedBy = value;
                if (value == null)
                    this.m_obsoletedById = Guid.Empty;
                else
                    this.m_obsoletedById = value.Key;

            }
        }

        /// <summary>
        /// Gets or sets the identifier of the user which created the data
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("createdBy"), JsonProperty("createdBy")]
        public virtual Guid? CreatedByKey
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
        /// Gets or sets the identifier of the user which obsoleted the data
        /// </summary>
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("obsoletedBy"), JsonProperty("obsoletedBy")]
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
