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
 * Date: 2016-8-4
 */
using System;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Map
{
    /// <summary>
    /// Cast map
    /// </summary>
    [XmlType(nameof(CastMap), Namespace = "http://openiz.org/model/map")]
    public class CastMap : PropertyMap
    {

        // Model type
        private Type m_modelType;

        /// <summary>
        /// Type name
        /// </summary>
        [XmlAttribute("type")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the model CLR type
        /// </summary>
        [XmlIgnore]
        public Type ModelType
        {
            get
            {
                if (this.m_modelType == null)
                    this.m_modelType = Type.GetType(this.TypeName);
                return this.m_modelType;
            }
        }
    }
}