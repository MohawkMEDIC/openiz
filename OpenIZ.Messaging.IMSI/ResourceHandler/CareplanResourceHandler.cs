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
 * Date: 2017-4-22
 */
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
using OpenIZ.Core.Model.Acts;
using System.Linq.Expressions;
using OpenIZ.Core.Security;

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
                return typeof(CarePlan);
            }
        }

        /// <summary>
        /// Create a care plan
        /// </summary>
        public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
        {

            (data as Bundle)?.Reconstitute();
            data = (data as CarePlan)?.Target ?? data as Patient;
            if (data == null)
                throw new InvalidOperationException("Careplan requires a patient or bundle containing a patient entry");

            // Get care plan service
            var carePlanner = ApplicationContext.Current.GetService<ICarePlanService>();
            var plan = carePlanner.CreateCarePlan(data as Patient, 
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["_asEncounters"] == "true",
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.ToQuery().ToDictionary(o=>o.Key, o=>(Object)o.Value));

            // Expand the participation roles form the care planner
            IConceptRepositoryService conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
            foreach (var p in plan.Action)
                p.Participations.ForEach(o => o.ParticipationRoleKey = o.ParticipationRoleKey ?? conceptService.GetConcept(o.ParticipationRole?.Mnemonic).Key);
            return Bundle.CreateBundle(plan);

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
            IConceptRepositoryService conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

            foreach (var p in plan.Action)
                p.Participations.ForEach(o => o.ParticipationRoleKey = o.ParticipationRoleKey ?? conceptService.GetConcept(o.ParticipationRole?.Mnemonic).Key);
            return plan;

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
            int tr = 0;
            return this.Query(queryParameters, 0, 100, out tr);
        }

        /// <summary>
        /// Query for care plan objects... Constructs a care plan for all patients matching the specified query parameters
        /// </summary>
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out int totalCount)
        {
            var repositoryService = ApplicationContext.Current.GetService<IRepositoryService<Patient>>();
            if (repositoryService == null)
                throw new InvalidOperationException("Could not find patient repository service");
            
            // Query
            var carePlanner = ApplicationContext.Current.GetService<ICarePlanService>();

            Expression<Func<Patient, bool>> queryExpr = QueryExpressionParser.BuildLinqExpression<Patient>(queryParameters);
            List<String> queryId = null;
            IEnumerable<Patient> patients = null;
            if (queryParameters.TryGetValue("_queryId", out queryId) && repositoryService is IPersistableQueryRepositoryService)
                patients = (repositoryService as IPersistableQueryRepositoryService).Find(queryExpr, offset, count, out totalCount, new Guid(queryId[0]));
            else
                patients = repositoryService.Find(queryExpr, offset, count, out totalCount);

            // Create care plan for the patients
            IConceptRepositoryService conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
            return patients.AsParallel().Select(o => {
                var plan = carePlanner.CreateCarePlan(o);
                foreach (var p in plan.Action)
                    p.Participations.ForEach(x => x.ParticipationRoleKey = x.ParticipationRoleKey ?? conceptService.GetConcept(x.ParticipationRole?.Mnemonic).Key);
                return plan;
            });

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
