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
using OpenIZ.Core.Applets.ViewModel.Description;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Applets.ViewModel.Json
{
    /// <summary>
    /// Represents a JSON serialization context
    /// </summary>
    public class JsonSerializationContext : SerializationContext
    {
        /// <summary>
        /// Creates a new JSON Serialization context
        /// </summary>
        public JsonSerializationContext(String propertyName, JsonViewModelSerializer context, Object instance) : base(propertyName, context, instance)
        {
        }

        /// <summary>
        /// Creates a new JSON serialization context
        /// </summary>
        public JsonSerializationContext(String propertyName, JsonViewModelSerializer context, Object instance, JsonSerializationContext parent) : base(propertyName, context, instance, parent)
        {

        }

        /// <summary>
        /// Gets the Context as a JSON serializer
        /// </summary>
        public JsonViewModelSerializer JsonContext { get { return base.Context as JsonViewModelSerializer; } }

        /// <summary>
        /// Gets the JSON parent context
        /// </summary>
        public JsonSerializationContext JsonParent {  get { return base.Parent as JsonSerializationContext; } }

       
    }
}
