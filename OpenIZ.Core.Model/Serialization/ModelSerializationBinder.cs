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
 * Date: 2016-2-1
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OpenIZ.Core.Model.Serialization
{
    /// <summary>
    /// Model binding 
    /// </summary>
    public class ModelSerializationBinder : SerializationBinder
    {
        /// <summary>
        /// Bind to type
        /// </summary>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // Assembly to search
            Assembly asm = typeof(ModelSerializationBinder).GetTypeInfo().Assembly;
            if (!String.IsNullOrEmpty(assemblyName))
                asm = Assembly.Load(new AssemblyName(assemblyName));

            // The type
            var type = asm.ExportedTypes.SingleOrDefault(
                t => t.GetTypeInfo().GetCustomAttribute<JsonObjectAttribute>()?.Id == typeName
                );
            if (type == null)
                type = asm.GetType(typeName);
            return type ?? typeof(IdentifiedData);
        }
    }
}
