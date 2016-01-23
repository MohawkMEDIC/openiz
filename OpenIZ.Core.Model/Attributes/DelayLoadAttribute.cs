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
 * Date: 2016-1-19
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Attribute which instructs mapper to ignore a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DelayLoadAttribute : Attribute
    {

        /// <summary>
        /// Gets or sets the key property for the delay field
        /// </summary>
        public DelayLoadAttribute(String keyPropertyName)
        {
            this.KeyPropertyName = keyPropertyName;
        }

        /// <summary>
        /// Gets pr sets the key property name
        /// </summary>
        public String KeyPropertyName { get; set; }
    }
}
