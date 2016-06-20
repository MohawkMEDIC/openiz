using OpenIZ.Core.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Persistence.Data.MSSQL.Data;
using System.Security.Principal;

namespace OpenIZ.Persistence.Data.MSSQL.Services.Persistence
{
    /// <summary>
    /// Concept persistence service
    /// </summary>
    public class ConceptPersistenceService : VersionedDataPersistenceService<Core.Model.DataTypes.Concept, ConceptVersion, Data.Concept>
    {

        /// <summary>
        /// Insert concept 
        /// </summary>
        public override Core.Model.DataTypes.Concept Insert(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {
            data.StatusConceptKey = data.StatusConceptKey ?? StatusKeys.Active;
            data.ClassKey = data.ClassKey == Guid.Empty ? ConceptClassKeys.Other : data.ClassKey;
            
            // Ensure exists
            data.Class?.EnsureExists(context, principal);
            data.StatusConcept?.EnsureExists(context, principal);

            // Persist
            var retVal = base.Insert(context, data, principal);

            // Concept names
            if (retVal.ConceptNames != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptName, Data.ConceptName>(
                    retVal.ConceptNames,
                    data,
                    context,
                    principal
                );

            return retVal;
        }

        /// <summary>
        /// Override update to handle associated items
        /// </summary>
        public override Core.Model.DataTypes.Concept Update(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {

            var retVal = base.Update(context, data, principal);

            var sourceKey = data.Key.ToByteArray();
            if (retVal.ConceptNames != null)
                base.UpdateVersionedAssociatedItems<Core.Model.DataTypes.ConceptName, Data.ConceptName>(
                     retVal.ConceptNames,
                     data,
                     context,
                     principal
                 );

            return retVal;
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override Core.Model.DataTypes.Concept Obsolete(ModelDataContext context, Core.Model.DataTypes.Concept data, IPrincipal principal)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Update(context, data, principal);
        }
    }
}
