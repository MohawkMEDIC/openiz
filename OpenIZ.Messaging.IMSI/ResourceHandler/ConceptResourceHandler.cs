/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Core;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace OpenIZ.Messaging.IMSI.ResourceHandler
{
	/// <summary>
	/// A resource handler for a concept
	/// </summary>
	public class ConceptResourceHandler : IResourceHandler
	{
		/// <summary>
		/// Gets the resource name
		/// </summary>
		public string ResourceName => nameof(Concept);

		/// <summary>
		/// Gets the model type of the handler
		/// </summary>
		public Type Type => typeof(Concept);

		/// <summary>
		/// Create the specified object in the database
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
		public IdentifiedData Create(IdentifiedData data, bool updateIfExists)
		{
			var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
			{
				throw new InvalidOperationException("Bundle must have entry of type Concept");
			}

			if (processData is Concept)
			{
				return updateIfExists ? conceptService.SaveConcept(processData as Concept) : conceptService.InsertConcept(processData as Concept);
			}

			throw new ArgumentException("Invalid persistence type");
		}

        /// <summary>
        /// Get the specified instance
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IdentifiedData Get(Guid id, Guid versionId)
		{
			var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
			return conceptService.GetConcept(id, versionId);
		}

		/// <summary>
		/// Obsolete the specified concept
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
		public IdentifiedData Obsolete(Guid key)
		{
			var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();
			return conceptService.ObsoleteConcept(key);
		}

        /// <summary>
        /// Query the specified data
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters)
		{
            int tr = 0;
			return this.Query(queryParameters, 0, 100, out tr);
		}

        /// <summary>
        /// Query with offsets
        /// </summary>
        [PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.ReadMetadata)]
        public IEnumerable<IdentifiedData> Query(NameValueCollection queryParameters, int offset, int count, out Int32 totalCount)
		{
            var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

            var filter = QueryExpressionParser.BuildLinqExpression<Concept>(queryParameters);
            List<String> queryId = null;
            if (conceptService is IPersistableQueryRepositoryService && queryParameters.TryGetValue("_queryId", out queryId))
                return (conceptService as IPersistableQueryRepositoryService).Find(filter, offset, count, out totalCount, Guid.Parse(queryId[0]));
            else
                return conceptService.FindConcepts(filter, offset, count, out totalCount);
            
		}

		/// <summary>
		/// Update the specified data
		/// </summary>
		[PolicyPermission(SecurityAction.Demand, PolicyId = PermissionPolicyIdentifiers.AdministerConceptDictionary)]
		public IdentifiedData Update(IdentifiedData data)
		{
			var conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

			Bundle bundleData = data as Bundle;
			bundleData?.Reconstitute();
			var processData = bundleData?.Entry ?? data;

			if (processData is Bundle)
				throw new InvalidOperationException("Bundle must have entry of type Concept");
			else if (processData is Concept)
			{
				var conceptData = processData as Concept;
				return conceptService.SaveConcept(conceptData);
			}
			else
				throw new ArgumentException("Invalid persistence type");
		}
	}
}