
using System;
using System.Runtime.Serialization;
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
namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a code system which is a collection of reference terms
    /// </summary>
    [Serializable]
    [DataContract(Name = "CodeSystem", Namespace = "http://openiz.org/model")]
    public class CodeSystem : BaseEntityData
    {
        /// <summary>
        /// Gets or sets the name of the code system
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Oid of the code system
        /// </summary>
        [DataMember(Name = "oRef")]
        public string Oid { get; set; }

        /// <summary>
        /// Gets or sets the authority of the code system
        /// </summary>
        [DataMember(Name = "authority")]
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion reason of the code system
        /// </summary>
        [DataMember(Name ="obsoletionReason")]
        public string ObsoletionReason { get; set; }

        /// <summary>
        /// Gets or sets the URL of the code system
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version text of the code system
        /// </summary>
        [DataMember(Name = "version")]
        public string VersionText { get; set; }

        /// <summary>
        /// Gets or sets the human description
        /// </summary>
        [DataMember(Name ="description")]
        public string Description { get; set; }
    }
}