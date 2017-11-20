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
 * Date: 2016-8-14
 */
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
