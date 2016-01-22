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
using System.Linq;
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenIZ.Core.Model.Attributes;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a basic reference term
    /// </summary>
    [Serializable]
    [DataContract(Name = "ReferenceTerm", Namespace = "http://openiz.org/model")]
    [Resource(ModelScope.Concept)]
    public class ReferenceTerm : IdentifiedData
    {

        // Backing field for code system identifier
        private Guid m_codeSystemId;
        // Code system
        
        private CodeSystem m_codeSystem;
        // Display names
        
        private List<ReferenceTermName> m_displayNames;

        /// <summary>
        /// Gets or sets the mnemonic for the reference term
        /// </summary>
        [DataMember(Name = "mnemonic")]
        public string Mnemonic { get; set; }

        /// <summary>
        /// Gets or sets the code system 
        /// </summary>
        [DelayLoad(nameof(CodeSystemKey))]
        [DataMember(Name = "codeSystem")]
        public CodeSystem CodeSystem {
            get
            {
                this.m_codeSystem = base.DelayLoad(this.m_codeSystemId, this.m_codeSystem);
                return this.m_codeSystem;
            }
            set
            {
                this.m_codeSystem = value;
                if (value == null)
                    this.m_codeSystemId = Guid.Empty;
                else
                    this.m_codeSystemId = value.Key;
            }
        }
        
        /// <summary>
        /// Gets or sets the code system identifier
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [IgnoreDataMember]
        [Browsable(false)]
        public Guid  CodeSystemKey {
            get { return this.m_codeSystemId; }
            set
            {
                this.m_codeSystemId = value;
                this.m_codeSystem = null;
            }
        }

        /// <summary>
        /// Gets display names associated with the reference term
        /// </summary>
        [DelayLoad(null)]
        [DataMember(Name = "name")]
        public List<ReferenceTermName> DisplayNames {
            get
            {
                if(this.m_displayNames == null && this.IsDelayLoadEnabled)
                {
                    var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<ReferenceTermName>>();
                    this.m_displayNames = dataService.Query(o => o.ReferenceTermKey == this.Key && o.ObsoletionTime == null, null).ToList();
                }
                return this.m_displayNames;
            }
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            this.m_codeSystem = null;
            this.m_displayNames = null;
        }

    }
}