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
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// An associative entity which links a SecurityApplication to an Entity
    /// </summary>
    
    [XmlType("ApplicationEntity",  Namespace = "http://openiz.org/model"), JsonObject("ApplicationEntity")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "ApplicationEntity")]
    public class ApplicationEntity : Entity
    {

        /// <summary>
        /// Creates application entity
        /// </summary>
        public ApplicationEntity()
        {
            this.ClassConceptKey = EntityClassKeys.NonLivingSubject;
            this.DeterminerConceptKey = DeterminerKeys.Specific;
        }

        /// <summary>
        /// Gets or sets the security application
        /// </summary>
        [DataIgnore, XmlElement("securityApplication"), JsonProperty("securityApplication")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        
        public Guid? SecurityApplicationKey
        {
            get { return this.SecurityApplication?.Key; }
            set
            {
                if (this.SecurityApplication?.Key != value)
                    this.SecurityApplication = this.EntityProvider.Get<SecurityApplication>(value);
            }
        }

        /// <summary>
        /// Gets or sets the security application
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(SecurityApplicationKey))]
		public SecurityApplication SecurityApplication { get; set; }

        /// <summary>
        /// Gets or sets the name of the software
        /// </summary>
        [XmlElement("softwareName"), JsonProperty("softwareName")]
        public String SoftwareName { get; set; }

        /// <summary>
        /// Gets or sets the version of the software
        /// </summary>
        [XmlElement("versionName"), JsonProperty("versionName")]
        public String VersionName { get; set; }

        /// <summary>
        /// Gets or sets the vendoer name of the software
        /// </summary>
        [XmlElement("vendorName"), JsonProperty("vendorName")]
        public String VendorName { get; set; }


    }
}
