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
 * Date: 2016-7-16
 */
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Acts;
using Newtonsoft.Json;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Entity identifiers
    /// </summary>

    [XmlType("EntityIdentifier", Namespace = "http://openiz.org/model"), JsonObject("EntityIdentifier")]
    public class EntityIdentifier : IdentifierBase<Entity>
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public EntityIdentifier()
        {

        }

        /// <summary>
        /// Creates a new entity identifier with specified authority
        /// </summary>
        public EntityIdentifier(Guid authorityId, String value)
        {
            this.AuthorityKey = authorityId;
            this.Value = value;
        }

        /// <summary>
        /// Creates a new entity identifier
        /// </summary>
        public EntityIdentifier(AssigningAuthority authority, String value)
        {
            this.Authority = authority;
            this.Value = value;
        }

    }


    /// <summary>
    /// Act identifier
    /// </summary>

    [XmlType(Namespace = "http://openiz.org/model", TypeName = "ActIdentifier"), JsonObject("ActIdentifier")]
    public class ActIdentifier : IdentifierBase<Act>
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public ActIdentifier()
        {

        }

        /// <summary>
        /// Creates a new entity identifier with specified authority
        /// </summary>
        public ActIdentifier(Guid authorityId, String value)
        {
            this.AuthorityKey = authorityId;
            this.Value = value;
        }

        /// <summary>
        /// Creates a new entity identifier
        /// </summary>
        public ActIdentifier(AssigningAuthority authority, String value)
        {
            this.Authority = authority;
            this.Value = value;
        }
    }

    /// <summary>
    /// Represents an external assigned identifier
    /// </summary>
    [XmlType(Namespace = "http://openiz.org/model"), JsonObject("IdentifierBase")]
    [Classifier(nameof(Authority))]
    public abstract class IdentifierBase<TBoundModel> : VersionedAssociation<TBoundModel> where TBoundModel : VersionedEntityData<TBoundModel>, new()
    {

        // Identifier id
        private Guid? m_identifierTypeId;
        // Authority id

        private Guid? m_authorityId;

        // Identifier type backing type

        private IdentifierType m_identifierType;
        // Assigning authority

        private AssigningAuthority m_authority;

        /// <summary>
        /// Gets or sets the value of the identifier
        /// </summary>
        [XmlElement("value"), JsonProperty("value")]
        public String Value { get; set; }


        /// <summary>
        /// Gets or sets the assinging authority id
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore, JsonIgnore]
        public Guid? AuthorityKey
        {
            get { return this.m_authorityId; }
            set
            {
                if (this.m_authorityId == value)
                    return;
                this.m_authority = null;
                this.m_authorityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the type identifier
        /// </summary>

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore, JsonIgnore]
        public Guid? IdentifierTypeKey
        {
            get { return this.m_identifierTypeId; }
            set
            {
                if (this.m_identifierTypeId == value)
                    return;
                this.m_identifierType = null;
                this.m_identifierTypeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the identifier type
        /// </summary>
        [SerializationReference(nameof(IdentifierTypeKey))]
        [XmlElement("type"), JsonProperty("type")]
        public IdentifierType IdentifierType
        {
            get
            {
                this.m_identifierType = base.DelayLoad(this.m_identifierTypeId, this.m_identifierType);
                return this.m_identifierType;
            }
            set
            {
                this.m_identifierType = value;
                this.m_identifierTypeId = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the assigning authority 
        /// </summary>
        [SerializationReference(nameof(AuthorityKey))]
        [XmlElement("authority"), JsonProperty("authority")]
        [AutoLoad]
        public AssigningAuthority Authority
        {
            get
            {
                if(this.m_authority == null)
                    this.m_authority = EntitySource.Current.Get<AssigningAuthority>(this.m_authorityId); // base.DelayLoad(this.m_authorityId, this.m_authority);
                return this.m_authority;
            }
            set
            {
                this.m_authority = value;
                this.m_authorityId = value?.Key;
            }
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_authority = null;
            this.m_identifierType = null;
        }

        /// <summary>
        /// True if the identifier is empty
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return String.IsNullOrEmpty(this.Value);
        }

        /// <summary>
        /// Returns true if the objects are equal
        /// </summary>
        public override bool SemanticEquals(object obj)
        {
            var other = obj as IdentifierBase<TBoundModel>;
            if (other == null) return false;
            return base.SemanticEquals(obj) && this.Value == other.Value && this.AuthorityKey == other.AuthorityKey;
        }
    }
}
