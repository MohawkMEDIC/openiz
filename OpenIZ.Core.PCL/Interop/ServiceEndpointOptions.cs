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
 * Date: 2017-5-4
 */
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Interop
{
    /// <summary>
    /// Service endpoint type
    /// </summary>
    [XmlType(nameof(ServiceEndpointType), Namespace = "http://openiz.org/model")]
    public enum ServiceEndpointType
    {
        [XmlEnum("imsi")]
        ImmunizationIntegrationService,
        [XmlEnum("risi")]
        ReportIntegrationService,
        [XmlEnum("ami")]
        AdministrationIntegrationService,
        [XmlEnum("pixpdq")]
        IhePixPdqInterface,
        [XmlEnum("fhir")]
        Hl7FhirInterface,
        [XmlEnum("gs1")]
        Gs1StockInterface,
        [XmlEnum("acs")]
        AuthenticationService
    }

    /// <summary>
    /// Represents service capabilities
    /// </summary>
    [XmlType(nameof(ServiceEndpointCapabilities), Namespace = "http://openiz.org/model"), Flags]
    public enum ServiceEndpointCapabilities
    {
        /// <summary>
        /// No options
        /// </summary>
        [XmlEnum("none")]
        None,
        /// <summary>
        /// Basic auth
        /// </summary>
        [XmlEnum("basic")]
        BasicAuth = 0x2,
        /// <summary>
        /// Bearer auth
        /// </summary>
        [XmlEnum("bearer")]
        BearerAuth = 0x4,
        /// <summary>
        /// Endpoint supports compression
        /// </summary>
        [XmlEnum("compress")]
        Compression = 0x1
    }

    /// <summary>
    /// Service endpoint options
    /// </summary>
    [XmlType(nameof(ServiceEndpointOptions), Namespace = "http://openiz.org/model"), JsonObject(nameof(ServiceEndpointOptions))]
    public class ServiceEndpointOptions
    {
        /// <summary>
        /// Gets or sets the service endpoint type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public ServiceEndpointType ServiceType { get; set; }

        /// <summary>
        /// Capabilities
        /// </summary>
        [XmlAttribute("cap"), JsonProperty("cap")]
        public ServiceEndpointCapabilities Capabilities { get; set; }

        /// <summary>
        /// Base URL type
        /// </summary>
        [XmlAttribute("url"), JsonProperty("url")]
        public string[] BaseUrl { get; set; }
    }
}