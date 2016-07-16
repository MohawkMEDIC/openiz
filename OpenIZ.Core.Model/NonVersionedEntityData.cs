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
 * Date: 2016-2-10
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Updateable entity data which is not versioned
    /// </summary>
    [XmlType(nameof(NonVersionedEntityData), Namespace = "http://openiz.org/model")]
    [JsonObject(Id = nameof(NonVersionedEntityData))]
    public class NonVersionedEntityData : BaseEntityData
    {


        /// <summary>
        /// Updated time
        /// </summary>
        [XmlIgnore, JsonIgnore]
		public DateTimeOffset? UpdatedTime { get; set; }


        /// <summary>
        /// Gets or sets the creation time in XML format
        /// </summary>
        [XmlElement("updatedTime"), JsonProperty("updatedTime")]
        public String UpdatedTimeXml
        {
            get { return this.UpdatedTime?.ToString("o", CultureInfo.InvariantCulture); }
            set
            {
                if (value != null)
                    this.UpdatedTime = DateTimeOffset.ParseExact(value, "o", CultureInfo.InvariantCulture);
                else
                    this.UpdatedTime = null;
            }
        }

        /// <summary>
        /// Gets or sets the user that updated this base data
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(UpdatedByKey))]
		public SecurityUser UpdatedBy
        {
            get { return this.EntityProvider.Get<SecurityUser>(this.UpdatedByKey); }
            set
            {
                this.UpdatedByKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the created by identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("updatedBy"), JsonProperty("updatedBy")]
        public Guid? UpdatedByKey { get; set; }


        /// <summary>
        /// True if key should be serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeUpdatedByKey()
        {
            return this.UpdatedByKey.HasValue;
        }


    }
}
