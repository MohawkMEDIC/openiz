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
    /// Patient encounter resource handler
    /// </summary>
    public class PatientEncounterResourceHandler : ResourceHandlerBase<PatientEncounter>
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
