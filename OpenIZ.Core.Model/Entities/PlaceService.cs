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

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents a service for a place
    /// </summary>
    
    [XmlType("PlaceService",  Namespace = "http://openiz.org/model"), JsonObject("PlaceService")]
    public class PlaceService : VersionedAssociation<Entity>
    {

        // Service key
        private Guid m_serviceConceptKey;
        // Service
        
        private Concept m_service;

        /// <summary>
        /// The schedule that the service is offered
        /// </summary>
        [XmlElement("serviceSchedule"), JsonProperty("serviceSchedule")]
        public Object ServiceSchedule { get; set; }

        /// <summary>
        /// Gets or sets the service concept key
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("serviceConcept"), JsonProperty("serviceConcept")]
        public Guid ServiceConceptKey
        {
            get { return this.m_serviceConceptKey; }
            set
            {
                this.m_serviceConceptKey = value;
                this.m_service = null;
            }
        }

        /// <summary>
        /// Gets or sets the service concept
        /// </summary>
        [DelayLoad(nameof(ServiceConceptKey))]
        [XmlIgnore, JsonIgnore]
        public Concept ServiceConcept
        {
            get {
                this.m_service = base.DelayLoad(this.m_serviceConceptKey, this.m_service);
                return this.m_service;
            }
            set
            {
                this.m_service = value;
                if (value == null)
                    this.m_serviceConceptKey = Guid.Empty;
                else
                    this.m_serviceConceptKey = value.Key;
            }
        }

        /// <summary>
        /// Refresh the delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_service = null;
        }
    }
}