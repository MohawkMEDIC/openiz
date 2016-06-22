using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Persistence.Data.MSSQL.Data;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Persistence service for matrials
    /// </summary>
    public class MaterialPersistenceService : EntityDerivedPersistenceService<Core.Model.Entities.Material, Data.Material>
    {

        /// <summary>
        /// Convert persistence model to business objects
        /// </summary>
        public override Core.Model.Entities.Material ToModelInstance(object dataInstance, ModelDataContext context, IPrincipal principal)
        {
            return this.ToModelInstance<Core.Model.Entities.Material>(dataInstance as Data.Material, context, principal);
        }

        /// <summary>
        /// Creates the specified model instance
        /// </summary>
        internal TModel ToModelInstance<TModel>(Data.Material dataInstance, ModelDataContext context, IPrincipal principal)
            where TModel : Core.Model.Entities.Material, new()
        {
            var dbe = context.GetTable<Data.EntityVersion>().FirstOrDefault(o => o.EntityVersionId == dataInstance.EntityVersionId);
            var retVal = this.m_entityPersister.ToModelInstance<TModel>(dbe, context, principal);
            retVal.ExpiryDate = dataInstance.ExpiryDate;
            retVal.IsAdministrative = dataInstance.IsAdministrative;
            retVal.Quantity = dataInstance.Quantity;
            retVal.QuantityConceptKey = dataInstance.QuantityConceptId;
            retVal.FormConceptKey = dataInstance.FormConceptId;
            return retVal;

        }

        /// <summary>
        /// Insert the material
        /// </summary>
        public override Core.Model.Entities.Material Insert(ModelDataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            data.FormConcept?.EnsureExists(context, principal);
            data.QuantityConcept?.EnsureExists(context, principal);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Insert(context, data, principal);
        }

        /// <summary>
        /// Update the specified material
        /// </summary>
        public override Core.Model.Entities.Material Update(ModelDataContext context, Core.Model.Entities.Material data, IPrincipal principal)
        {
            data.FormConcept?.EnsureExists(context, principal);
            data.QuantityConcept?.EnsureExists(context, principal);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Update(context, data, principal);
        }
    }
}
