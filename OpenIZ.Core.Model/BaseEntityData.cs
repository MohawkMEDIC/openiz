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
    
    [XmlType("BaseEntityData",  Namespace = "http://openiz.org/model"), JsonObject("BaseEntityData")]
    public abstract class BaseEntityData : IdentifiedData, IBaseEntityData
    {

        /// <summary>
        /// Constructs a new base entity data
        /// </summary>
        public BaseEntityData()
        {
        }

        /// <summary>
        /// Creation Time
        /// </summary>
        [XmlIgnore, JsonIgnore]
		public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [DataIgnore, XmlElement("creationTime"), JsonProperty("creationTime")]
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
        [XmlIgnore, JsonIgnore]
		public DateTimeOffset? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [DataIgnore, XmlElement("obsoletionTime", IsNullable = false), JsonProperty("obsoletionTime")]
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
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(CreatedByKey))]
        public virtual SecurityUser CreatedBy
        {
            get; set;
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
        [XmlIgnore, JsonIgnore]
        [AutoLoad, SerializationReference(nameof(ObsoletedByKey))]
        public virtual SecurityUser ObsoletedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("createdBy"), JsonProperty("createdBy")]
        public virtual Guid? CreatedByKey { get
            {
                return this.CreatedBy?.Key;
            }
            set
            {
                if (this.CreatedBy?.Key != value)
                    this.CreatedBy = this.EntityProvider.Get<SecurityUser>(value);
            }
        }

        /// <summary>
        /// Gets or sets the obsoleted by identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("obsoletedBy"), JsonProperty("obsoletedBy")]
        public virtual Guid? ObsoletedByKey {
            get {
                return this.ObsoletedBy?.Key;
            }
            set
            {
                if (this.ObsoletedBy?.Key != value)
                    this.ObsoletedBy = this.EntityProvider.Get<SecurityUser>(value);
            }
        }

        /// <summary>
        /// Represent the data as a string
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} (K:{1})", this.GetType().Name, this.Key);
        }

    }
}
