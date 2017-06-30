using MARC.HI.EHRS.SVC.Messaging.FHIR.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Messaging.FHIR;
using MARC.HI.EHRS.SVC.Messaging.FHIR.Resources;
using System.Collections.Specialized;
using OpenIZ.Messaging.FHIR.Util;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Messaging.FHIR.Handlers
{

    /// <summary>
    /// Valueset resource handler
    /// </summary>
    public class ValueSetResourceHandler : IFhirResourceHandler
    {
        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "ValueSet";
            }
        }

        /// <summary>
        /// Creates the specified resource
        /// </summary>
        public FhirOperationResult Create(ResourceBase target, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the resource
        /// </summary>
        public FhirOperationResult Delete(string id, TransactionMode mode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Query the resource
        /// </summary>
        public FhirQueryResult Query(NameValueCollection parameters)
        {
            var query = QueryParameterLinqExpressionBuilder.BuildFhirQueryObject<ConceptSet>(parameters);
            return new FhirQueryResult()
            {
                Outcome = MARC.Everest.Connectors.ResultCode.Accepted,
                Query = query,
                Results = new List<ResourceBase>(),
                TotalResults = 0
            };
        }

        /// <summary>
        /// Read the resource
        /// </summary>
        public FhirOperationResult Read(string id, string versionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the resource
        /// </summary>
        public FhirOperationResult Update(string id, ResourceBase target, TransactionMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
