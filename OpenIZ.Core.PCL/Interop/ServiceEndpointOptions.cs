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