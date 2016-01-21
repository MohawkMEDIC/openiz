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
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace OpenIZ.Core.Model.DataTypes
{
    /// <summary>
    /// Represents a name (human name) that a concept may have
    /// </summary>
    [Serializable]
    [DataContract(Name = "ConceptName", Namespace = "http://openiz.org/model")]
    public class ConceptName : VersionBoundRelationData<Concept>
    {

        // Id of the algorithm used to generate phonetic code
        private Guid m_phoneticAlgorithmId;

        // Algorithm used to generate the code
        [NonSerialized]
        private PhoneticAlgorithm m_phoneticAlgorithm;

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
        [DataMember(Name = "phoneticAlgorithmRef")]
        public Guid  PhoneticAlgorithmKey
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
                this.m_phoneticAlgorithm = base.DelayLoad(this.m_phoneticAlgorithmId, this.m_phoneticAlgorithm);
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
        /// Refresh the object's delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_phoneticAlgorithm = null;
        }

    }
}