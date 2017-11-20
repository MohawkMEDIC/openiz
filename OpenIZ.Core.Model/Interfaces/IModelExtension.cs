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
 * User: fyfej
 * Date: 2017-7-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model.Interfaces
{
    /// <summary>
    /// Model extension
    /// </summary>
    public interface IModelExtension
    {

        /// <summary>
        /// Gets the extension type key
        /// </summary>
        Guid ExtensionTypeKey { get; }

        /// <summary>
        /// Gets the data for the extension
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Gets the display value
        /// </summary>
        string Display { get; }

        /// <summary>
        /// Gets the value of the extension
        /// </summary>
        object Value { get; }
    }
}
