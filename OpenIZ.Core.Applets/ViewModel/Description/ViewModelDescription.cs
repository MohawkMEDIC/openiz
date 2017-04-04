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
 * Date: 2016-11-3
 */
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Represents a refined message model
    /// </summary>
    [XmlType(nameof(ViewModelDescription), Namespace = "http://openiz.org/model/view")]
    [XmlRoot("ViewModel", Namespace = "http://openiz.org/model/view")]
    public class ViewModelDescription
    {

        // Description lookup
        private Dictionary<String, PropertyContainerDescription> m_description = new Dictionary<String, PropertyContainerDescription>();

        // Lock object
        protected object m_lockObject = new object();

        /// <summary>
        /// Defaut ctor
        /// </summary>
        public ViewModelDescription()
        {
            this.Model = new List<TypeModelDescription>();
            this.Include = new List<string>();
        }

        /// <summary>
        /// Includes
        /// </summary>
        [XmlElement("include")]
        public List<String> Include { get; set; }

        /// <summary>
        /// Gets or sets the name of the view model description
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Represents the models which are to be defined in the model
        /// </summary>
        [XmlElement("type")]
        public List<TypeModelDescription> Model { get; set; }

        /// <summary>
        /// Load the specified view model description
        /// </summary>
        public static ViewModelDescription Load(Stream stream)
        {
            XmlSerializer xsz = new XmlSerializer(typeof(ViewModelDescription));
            var retVal = xsz.Deserialize(stream) as ViewModelDescription;
            retVal.Initialize();
            return retVal;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            foreach (var itm in this.Model)
                itm.Initialize();
        }

        /// <summary>
        /// Find description based on name
        /// </summary>
        public PropertyContainerDescription FindDescription(String name)
        {
            PropertyContainerDescription value = null;
            if (!this.m_description.TryGetValue(name, out value))
            {
                value = this.Model.Find(o => o.TypeName == name);
                lock (this.m_lockObject)
                    if(!this.m_description.ContainsKey(name))
                        this.m_description.Add(name, value);
            }
            return value;
        }
        /// <summary>
        /// Find description
        /// </summary>
        public PropertyContainerDescription FindDescription(Type rootType)
        {
            PropertyContainerDescription retVal = null;
            string rootTypeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                               rootType.Name;
            // Type name
            if (!this.m_description.TryGetValue(rootTypeName, out retVal))
            {
                retVal = this.Model.FirstOrDefault(o => o.TypeName == rootTypeName);
                String typeName = rootTypeName;
                // Children from the heirarchy
                while (rootType != typeof(IdentifiedData) && retVal == null)
                {
                    rootType = rootType.GetTypeInfo().BaseType;
                    if (rootType == null) break;
                    typeName = rootType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName ??
                        rootType.Name;

                    if(!this.m_description.TryGetValue(typeName, out retVal))
                        retVal = this.Model.FirstOrDefault(o => o.TypeName == typeName);
                }

                if (retVal != null)
                    lock (this.m_lockObject)
                        if (!this.m_description.ContainsKey(rootTypeName))
                            this.m_description.Add(rootTypeName, retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Find description
        /// </summary>
        public PropertyContainerDescription FindDescription(String propertyName, PropertyContainerDescription context)
        {
            if (propertyName == null)
                return null;
            PropertyContainerDescription retVal = null;
            String pathName = propertyName;
            var pathContext = context;
            while(pathContext != null) { 
                pathName = pathContext.GetName() + "." + pathName;
                pathContext = pathContext.Parent;
            }

            if (!this.m_description.TryGetValue(pathName, out retVal))
            {

                // Find the property information
                retVal = context?.Properties.FirstOrDefault(o => o.Name == propertyName);
                if (retVal == null)
                    retVal = context?.Properties.FirstOrDefault(o => o.Name == "*");
                if (retVal != null)
                    lock (this.m_lockObject)
                        if(!this.m_description.ContainsKey(pathName))
                            this.m_description.Add(pathName, retVal);
            }
            return retVal;
        }
    }
}
