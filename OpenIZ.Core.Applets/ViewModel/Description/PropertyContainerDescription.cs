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
 * Date: 2016-11-30
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Property container description
    /// </summary>
    [XmlType(nameof(PropertyContainerDescription), Namespace = "http://openiz.org/model/view")]
    public abstract class PropertyContainerDescription
    {

        // Property models by name
        private Dictionary<String, PropertyModelDescription> m_properties = new Dictionary<string, PropertyModelDescription>();

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        internal abstract String GetName();

        /// <summary>
        /// Type model description
        /// </summary>
        public PropertyContainerDescription()
        {
            this.Properties = new List<PropertyModelDescription>();
        }

        
        /// <summary>
        /// Property container description
        /// </summary>
        [XmlIgnore]
        public PropertyContainerDescription Parent { get; protected set; }

        /// <summary>
        /// Identifies the properties to be included
        /// </summary>
        [XmlElement("property")]
        public List<PropertyModelDescription> Properties { get; set; }

        /// <summary>
        /// Whether to retrieve all children
        /// </summary>
        [XmlAttribute("all")]
        public bool All { get; set; }

        /// <summary>
        /// Gets the reference to use
        /// </summary>
        [XmlAttribute("ref")]
        public String Ref { get; set; }

        /// <summary>
        /// Find property
        /// </summary>
        public PropertyModelDescription FindProperty(String name)
        {
            PropertyModelDescription model = null;
            if(!this.m_properties.TryGetValue(name, out model))
            {
                model = this.Properties.FirstOrDefault(o => o.Name == name);
                lock (this.m_properties)
                    if (!this.m_properties.ContainsKey(name))
                        this.m_properties.Add(name, model);
            }
            return model;
        }

    }
}
