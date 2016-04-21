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
 * Date: 2016-4-19
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Represents a property map
    /// </summary>
    [XmlType( nameof(PropertyMap), Namespace = "http://openiz.org/model/map")]
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
        public PropertyMap Via { get; set; }

        /// <summary>
        /// When this property is a via then traverse this
        /// </summary>
        [XmlAttribute("dontLoad")]
        public bool DontLoad { get; set; }

        /// <summary>
        /// Validate the property type
        /// </summary>
        public IEnumerable<ValidationResultDetail> Validate(Type modelClass, Type domainClass)
        {
            
            if (domainClass?.IsConstructedGenericType == true)
                domainClass = domainClass.GenericTypeArguments[0];
            if (modelClass?.IsConstructedGenericType == true)
                modelClass = modelClass.GenericTypeArguments[0];

            List<ValidationResultDetail> retVal = new List<ValidationResultDetail>();
            // 0. Property and model names should exist
            if (String.IsNullOrEmpty(this.DomainName))
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, "@domainName not set", null, null));

            // 1. The property should exist
            if (!String.IsNullOrEmpty(this.ModelName) && modelClass?.GetRuntimeProperty(this.ModelName ?? "") == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", modelClass?.Name, this.ModelName), null, null));
            if (domainClass?.GetRuntimeProperty(this.DomainName ?? "") == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} not found", domainClass?.Name, this.DomainName), null, null));

            // 2. All property maps should exist
            if (this.Via != null)
                retVal.AddRange(this.Via.Validate(modelClass?.GetRuntimeProperty(this.ModelName ?? "")?.PropertyType ?? modelClass, domainClass?.GetRuntimeProperty(this.DomainName)?.PropertyType));

            return retVal;

        }
    }
}