using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Represents an organization persistence service
    /// </summary>
    public class OrganizationPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Organization, Data.Organization>
    {

       
        /// <summary>
        /// Model instance
        /// </summary>
        public override Core.Model.Entities.Organization ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            var organization = dataInstance as Data.Organization;
            var dbe = context.GetTable<Data.EntityVersion>().Where(o => o.EntityVersionId == organization.EntityVersionId).First();
            var retVal = m_entityPersister.ToModelInstance<Core.Model.Entities.Organization>(dbe, context, principal);
            retVal.IndustryConceptKey = organization.IndustryConceptId;
            return retVal;
        }

        /// <summary>
        /// Insert the organization
        /// </summary>
        public override Core.Model.Entities.Organization Insert(ModelDataContext context, Core.Model.Entities.Organization data, IPrincipal principal)
        {
            // ensure industry concept exists
            data.IndustryConcept?.EnsureExists(context, principal);
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;

            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the organization
        /// </summary>
        public override Core.Model.Entities.Organization Update(ModelDataContext context, Core.Model.Entities.Organization data, IPrincipal principal)
        {
            data.IndustryConcept?.EnsureExists(context, principal);
            data.IndustryConceptKey = data.IndustryConcept?.Key ?? data.IndustryConceptKey;
            return base.Update(context, data, principal);
        }

    }
}
