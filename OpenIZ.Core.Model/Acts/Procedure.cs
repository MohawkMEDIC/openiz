/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-10-30
 */
using Newtonsoft.Json;
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Core.Model.Acts
{
    /// <summary>
    /// Represents a class which has an immediate and primary outcome and is an alteration 
    /// of the physical condition of the subject.
    /// </summary>
    [XmlType(nameof(Procedure), Namespace = "http://openiz.org/model"), JsonObject(nameof(Procedure))]
    [XmlRoot(nameof(Procedure), Namespace = "http://openiz.org/model")]
    public class Procedure : Act
    {

        private Guid? m_methodKey;
        private Guid? m_approachSiteKey;
        private Guid? m_targetSiteKey;

        private Concept m_method;
        private Concept m_approachSite;
        private Concept m_targetSite;

        /// <summary>
        /// Default ctor for procedure
        /// </summary>
        public Procedure()
        {
            base.ClassConceptKey = ActClassKeys.Procedure;
        }

        /// <summary>
        /// Gets or sets te method/technique used to perform the procedure
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("method"), JsonProperty("method")]
        public Guid? MethodKey
        {
            get { return this.m_methodKey; }
            set
            {
                if (this.m_methodKey != value)
                {
                    this.m_methodKey = value;
                    this.m_method = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the anatomical site or system through which the procedure was performed
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("approachSite"), JsonProperty("approachSite")]
        public Guid? ApproachSiteKey
        {
            get {
                return this.m_approachSiteKey;
            }
            set
            {
                if(this.m_approachSiteKey != value)
                {
                    this.m_approachSiteKey = value;
                    this.m_approachSite = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the anatomical site or system which is the target of the procedure
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("targetSite"), JsonProperty("targetSite")]
        public Guid? TargetSiteKey
        {
            get
            {
                return this.m_targetSiteKey;
            }
            set
            {
                if(this.m_targetSiteKey != value)
                {
                    this.m_targetSiteKey = value;
                    this.m_targetSite = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets te method/technique used to perform the procedure
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(MethodKey))]
        public Concept Method
        {
            get
            {
                this.m_method = base.DelayLoad(this.m_methodKey, this.m_method);
                return this.m_method;
            }
            set
            {
                this.m_method = value;
                this.m_methodKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets the anatomical site or system which is the target of the procedure
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(ApproachSiteKey))]
        public Concept ApproachSite
        {
            get
            {
                this.m_approachSite = base.DelayLoad(this.m_approachSiteKey, this.m_approachSite);
                return this.m_approachSite;
            }
            set
            {
                this.m_approachSite = value;
                this.m_approachSiteKey = value?.Key;
            }
        }

        /// <summary>
        /// Gets or sets te method/technique used to perform the procedure
        /// </summary>
        [AutoLoad, XmlIgnore, JsonIgnore]
        [SerializationReference(nameof(TargetSiteKey))]
        public Concept TargetSite
        {
            get
            {
                this.m_targetSite = base.DelayLoad(this.m_targetSiteKey, this.m_targetSite);
                return this.m_targetSite;
            }
            set
            {
                this.m_targetSite = value;
                this.m_targetSiteKey = value?.Key;
            }
        }

    }
}
