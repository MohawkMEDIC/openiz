using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Messaging.FHIR.Util
{

    /// <summary>
    /// Represents a query parameter map
    /// </summary>
    [XmlType(nameof(QueryParameterMap), Namespace = "http://openiz.org/model/fhir")]
    [XmlRoot(nameof(QueryParameterMap), Namespace = "http://openiz.org/model/fhir")]
    public class QueryParameterMap
    {

        /// <summary>
        /// The type of the map
        /// </summary>
        [XmlElement("type")]
        public List<QueryParameterType> Map { get; set; }

    }

    /// <summary>
    /// Represents a query parameter map
    /// </summary>
    [XmlType(nameof(QueryParameterType), Namespace = "http://openiz.org/model/fhir")]
    public class QueryParameterType
    {


        /// <summary>
        /// Gets or sets the source type
        /// </summary>
        [XmlIgnore]
        public Type SourceType { get; set; }

        /// <summary>
        /// The model type
        /// </summary>
        [XmlAttribute("model")]
        public String SourceTypeXml {
            get { return this.SourceType.AssemblyQualifiedName; }
            set { this.SourceType = Type.GetType(value); }
        }

        /// <summary>
        /// Map the query parameter
        /// </summary>
        [XmlElement("map")]
        public List<QueryParameterMapProperty> Map { get; set; }

    }

    /// <summary>
    /// Represents a query parameter map 
    /// </summary>
    [XmlType(nameof(QueryParameterMapProperty), Namespace = "http://openiz.org/model/fhir")]
    public class QueryParameterMapProperty
    {

        /// <summary>
        /// The model query parameter
        /// </summary>
        [XmlAttribute("model")]
        public String ModelName { get; set; }

        /// <summary>
        /// The FHIR name
        /// </summary>
        [XmlAttribute("fhir")]
        public String FhirName { get; set; }

        /// <summary>
        /// Gets or sets the type of the fhir parmaeter
        /// </summary>
        [XmlAttribute("type")]
        public String FhirType { get; set; }
        
    }

}
