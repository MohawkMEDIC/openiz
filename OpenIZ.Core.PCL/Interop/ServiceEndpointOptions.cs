using Newtonsoft.Json;
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
        Gs1StockInterface
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
        /// Base URL type
        /// </summary>
        [XmlAttribute("url"), JsonProperty("url")]
        public string[] BaseUrl { get; set; }
    }
}