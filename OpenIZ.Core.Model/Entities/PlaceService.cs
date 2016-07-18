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
using OpenIZ.Core.Model.DataTypes;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a service for a place
    /// </summary>
    
    [XmlType("PlaceService",  Namespace = "http://openiz.org/model"), JsonObject("PlaceService")]
    public class PlaceService : VersionedAssociation<Entity>
    {


        /// <summary>
        /// The schedule that the service is offered
        /// </summary>
        [XmlElement("serviceSchedule"), JsonProperty("serviceSchedule")]
        public Object ServiceSchedule { get; set; }

        /// <summary>
        /// Gets or sets the service concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataIgnore, XmlElement("serviceConcept"), JsonProperty("serviceConcept")]
        public Guid? ServiceConceptKey
        {
            get { return this.ServiceConcept?.Key; }
            set
            {
                if (this.ServiceConcept?.Key != value)
                    this.ServiceConcept = this.EntityProvider?.Get<Concept>(value);
            }
        }

        /// <summary>
        /// Gets or sets the service concept
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReference(nameof(ServiceConceptKey))]
		public Concept ServiceConcept { get; set; }


    }
}