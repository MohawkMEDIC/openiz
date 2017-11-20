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
 * Date: 2016-8-2
 */
using Newtonsoft.Json;
using System.Xml.Serialization;
using System;
using OpenIZ.Core.Model.Attributes;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents information related to the clinical protocol to which an act is a member of
    /// </summary>
    /// <remarks>
    /// The <see cref="ActProtocol"/> class is used to link an act instance (<see cref="Act"/>) with the clinical 
    /// protocol (<see cref="Protocol"/>) to which the act belongs.
    /// </remarks>
    [XmlType(nameof(ActProtocol), Namespace = "http://openiz.org/model"), JsonObject(nameof(ActProtocol))]
    public class ActProtocol : Association<Act>
    {
        /// <summary>
        /// Gets or sets the protocol  to which this act belongs
        /// </summary>
        [XmlElement("protocol"), JsonProperty("protocol")]
        public Guid ProtocolKey { get; set; }

        /// <summary>
        /// Gets or sets the protocol data related to the protocol
        /// </summary>
        [XmlIgnore, JsonIgnore, SerializationReferenceAttribute(nameof(ProtocolKey))]
        public Protocol Protocol { get; set; }

        /// <summary>
        /// Represents the sequence of the act in the protocol
        /// </summary>
        [XmlElement("sequence"), JsonProperty("sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// Represents any state data related to the act / protocol link
        /// </summary>
        [XmlElement("state"), JsonProperty("state")]
        public byte[] StateData { get; set; }

		/// <summary>
		/// Determines equality of this association
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public override bool SemanticEquals(object obj)
        {
            var other = obj as ActProtocol;
            if (other == null) return false;
            return base.SemanticEquals(obj) && other.ProtocolKey == this.ProtocolKey;
        }

        /// <summary>
        /// Shoud serialize source
        /// </summary>
        public override bool ShouldSerializeSourceEntityKey()
        {
            return false;
        }
    }
}