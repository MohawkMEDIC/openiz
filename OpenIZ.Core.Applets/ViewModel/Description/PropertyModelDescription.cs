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
using System.Xml.Serialization;

namespace OpenIZ.Core.Applets.ViewModel.Description
{
    /// <summary>
    /// Property model description
    /// </summary>
    [XmlType(nameof(PropertyModelDescription) ,Namespace = "http://openiz.org/model/view")]
    public class PropertyModelDescription : PropertyContainerDescription
    {


        /// <summary>
        /// Initialize the parent structure
        /// </summary>
        public void Initialize(PropertyContainerDescription parent)
        {
            this.Parent = parent;
            foreach (var itm in this.Properties)
                itm.Initialize(this);
        }


        /// <summary>
        /// The property of the model
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or ssets the where classifiers
        /// </summary>
        [XmlAttribute("classifier")]
        public String Classifier { get; set; }

        /// <summary>
        /// Seriallization behavior
        /// </summary>
        [XmlAttribute("behavior")]
        public SerializationBehaviorType Action { get; set; }

        /// <summary>
        /// Get name 
        /// </summary>
        internal override string GetName()
        {
            return this.Name;
        }
    }
}