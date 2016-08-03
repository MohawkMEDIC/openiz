/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-6-28
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Class redirect on mapper
    /// </summary>
    [XmlType(nameof(ClassRedirect), Namespace = "http://openiz.org/model/map")]
    public class ClassRedirect
    {
        // Domain type
        private Type m_fromType = null;
        /// <summary>
        /// Gets the domain CLR type
        /// </summary>
        [XmlIgnore]
        public Type FromType
        {
            get
            {
                if (this.m_fromType == null)
                    this.m_fromType = Type.GetType(this.FromClass);
                return this.m_fromType;
            }
        }

        /// <summary>
        /// Gets or sets the model class for the mapper
        /// </summary>
        [XmlAttribute("fromClass")]
        public String FromClass { get; set; }

        /// <summary>
        /// Gets or sets the property maps
        /// </summary>
        [XmlElement("via")]
        public List<PropertyMap> Property { get; set; }

        // TODO: Validation
    }
}