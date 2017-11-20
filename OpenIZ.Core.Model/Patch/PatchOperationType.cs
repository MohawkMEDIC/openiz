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
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Patch
{
    /// <summary>
    /// Represents a patch operation type
    /// </summary>
    [XmlType(nameof(PatchOperationType), Namespace = "http://openiz.org/model")]
    public enum PatchOperationType
    {
        /// <summary>
        /// Patch operation adds the specified value to the array
        /// </summary>
        [XmlEnum("add")]
        Add, 
        /// <summary>
        /// Patch operation removes the specified value from the array
        /// </summary>
        [XmlEnum("remove")]
        Remove,
        /// <summary>
        /// Patch operation replaces the specified item at the path 
        /// </summary>
        [XmlEnum("replace")]
        Replace, 
        /// <summary>
        /// Patch should test value before proceeding
        /// </summary>
        [XmlEnum("test")]
        Test
    }
}