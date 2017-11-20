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
 * Date: 2017-4-13
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{

    /// <summary>
    /// Handler for QOBS
    /// </summary>
    public class QuantityObservationResourceHandler : ObservationResourceHandler<QuantityObservation> { }
    /// <summary>
    /// Handler for COBS
    /// </summary>
    public class CodedObservationResourceHandler : ObservationResourceHandler<CodedObservation> { }
    /// <summary>
    /// Handlers TOBS
    /// </summary>
    public class TextObservationResourceHandler : ObservationResourceHandler<TextObservation> { }

    /// <summary>
    /// Handler for observations (handles permissions)
    /// </summary>
    public abstract class ObservationResourceHandler<TObservation> : ResourceHandlerBase<TObservation> where TObservation : Observation
    {
        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public override IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {
            return base.Create(data, updateIfExists);
        }

        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public override IdentifiedData Get(Guid id, Guid versionId)
        {
            return base.Get(id, versionId);
        }

        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.DeleteClinicalData)]
        public override IdentifiedData Obsolete(Guid key)
        {
            return base.Obsolete(key);
        }

        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public override IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            return base.Query(queryParameters);
        }

        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadClinicalData)]
        public override IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            return base.Query(queryParameters, offset, count, out totalCount);
        }

        [PolicyPermission(System.Security.Permissions.SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.WriteClinicalData)]
        public override IdentifiedData Update(IdentifiedData data)
        {
            return base.Update(data);
        }
    }
    
    
}
