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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Class mapping
    /// </summary>
    [XmlType( nameof(ClassMap), Namespace = "http://openiz.org/model/map")]
    public class ClassMap
    {

        // Cache
        private Dictionary<String, PropertyMap> m_modelPropertyMap = new Dictionary<String, PropertyMap>();
        private Dictionary<String, PropertyMap> m_domainPropertyMap = new Dictionary<String, PropertyMap>();
        private Object m_lockObject = new Object();

        /// <summary>
        /// Gets or sets the model class for the mapper
        /// </summary>
        [XmlAttribute("modelClass")]
        public String ModelClass { get; set; }

        /// <summary>
        /// Gets or sets the domain class for the mapper
        /// </summary>
        [XmlAttribute("domainClass")]
        public String DomainClass { get; set; }

        /// <summary>
        /// Gets or sets the association maps which are used for collapsing keys
        /// </summary>
        [XmlElement("collapseKey")]
        public List<CollapseKey> CollapseKey { get; set; }

        /// <summary>
        /// Gets or sets the property maps
        /// </summary>
        [XmlElement("property")]
        public List<PropertyMap> Property { get; set; }

        /// <summary>
        /// Try to get a collapse key
        /// </summary>
        public bool TryGetCollapseKey(string propertyName, out CollapseKey retVal)
        {
            retVal = this.CollapseKey.Find(o => o.PropertyName == propertyName);
            return retVal != null;
        }

        /// <summary>
        /// Try to get a property map 
        /// </summary>
        public bool TryGetModelProperty(string modelName, out PropertyMap retVal)
        {
            if (!this.m_modelPropertyMap.TryGetValue(modelName, out retVal))
            {
                retVal = this.Property.Find(o => o.ModelName == modelName);
                lock(this.m_lockObject)
                    if (!this.m_modelPropertyMap.ContainsKey(modelName))
                        this.m_modelPropertyMap.Add(modelName, retVal);
            }
            return retVal != null;
        }

        /// <summary>
        /// Validate the class map
        /// </summary>
        public IEnumerable<ValidationResultDetail> Validate()
        {
#if DEBUG
            Debug.WriteLine(String.Format("Validating {0}>{1}", this.ModelClass, this.DomainClass));
#endif
            List<ValidationResultDetail> retVal = new List<ValidationResultDetail>();
            Type modelClass = Type.GetType(this.ModelClass),
                domainClass = Type.GetType(this.DomainClass);
            if (modelClass == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("Class {0} not found", this.ModelClass), null, null));
            if(domainClass == null)
                retVal.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("Class {0} not found", this.DomainClass), null, null));

            foreach(var p in this.Property)
                retVal.AddRange(p.Validate(modelClass, domainClass).Select(o => { o.Location = this.ModelClass; return o; }));
            foreach (var k in this.CollapseKey)
                retVal.AddRange(k.Validate(domainClass).Select(o => { o.Location = this.ModelClass; return o; }));

            return retVal;
        }

        /// <summary>
        /// Try to get a property map 
        /// </summary>
        public bool TryGetDomainProperty(string domainName, out PropertyMap retVal)
        {

            if (!this.m_domainPropertyMap.TryGetValue(domainName, out retVal))
            {
                retVal = this.Property.Find(o => o.DomainName == domainName);
                lock (this.m_lockObject)
                    if (!this.m_domainPropertyMap.ContainsKey(domainName))
                        this.m_domainPropertyMap.Add(domainName, retVal);
            }
            return retVal != null;

            
        }
    }
}