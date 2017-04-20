using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Services;
using System.ServiceModel.Web;
using OpenIZ.Messaging.IMSI.Util;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
    /// <summary>
    /// Represents a care plan resource handler
    /// </summary>
    public class CareplanResourceHandler : IResourceHandler
    {
        /// <summary>
        /// Gets the resource name
        /// </summary>
        public string ResourceName
        {
            get
            {
                return "CarePlan";
            }
        }

        /// <summary>
        /// Gets the type that this produces
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(Bundle);
            }
        }

        /// <summary>
        /// Create a care plan
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {

            (data as Bundle)?.Reconstitute();
            data = (data as Bundle)?.Entry as Patient ?? data as Patient;
            if (data == null)
                throw new InvalidOperationException("Careplan requires a patient or bundle containing a patient entry");

            // Get care plan service
            var carePlanner = ApplicationContext.Current.GetService<ICarePlanService>();
            var plan = carePlanner.CreateCarePlan(data as Patient, 
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_asEncounters"] == "true",
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery().ToDictionary(o=>o.Key, o=>(Object)o.Value));
            var retVal = new Bundle()
            {
                Item = plan.OfType<IdentifiedData>().ToList(),
                TotalResults = plan.Count(),
                Count = plan.Count(),
                Offset = 0
            };
            retVal.Key = null;
            return retVal;
        }

        /// <summary>
        /// Gets a careplan by identifier
        /// </summary>
        public IdentifiedData Get(Guid id, Guid versionId)
        {

            Patient target = ApplicationContext.Current.GetService<IRepositoryService<Patient>>().Get(id);
            var carePlanner = ApplicationContext.Current.GetService<ICarePlanService>();
            var plan = carePlanner.CreateCarePlan(target,
               WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_asEncounters"] == "true",
               WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery().ToDictionary(o => o.Key, o => (Object)o.Value));

            var retVal = new Bundle()
            {
                Item = plan.OfType<IdentifiedData>().ToList(),
                TotalResults = plan.Count(),
                Count = plan.Count(),
                Offset = 0
            };
            retVal.Key = null;
            return retVal;

        }

        /// <summary>
        /// Obsolete the care plan
        /// </summary>
        public IdentifiedData Obsolete(Guid key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query for care plan
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Query for care plan objects
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Update care plan 
        /// </summary>
        public IdentifiedData Update(IdentifiedData data)
        {
            throw new NotSupportedException();
        }
    }
}
