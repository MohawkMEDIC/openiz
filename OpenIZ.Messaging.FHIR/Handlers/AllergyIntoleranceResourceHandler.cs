using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using OpenIZ.Core.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using MARC.Everest.Connectors;
using System.Linq.Expressions;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Messaging.FHIR.Handlers
{
    /// <summary>
    /// Allergy / intolerance resource handler
    /// </summary>
    public class AllergyIntoleranceResourceHandler : RepositoryResourceHandlerBase<AllergyIntolerance, CodedObservation>
    {
        protected override AllergyIntolerance MapToFhir(CodedObservation model, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }

        protected override CodedObservation MapToModel(AllergyIntolerance resource, WebOperationContext webOperationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query which filters only allergies and intolerances
        /// </summary>
        protected override IEnumerable<CodedObservation> Query(Expression<Func<CodedObservation, bool>> query, List<IResultDetail> issues, Guid queryId, int offset, int count, out int totalResults)
        {
            var anyRef = base.CreateConceptSetFilter(ConceptSetKeys.AllergyIntoleranceTypes, query.Parameters[0]);
            query = Expression.Lambda<Func<CodedObservation, bool>>(Expression.AndAlso(query.Body, anyRef), query.Parameters);
            return base.Query(query, issues, queryId, offset, count, out totalResults);
        }
    }
}
