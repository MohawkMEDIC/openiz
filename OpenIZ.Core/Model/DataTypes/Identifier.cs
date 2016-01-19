/**
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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /*
    /// <summary>
    /// Entity identifiers
    /// </summary>
    public class EntityIdentifier : Identifier<Entity>
    {

    }

    /// <summary>
    /// Act identifier
    /// </summary>
    public class ActIdentifier : Identifier<Act>
    {

    }
    */
    /// <summary>
    /// Represents an external assigned identifier
    /// </summary>
    [DataContract(Name = "IdentifierBase", Namespace = "http://openiz.org/model")]
    [Serializable]
    public abstract class IdentifierBase<TBoundModel> : VersionBoundRelationData<TBoundModel> where TBoundModel : VersionedEntityData<TBoundModel>
    {

        // Identifier id
        private Guid? m_identifierTypeId;
        // Authority id
        [NonSerialized]
        private Guid m_authorityId;

        // Identifier type backing type
        [NonSerialized]
        private IdentifierType m_identifierType;
        // Assigning authority
        [NonSerialized]
        private AssigningAuthority m_authority;

        /// <summary>
        /// Gets or sets the value of the identifier
        /// </summary>
        [DataMember(Name = "value")]
        public String Value { get; set; }

        /// <summary>
        /// Gets or sets the identifier type
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public IdentifierType Type
        {
            get
            {
               if(this.DelayLoad && 
                    this.m_identifierType == null && 
                    this.m_identifierTypeId.HasValue)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<IdentifierType>>();
                    this.m_identifierType = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_identifierTypeId.Value), null, true);
                }
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
        [DelayLoad]
        [IgnoreDataMember]
        public AssigningAuthority Authority
        {
            get
            {
                if (this.DelayLoad &&
                    this.m_authorityId != Guid.Empty &&
                    this.m_authority == null)
                {
                    var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<AssigningAuthority>>();
                    this.m_authority = persistenceService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_authorityId), null, true);
                }
                return this.m_authority;
            }
            set
            {
                this.m_authority = value;
                if (value != null)
                    this.m_authorityId = value.Key;
                else
                    this.m_authorityId = Guid.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the assinging authority id
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "authorityId")]
        public Guid AuthorityId {
            get { return this.m_authorityId; }
            set
            {
                if (this.m_authority?.Key == value)
                    return;
                this.m_authority = null;
                this.m_authorityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the type identifier
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "typeId")]
        public Guid? TypeId
        {
            get { return this.m_identifierTypeId; }
            set
            {
                if (this.m_identifierType?.Key == value)
                    return;
                this.m_identifierType = null;
                this.m_identifierTypeId = value;
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
    }
}
