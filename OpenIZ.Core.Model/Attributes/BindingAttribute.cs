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

namespace OpenIZ.Core.Model.Attributes
{
    /// <summary>
    /// Binding attributes to suggest what values can be used in a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingAttribute : Attribute
    {

        /// <summary>
        /// Binding attribute
        /// </summary>
        public BindingAttribute(Type binding)
        {
            this.Binding = binding;
        }

        /// <summary>
        /// Gets or sets the type binding
        /// </summary>
        public Type Binding { get; set; }

    }
}
