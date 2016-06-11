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
using OpenIZ.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Model map format class
    /// </summary>
    [XmlRoot(Namespace = "http://openiz.org/model/map", ElementName ="modelMap")]
    [XmlType(nameof(ModelMap), Namespace = "http://openiz.org/model/map")]
    public class ModelMap
    {

        // Class cache
        private Dictionary<Type, ClassMap> m_classCache = new Dictionary<Type, ClassMap>();
        // Lock object
        private Object m_lockObject = new Object();


        /// <summary>
        /// Creates the specified model mmap
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ModelMap Load(Stream sourceStream)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ModelMap));
            var retVal = xsz.Deserialize(sourceStream) as ModelMap;
            var validation = retVal.Validate();
            if (validation.Any(o => o.Level == ResultDetailType.Error))
                throw new ModelMapValidationException(validation);
            return retVal;
        }

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
                        if(!this.m_classCache.ContainsKey(type))
                            this.m_classCache.Add(type, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Validate the map
        /// </summary>
        public IEnumerable<ValidationResultDetail> Validate()
        {
            List<ValidationResultDetail> retVal = new List<ValidationResultDetail>();
            foreach(var cls in this.Class)
                retVal.AddRange(cls.Validate());
            return retVal;
        }
    }
}
