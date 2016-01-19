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
    /// Display name of a code system or reference term
    /// </summary>
    [DataContract(Name = "ReferenceTermName", Namespace = "http://openiz.org/model")]
    [Serializable]
    public abstract class ReferenceTermName : BaseEntityData
    {

        // Id of the algorithm used to generate phonetic code
        private Guid m_phoneticAlgorithmId;
        // Algorithm used to generate the code
        [NonSerialized]
        private PhoneticAlgorithm m_phoneticAlgorithm;

        /// <summary>
        /// Back-reference to reference term
        /// </summary>
        [DataMember(Name = "referenceTermId")]
        public Guid ReferenceTermId { get; set; }

        /// <summary>
        /// Gets or sets the language code of the object
        /// </summary>
        [DataMember(Name = "language")]
        public String Language { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference term
        /// </summary>
        [DataMember(Name = "name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the phonetic code of the reference term
        /// </summary>
        [DataMember(Name = "phoneticCode")]
        public String PhoneticCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the phonetic code
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [DataMember(Name = "phoneticAlgorithmId")]
        public Guid PhoneticAlgorithmId
        {
            get { return this.m_phoneticAlgorithmId; }
            set
            {
                this.m_phoneticAlgorithmId = value;
                this.m_phoneticAlgorithm = null;
            }
        }

        /// <summary>
        /// Gets or sets the phonetic algorithm
        /// </summary>
        [DelayLoad]
        [IgnoreDataMember]
        public PhoneticAlgorithm PhoneticAlgorithm
        {
            get
            {
                if(this.m_phoneticAlgorithm == null &&
                    this.DelayLoad &&
                    this.m_phoneticAlgorithmId != Guid.Empty)
                {
                    var dataService = ApplicationContext.Current.GetService<IDataPersistenceService<PhoneticAlgorithm>>();
                    this.m_phoneticAlgorithm = dataService.Get(new MARC.HI.EHRS.SVC.Core.Data.Identifier<Guid>(this.m_phoneticAlgorithmId), null, true);
                }
                return this.m_phoneticAlgorithm;
            }
            set
            {
                this.m_phoneticAlgorithm = value;
                if (value == null)
                    this.m_phoneticAlgorithmId = Guid.Empty;
                else
                    this.m_phoneticAlgorithmId = value.Key;
            }
        }

        /// <summary>
        /// Force reloading of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_phoneticAlgorithm = null;
        }
    }
}