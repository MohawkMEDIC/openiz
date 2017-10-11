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
    /// Represents model descriptions
    /// </summary>
    [XmlType(nameof(TypeModelDescription), Namespace = "http://openiz.org/model/view")]
    public class TypeModelDescription : PropertyContainerDescription
    {

        /// <summary>
        /// Initialize the type mode description
        /// </summary>
        public void Initialize()
        {
            for(int i = 0; i < this.Properties?.Count; i++)
                this.Properties[i].Initialize(this);
        }

        /// <summary>
        /// Name of the type to be loaded
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Get the name of the container
        /// </summary>
        internal override string GetName()
        {
            return this.TypeName;
        }

    }
}