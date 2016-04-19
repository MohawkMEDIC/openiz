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
using MARC.HI.EHRS.SVC.Core.Data;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Model
{
    /// <summary>
    /// Model extensions
    /// </summary>
    public static class ModelExtensions
    {

        /// <summary>
        /// Get the identifier
        /// </summary>
        public static Identifier<Guid> Id(this IIdentifiedEntity me)
        {
            // TODO: My AA
            return new Identifier<Guid>(me.Key);
        }

        /// <summary>
        /// Get the identifier
        /// </summary>
        public static Identifier<Guid> Id(this IVersionedEntity me)
        {
            return new Identifier<Guid>(me.Key, me.VersionKey);
        }

        /// <summary>
        /// Validates that this object has a target entity
        /// </summary>
        public static IEnumerable<ValidationResultDetail> Validate<TSourceType>(this VersionedAssociation<TSourceType> me) where TSourceType : VersionedEntityData<TSourceType>
        {
            var validResults = new List<ValidationResultDetail>();
            if (me.SourceEntityKey == Guid.Empty)
                validResults.Add(new ValidationResultDetail(ResultDetailType.Error, String.Format("({0}).{1} required", me.GetType().Name, "SourceEntityKey"), null, null));
            return validResults;
        }

        /// <summary>
        /// Validate the state of this object
        /// </summary>
        public static IEnumerable<ValidationResultDetail> Validate(this IdentifiedData me)
        {
            return new List<ValidationResultDetail>();
        }

        /// <summary>
        /// Convert this AA to OID Data for configuration purposes
        /// </summary>
        public static OidData ToOidData(this AssigningAuthority me)
        {
            return new OidData()
            {
                Name = me.Name,
                Description = me.Description,
                Oid = me.Oid,
                Ref = new Uri(String.IsNullOrEmpty(me.Url) ? String.Format("urn:uuid:{0}", me.Oid) : me.Url),
                Attributes = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>()
                {
                    new System.Collections.Generic.KeyValuePair<string, string>("HL7CX4", me.DomainName)
                    //new System.Collections.Generic.KeyValuePair<string, string>("AssigningDevFacility", this.AssigningDevice.DeviceEvidence)
                }
            };
        }
    }
}
