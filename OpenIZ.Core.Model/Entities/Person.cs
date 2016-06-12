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
 * Date: 2016-2-1
 */
using OpenIZ.Core.Model.Attributes;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.EntityLoader;
using Newtonsoft.Json;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Core.Model.Entities
{
    /// <summary>
    /// Represents an entity which is a person
    /// </summary>
    
    [XmlType("Person",  Namespace = "http://openiz.org/model"), JsonObject("Person")]
    [XmlRoot(Namespace = "http://openiz.org/model", ElementName = "Person")]
    public class Person : Entity
    {

        // Security user
        private SecurityUser m_securityUser;

        // Language communication
        private List<PersonLanguageCommunication> m_languageCommunication;

        /// <summary>
        /// Person constructor
        /// </summary>
        public Person()
        {
            base.DeterminerConceptKey = DeterminerKeys.Specific;
            base.ClassConceptKey = EntityClassKeys.Person;
        }

        /// <summary>
        /// Gets or sets the person's date of birth
        /// </summary>
        [XmlElement("dateOfBirth"), JsonProperty("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the precision ofthe date of birth
        /// </summary>
        [XmlElement("dateOfBirthPrecision"), JsonProperty("dateOfBirthPrecision")]
        public DatePrecision? DateOfBirthPrecision { get; set; }

        /// <summary>
        /// Gets the person's languages of communication
        /// </summary>
        [DelayLoad(null)]
        [XmlElement("language"), JsonProperty("language")]
        public List<PersonLanguageCommunication> LanguageCommunication
        {
            get
            {
                if (this.IsDelayLoadEnabled)
                    this.m_languageCommunication = EntitySource.Current.GetRelations(this.Key, this.VersionSequence, this.m_languageCommunication);

                return this.m_languageCommunication;
            }
            set
            {
                this.m_languageCommunication = value;
            }
        }

        /// <summary>
        /// Forces a refresh of delay load properties
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            this.m_languageCommunication = null;
        }

        /// <summary>
        /// Gets the security user account associated with this person if applicable
        /// </summary>
        public SecurityUser AsSecurityUser
        {
            get
            {
                if(this.IsDelayLoadEnabled && this.m_securityUser == null)
                    this.m_securityUser = EntitySource.Current.Get<UserEntity>(this.Key, this.VersionKey, null).SecurityUser;
                return this.m_securityUser;
            }
        }
    }
}
