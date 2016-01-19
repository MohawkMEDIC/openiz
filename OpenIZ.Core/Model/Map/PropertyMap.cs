/**
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
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MARC.Everest.Connectors;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Represents a property map
    /// </summary>
    [XmlType(nameof(PropertyMap), Namespace = "http://openiz.org/model/map")]
    public class PropertyMap
    {
        /// <summary>
        /// Gets or sets the name of the property in the model
        /// </summary>
        [XmlAttribute("modelName")]
        public String ModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the property in the domain model
        /// </summary>
        [XmlAttribute("domainName")]
        public String DomainName { get; set; }

        /// <summary>
        /// Identifies the route 
        /// </summary>
        [XmlElement("via")]
        public List<PropertyMap> Via { get; set; }

        /// <summary>
        /// When this property is a via then traverse this
        /// </summary>
        [XmlAttribute("traverse")]
        public bool Traverse { get; set; }

        /// <summary>
        /// Validate the property type
        /// </summary>
        public IEnumerable<IResultDetail> Validate(Type modelClass, Type domainClass)
        {

            if (domainClass?.IsGenericType == true)
                domainClass = domainClass.GetGenericArguments()[0];
            if (modelClass?.IsGenericType == true)
                modelClass = modelClass.GetGenericArguments()[0];

            List<IResultDetail> retVal = new List<IResultDetail>();
            // 0. Property and model names should exist
            if (String.IsNullOrEmpty(this.DomainName))
                retVal.Add(new RequiredElementMissingResultDetail(ResultDetailType.Error, "@domainName not set", null));

            // 1. The property should exist
            if (!String.IsNullOrEmpty(this.ModelName) && modelClass?.GetProperty(this.ModelName ?? "") == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", modelClass?.Name, this.ModelName), null, null));
            if (domainClass?.GetProperty(this.DomainName ?? "") == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.DomainName), null, null));

            // 2. All property maps should exist
            if (this.Via != null)
                foreach (var v in this.Via)
                    retVal.AddRange(v.Validate(modelClass?.GetProperty(this.ModelName)?.PropertyType, domainClass?.GetProperty(this.DomainName)?.PropertyType));

            return retVal;

        }
    }
}