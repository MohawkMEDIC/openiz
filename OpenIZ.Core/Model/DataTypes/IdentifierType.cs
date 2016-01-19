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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic information class which classifies the use of an identifier
    /// </summary>
    [Serializable]
    [DataContract(Name = "IdentifierType", Namespace = "http://openiz.org/model")]
    public class IdentifierType : BaseEntityData
    {

        // Type concept id
        private Guid m_typeConceptId;
        // Scope concept identifier
        private Guid? m_scopeConceptId;
        // Type concept
        [NonSerialized]
        private Concept m_typeConcept;
        // Scope concept
        [NonSerialized]
        private Concept m_scopeConcept;

        /// <summary>
        /// Gets or sets the id of the scope concept
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "scopeConceptRef")]
        public Guid?  ScopeConceptKey
        {
            get { return this.m_scopeConceptId; }
            set
            {
                this.m_scopeConcept = null;
                this.m_scopeConceptId = value;
            }
        }

        /// <summary>
        /// Gets or sets the concept which identifies the type
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DataMember(Name = "typeConceptRef")]
        public Guid  TypeConceptKey
        {
            get { return this.m_typeConceptId; }
            set
            {
                this.m_typeConcept = null;
                this.m_typeConceptId = value;
            }
        }

        /// <summary>
        /// Type concept
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Concept TypeConcept
        {
            get
            {
                return base.DelayLoad(this.m_typeConceptId, this.m_typeConcept);
            }
            set
            {
                this.m_typeConcept = value;
                if (value == null)
                    this.m_typeConceptId = Guid.Empty;
                else
                    this.m_typeConceptId = value.Key;
            }
        }

        /// <summary>
        /// Gets the scope of the identifier
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public Concept Scope
        {
            get
            {
                return base.DelayLoad(this.m_scopeConceptId, this.m_scopeConcept);
            }
            set
            {
                this.m_scopeConcept = value;
                this.m_scopeConceptId = value?.Key;
            }
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_scopeConcept = null;
            this.m_typeConcept = null;
        }
    }
}