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
    /// Model map format class
    /// </summary>
    [XmlRoot("modelMap", Namespace = "http://openiz.org/model/map")]
    [XmlType( nameof(ModelMap), Namespace = "http://openiz.org/model/map")]
    public class ModelMap
    {

        // Class cache
        private Dictionary<Type, ClassMap> m_classCache = new Dictionary<Type, ClassMap>();
        // Lock object
        private Object m_lockObject = new Object();

        /// <summary>
        /// Gets or sets the class mapping
        /// </summary>
        [XmlElement("class")]
        public List<ClassMap> Class { get; set; }

        /// <summary>
        /// Get a class map for the specified type
        /// </summary>
        public ClassMap GetModelClassMap(Type type)
        {
            ClassMap retVal = null;
            if(!this.m_classCache.TryGetValue(type, out retVal))
            {
                retVal = this.Class.Find(o => Type.GetType(o.ModelClass) == type);
                if(retVal != null)
                    lock(this.m_lockObject)
                        this.m_classCache.Add(type, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Validate the map
        /// </summary>
        public IEnumerable<IResultDetail> Validate()
        {
            List<IResultDetail> retVal = new List<IResultDetail>();
            foreach(var cls in this.Class)
                retVal.AddRange(cls.Validate());
            return retVal;
        }
    }
}
